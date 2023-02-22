using ShootSystem;
using SkillSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterMsgCenter))]
public abstract class FSM : MonoBehaviour
{
    [SerializeField]
    private IState m_currentState; //当前状态

    public float CharacterWidthRadius = 0f;

    public float CharacterHight = 0f;

    public float DeathDissolveSeconds = 5f;

    public float TimeScale { get; protected set; } = 1;

    public class MsgOnFSMUpdate : EventsManagerBaseMsg { }
    public class MsgOnFSMFixedUpdate : EventsManagerBaseMsg { }

    public MeshRenderer MeshReander { set; get; }

    protected IState currentState 
    { 
        get => m_currentState; 
        set
        {
            m_lastState = m_currentState;
            m_currentState = value;
        }
    }

    public IState CurrentState { get => currentState; }

    private IState m_lastState;

    public IState LastState { get => m_lastState; }

    private StateType m_currentStateType;
    public StateType CurrentStateType
    {
        protected set
        {
            m_lasstStateType = m_currentStateType;
            m_currentStateType = value;
        }
        get => m_currentStateType;
    }

    private StateType m_lasstStateType;
    public StateType LastStateType { get => m_lasstStateType; }


    protected Dictionary<StateType, IState> states = new Dictionary<StateType, IState>(); //注册的状态索引字典

    protected CharacterMsgCenter msgCenter;

    public UIHealthBar HealthBar { get;set; }

    public StateParameter parameter;
    public Transform AttackPoint;

    private List<GameObject> m_recycleAIPointList = new List<GameObject>();

    float m_waitSeconds;
    WaitForSecondsRealtime m_waitForLostTarget;
    IEnumerator m_waitRoutine;

    /// <summary>
    /// 触地板或者触墙体事件
    /// </summary>
    protected class MsgOnTilemapCollisionEnter2D : EventsManagerBaseMsg { }

    private void Awake()
    {
        InitAwake();
        m_waitSeconds = parameter.lostTargetTime;
        m_waitForLostTarget = new WaitForSecondsRealtime(parameter.lostTargetTime);
    }

    protected virtual void InitParameter()
    {
        parameter = new StateParameter();
    }

    /// <summary>
    /// 初始化状态参数
    /// </summary>
    protected virtual void Start()
    {
        parameter.animator = transform.GetComponent<Animator>();
        parameter.attackPoint = AttackPoint;
        msgCenter = GetComponent<CharacterMsgCenter>();
        MeshReander = GetComponent<MeshRenderer>();
        m_mpb = new MaterialPropertyBlock();

        InitCharacterWidthHight();

        //添加重叠伤害事件
        msgCenter.RegisterMsgEvent<MsgOnFSMFixedUpdate>(overHitEvent);

        //简单模式判断
        if (Game.Instance.SaveSystem.currentSave.DifficultyMode != 0)
            parameter.health /= 2;

        InitAIState();

        InitWayPointLogic();


        //监听进入时缓事件
        EventCenter.Instance.AddEventListener<SlowTimeArgs>(Consts.E_BUFF_TimeSlow, slowTimeEvent);

        //监听退出时缓事件
        EventCenter.Instance.AddEventListener(Consts.E_BUFF_TimeNormal, normalTimeEvent);

        

    }

    protected virtual void slowTimeEvent(SlowTimeArgs args)
    {
        TimeScale = args.magnification;
        parameter.animator.SetFloat("Speed", TimeScale);
    }

    protected virtual void normalTimeEvent()
    {
        TimeScale = 1;
        parameter.animator.SetFloat("Speed", TimeScale);
    }

    protected virtual void overHitEvent()
    {
        if (!parameter.target || parameter.health <= 0 || PlayerManager.Instance.transform.Find("Buff").GetComponentInChildren<ShieldDeployer>())
            return;
        Vector2 selfPos = transform.position;
        Vector2 targetfPos = parameter.target.transform.position;
        if (targetfPos.x > selfPos.x - CharacterWidthRadius &&
            targetfPos.x < selfPos.x + CharacterWidthRadius &&
            targetfPos.y < selfPos.y + CharacterHight &&
            targetfPos.y + 1/*补正*/ >= selfPos.y)
            parameter.target.GetComponent<RealCharacterController>().Hurt(selfPos.x - targetfPos.x > 0 ? 1 : -1, 1);
    }

    protected virtual void InitCharacterWidthHight()
    {
        if (CharacterWidthRadius <= 0 || CharacterHight <= 0)
        {
            float maxWidth = 0, maxHight = 0;
            foreach (var collider2d in GetComponentsInChildren<BoxCollider2D>())
            {
                if (!collider2d.isTrigger && collider2d.size.x > maxWidth)
                    maxWidth = collider2d.size.x;
                if (!collider2d.isTrigger && collider2d.size.y > maxHight)
                    maxHight = collider2d.size.y;
            }
            float maxWidthRadius = maxWidth / 2;
            if (CharacterWidthRadius <= 0 || CharacterWidthRadius < maxWidthRadius)
                CharacterWidthRadius = maxWidthRadius;
            if (CharacterHight <= 0 || CharacterHight < maxHight)
                CharacterHight = maxHight;
        }
        CharacterWidthRadius *= transform.localScale.x;
        CharacterHight *= transform.localScale.y;
    }

    protected virtual void InitAwake()
    {
        //初始化 AttackPoint 和 muzzle 位置
        if (!AttackPoint)
            AttackPoint = transform.Find("AttackPoint");
        GameObject muzzle = new GameObject("Muzzle");
        muzzle.transform.parent = transform;
        muzzle.transform.localPosition = Vector3.zero;
        muzzle.transform.localRotation = Quaternion.identity;
        if (AttackPoint != null)
            muzzle.transform.position = AttackPoint.position;
        var fc = GetComponent<FireController>();

        List<Transform> tempPPoints = new List<Transform>();
        List<Transform> tempCPoints = new List<Transform>();
        if(parameter!=null)
        {
            tempPPoints = parameter.patrolPoints;
            tempCPoints = parameter.chasePoints;
        }
        InitParameter();
        parameter.patrolPoints = tempPPoints;
        parameter.chasePoints = tempCPoints;
    }



    /// <summary>
    /// 生成AI时初始化AI状态的过程
    /// </summary>
    protected virtual void InitAIState()
    {
        //添加状态
        states.Add(StateType.Idle, new State.IdleState(this));
        states.Add(StateType.Patrol, new State.PatrolState(this));
        states.Add(StateType.Chase, new State.ChaseState(this));
        states.Add(StateType.React, new State.ReactState(this));
        states.Add(StateType.Attack, new State.AttackState(this));
        states.Add(StateType.Hit, new State.HitState(this));
        states.Add(StateType.Death, new State.DeathState(this));

        //选择初始状态
        TransitionState(StateType.Idle);
    }

    protected virtual void Update()
    {
        currentState.OnUpdate();
        msgCenter.BroadcastMsg<MsgOnFSMUpdate>();
    }

    protected void FixedUpdate()
    {
        currentState.OnFixedUpdate();
        msgCenter.BroadcastMsg<MsgOnFSMFixedUpdate>();
    }

    //状态转移
    public void TransitionState(StateType type)
    {
        //执行前一个状态的退出函数
        if (currentState != null)
            currentState.OnExit();
        //将当前状态切换为指定状态
        currentState = states[type];
        CurrentStateType = type;
        currentState.OnEnter();
    }

    /// <summary>
    /// 面向目标方向
    /// </summary>
    /// <param name="target"></param>
    public virtual void FlipTo(Transform target)
    {
        if (target != null)
        {
            if (transform.position.x > target.position.x)
            {
                transform.localEulerAngles = new Vector3(0, 0, 0);
            }
            else if (transform.position.x < target.position.x)
            {
                transform.localEulerAngles = new Vector3(0, 180, 0);
            }
        }
    }


    /// <summary>
    /// 由怪物生成AIPoint
    /// </summary>
    protected virtual void spawnAIPoints()
    {

        Vector3 underOffset = new Vector3(0, -0.2f, 0);
        Vector3 onOffset = new Vector3(0, 0.2f, 0);

        //参数中未预设 PatrolPoint 物体，由怪物自动生成
        if (parameter.patrolPoints.Count == 0)
        {
            float radius = parameter.spawnPatrolRadius == 0 ? 1 : parameter.spawnPatrolRadius;
            GameObject patrolPointL;
            if (Game.Instance && Game.Instance.ObjectPool)
                patrolPointL = Game.Instance.ObjectPool.GetObject("Prefabs/AIPoint/PatrolPoint");
            else
                patrolPointL = Resources.Load<GameObject>("Prefabs/AIPoint/PatrolPoint");


            Vector3 leftPos = transform.position + new Vector3(-radius, 0, 0);
            //墙面阻挡检测
            RaycastHit2D hitLOnGround = Physics2D.Raycast(transform.position, new Vector3(-radius , 0, 0),radius + CharacterWidthRadius, 1<<LayerMask.NameToLayer("NormalPlatForm"));
            if (hitLOnGround && hitLOnGround.point.x > leftPos.x-CharacterWidthRadius)
                leftPos = hitLOnGround.point + new Vector2(CharacterWidthRadius, 0f);

            //地面检测
            RaycastHit2D hitLUnderGround = Physics2D.Raycast(leftPos+onOffset, new Vector2(0, -1f), 0.5f, 1 << LayerMask.NameToLayer("NormalPlatForm"));
            if (!hitLUnderGround)
            {
                float posX = Physics2D.Raycast(leftPos + underOffset, new Vector2(1, 0), radius, 1 << LayerMask.NameToLayer("NormalPlatForm")).point.x;
                leftPos = new Vector2((posX == 0 ? leftPos.x: posX )+ 0.2f, leftPos.y);
            }

            patrolPointL.transform.position = leftPos;

            GameObject patrolPointR;
            if (Game.Instance && Game.Instance.ObjectPool)
                patrolPointR = Game.Instance.ObjectPool.GetObject("Prefabs/AIPoint/PatrolPoint");
            else
                patrolPointR = Resources.Load<GameObject>("Prefabs/AIPoint/PatrolPoint");

            Vector3 rightPos = transform.position + new Vector3(radius, 0, 0);
            //墙面阻挡检测
            RaycastHit2D hitROnGround = Physics2D.Raycast(transform.position, new Vector3(radius, 0, 0), radius + CharacterWidthRadius, 1 << LayerMask.NameToLayer("NormalPlatForm"));
            if (hitROnGround && hitROnGround.point.x < rightPos.x+CharacterWidthRadius)
                rightPos = hitROnGround.point + new Vector2(-CharacterWidthRadius, 0f);

            //地面检测
            RaycastHit2D hitRUnderGround = Physics2D.Raycast(rightPos+onOffset, new Vector2(0, -1f), 0.5f, 1 << LayerMask.NameToLayer("NormalPlatForm"));
            if (!hitRUnderGround)
            {
                float posX = Physics2D.Raycast(rightPos + underOffset, new Vector2(-1, 0), radius, 1 << LayerMask.NameToLayer("NormalPlatForm")).point.x;
                rightPos = new Vector2((posX==0?rightPos.x: posX) - 0.2f, rightPos.y);
            }


            patrolPointR.transform.position = rightPos;

            parameter.patrolPoints.Add(patrolPointL.transform);
            parameter.patrolPoints.Add(patrolPointR.transform);
            m_recycleAIPointList.Add(patrolPointL);
            m_recycleAIPointList.Add(patrolPointR);
        }

        //参数中未预设 ChasePoint 物体，由怪物自动生成
        if (parameter.chasePoints.Count == 0)
        {
            float radius = parameter.spawnChaseRadius == 0 ? 1 : parameter.spawnChaseRadius;
            GameObject chaselPointL;
            if (Game.Instance && Game.Instance.ObjectPool)
                chaselPointL = Game.Instance.ObjectPool.GetObject("Prefabs/AIPoint/ChasePoint");
            else
                chaselPointL = Resources.Load<GameObject>("Prefabs/AIPoint/ChasePoint");

            Vector3 leftPos = transform.position + new Vector3(-radius, 0, 0);
            //墙面阻挡检测
            RaycastHit2D hitLOnGround = Physics2D.Raycast(transform.position, new Vector3(-radius, 0, 0), radius, 1<<LayerMask.NameToLayer("NormalPlatForm"));
            if (hitLOnGround && hitLOnGround.point.x > leftPos.x)
                leftPos = hitLOnGround.point;

            //地面检测
            RaycastHit2D hitLUnderGround = Physics2D.Raycast(leftPos+onOffset, new Vector2(0, -1f), 0.5f, 1 << LayerMask.NameToLayer("NormalPlatForm"));
            if (!hitLUnderGround)
            {
                float posX = Physics2D.Raycast(leftPos + underOffset, new Vector2(1, 0), radius, 1 << LayerMask.NameToLayer("NormalPlatForm")).point.x;
                leftPos = new Vector2((posX == 0 ? leftPos.x : posX)+ 0.2f, leftPos.y);
            }

            chaselPointL.transform.position = leftPos;


            GameObject chaselPointR;
            if (Game.Instance && Game.Instance.ObjectPool)
                chaselPointR = Game.Instance.ObjectPool.GetObject("Prefabs/AIPoint/ChasePoint");
            else
                chaselPointR = Resources.Load<GameObject>("Prefabs/AIPoint/ChasePoint");

            Vector3 rightPos = transform.position + new Vector3(radius, 0, 0);
            //墙面阻挡检测
            RaycastHit2D hitROnGround = Physics2D.Raycast(transform.position, new Vector3(radius, 0, 0), radius, 1<<LayerMask.NameToLayer("NormalPlatForm"));
            if (hitROnGround && hitROnGround.point.x < rightPos.x)
                rightPos = hitROnGround.point;


            //地面检测
            RaycastHit2D hitRUnderGround = Physics2D.Raycast(rightPos+onOffset, new Vector2(0, -1f), 0.5f, 1 << LayerMask.NameToLayer("NormalPlatForm"));
            if (!hitLUnderGround)
            {
                float posX = Physics2D.Raycast(leftPos + underOffset, new Vector2(1, 0), radius, 1 << LayerMask.NameToLayer("NormalPlatForm")).point.x;
                leftPos = new Vector2((posX == 0 ? leftPos.x : posX) + 0.2f, leftPos.y);
            }

            chaselPointR.transform.position = rightPos;

            parameter.chasePoints.Add(chaselPointL.transform);
            parameter.chasePoints.Add(chaselPointR.transform);
            m_recycleAIPointList.Add(chaselPointL);
            m_recycleAIPointList.Add(chaselPointR);
        }
    }


    /// <summary>
    /// 回收所有AIPoint
    /// </summary>
    protected virtual void recycleAIPoints()
    {
        foreach (var point in m_recycleAIPointList)
        {
            if(point)
                Game.Instance.ObjectPool.RecycleObject(point);
        }
    }

    protected virtual void OnDestroy()
    {
        recycleAIPoints();

        //移除进入时缓事件
        EventCenter.Instance.RemoveEventListener<SlowTimeArgs>(Consts.E_BUFF_TimeSlow, slowTimeEvent);

        //移除退出时缓事件
        EventCenter.Instance.RemoveEventListener(Consts.E_BUFF_TimeNormal, normalTimeEvent);

        //移除healthBar
        if (HealthBar)
            UIManager.Instance.HideDynamicPanel(GetInstanceID());

    }

    /// <summary>
    /// 用于在AI落地时生成AI寻路点
    /// </summary>
    /// <param name="collision"></param>
    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        //判断是否是触地板或者触墙体
        TilemapCollider2D tilemapCollider = collision.collider.GetComponent<TilemapCollider2D>();
        if (tilemapCollider)
        {
            //广播触地消息
            msgCenter.BroadcastMsg<MsgOnTilemapCollisionEnter2D, TilemapCollider2D>(tilemapCollider);
        }
    }

    /// <summary>
    /// 添加丢失目标协程
    /// </summary>
    protected void addWaitForClearTarget()
    {
        if (Mathf.Abs(m_waitSeconds - parameter.lostTargetTime) < 0.1f)
        {
            //与初始化时的等待配置相同直接引用不产生GC
            m_waitRoutine = _waitforHelper(m_waitForLostTarget);
            StartCoroutine(m_waitRoutine);
        }
        else
        {
            //与初始化的配置不同根据现在的值生成新的等待时间
            m_waitRoutine = _waitforHelper(new WaitForSecondsRealtime(parameter.lostTargetTime));
            StartCoroutine(m_waitRoutine);
        }
    }

    /// <summary>
    /// 移除丢失目标的协程
    /// </summary>
    protected void removeWaitForClearTarget()
    {
        if (m_waitRoutine != null)
            StopCoroutine(m_waitRoutine);
        m_waitRoutine = null;
    }

    IEnumerator _waitforHelper(WaitForSecondsRealtime waitForSeconds)
    {
        yield return waitForSeconds;
        parameter.target = null;
    }


    protected virtual void InitWayPointLogic()
    {
        //绑定触墙消息
        msgCenter.RegisterMsgEvent<MsgOnTilemapCollisionEnter2D, TilemapCollider2D>((TilemapCollider2D co) =>
        {
            //触地生成寻路点
            spawnAIPoints();
            ////脚下有地面
            //var hit = Physics2D.Raycast(transform.position, new Vector2(0, -1), 1f, LayerMask.GetMask(new[] { "NormalPlatForm" }));

            //if (hit)
            //{
                //触地生成寻路点
                //spawnAIPoints();
            //}
        });

        //添加索敌触发逻辑
        msgCenter.RegisterMsgEvent<OnTriggerMsgBroadcast.MsgOnTriggerEnter2D, Collider2D>((Collider2D other) =>
        {
            
            if (other.CompareTag("Player"))
            {
                //清除可能存在的脱索协程
                removeWaitForClearTarget();
                parameter.target = other.transform;
            }
        });
        //脱索逻辑
        msgCenter.RegisterMsgEvent<OnTriggerMsgBroadcast.MsgOnTriggerExit2D, Collider2D>((Collider2D other) =>
        {
            if (other.CompareTag("Player"))
            {
                //添加脱索协程
                addWaitForClearTarget();
            }
        });
    }

    protected virtual void die(string str)
    {
        StartCoroutine(dieDissolveHelper(DeathDissolveSeconds));
        //Destroy(gameObject, DeathDissolveSeconds);
    }

    WaitForFixedUpdate wtffu = new WaitForFixedUpdate();

    MaterialPropertyBlock m_mpb;

    protected IEnumerator dieDissolveHelper(float seconds)
    {
        float startSec = seconds;
        while (seconds>=0)
        {
            float amounnt = seconds > 0 ? 1.3f * (1 - seconds / startSec) : 0;
            m_mpb.SetFloat("_DissolveAmount", amounnt);
            MeshReander.SetPropertyBlock(m_mpb);
            yield return wtffu;
            seconds -= Time.fixedDeltaTime;
        }
        Destroy(gameObject);
    }
}

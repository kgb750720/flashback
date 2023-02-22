using CharaterMsg;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(EnemyMeleeAttack))]
public class NarcissFSM : FSM
{
    protected Dictionary<Transform,CharacterMsgCenter> _paleyrMsgCenters = new Dictionary<Transform, CharacterMsgCenter>();

    protected Transform _currActivePlayer;
    public Transform CurrActivePlayer { get => _currActivePlayer; }
    public float WaitActionSeconds = 1f;
    public int TeleprotMaxCount = 3;
    int m_teleportCount;
    float m_teleportRecoverTimer = -1;
    public float TeleprotRecoverSeconds = 5f;

    public Transform Stage2HidePos;
    public List<NarcissSwordStage2> Stage2Swords = new List<NarcissSwordStage2>();
    public float Stage2FireInterval = 20f;
    public float Stage2ChargeSeconds = 3f;

    public Transform Stage3LaserStandPos;
    public int Stage3MeleeAttackPrepareCount = 2;
    public List<NarcissSwordStage3> Stage3Swords = new List<NarcissSwordStage3>();
    public int Stage3LaserAttackPrepareCount = 4;
    public float Stage3ChargeSeconds = 1f;


    private List<GameObject> m_recycleAIPointList = new List<GameObject>();
    protected NarcissStateParameter _narcissParameter;

    float m_stage2FireTimer = 0;

    EnemyMeleeAttack m_melee;

    Rigidbody2D m_rigbody;

    [HideInInspector]
    public UIBossHealthBar BossHealthBar;

    protected override void InitParameter()
    {
        _narcissParameter= new NarcissStateParameter();
        _narcissParameter.bossCurrStage = BossStage.Stage1;
        parameter = _narcissParameter;
    }


    protected override void Start()
    {
        base.Start();
        StartCoroutine(setPlayerMsgCenters());

        //开场等待
        m_wtfs = new WaitForSeconds(WaitActionSeconds);
        StartCoroutine(setBossChase());


        //EventCenter.Instance.AddEventListener<DialogCompleteArgs>(Consts.E_TalkCompleted,talkCompleted);
        SetStage2SwordsActive(false);
        m_stage2FireTimer = Stage2FireInterval - 1;
        m_melee = GetComponent<EnemyMeleeAttack>();
        m_rigbody = GetComponent<Rigidbody2D>();
        SetStage3SwordsActive(false);
        m_teleportCount = TeleprotMaxCount;
        msgCenter.RegisterMsgEvent<MsgOnFSMFixedUpdate>(teleprotRecoverFixedUpdate);
    }

    WaitForSeconds m_wtfs;

    private IEnumerator setBossChase()
    {
        yield return m_wtfs;
        parameter.target = _currActivePlayer;
        TransitionState(StateType.Jump);
    }
    //void talkCompleted(DialogCompleteArgs args)
    //{
    //    parameter.target = _currActivePlayer;
    //    TransitionState(StateType.Jump);
    //    print("对话完成Boss开始攻击");
    //}

    void teleprotRecoverFixedUpdate()
    {
        if (m_teleportCount < TeleprotMaxCount && m_teleportRecoverTimer >= 0)
            m_teleportRecoverTimer += Time.fixedDeltaTime;

        if (m_teleportRecoverTimer >= TeleprotRecoverSeconds)
        {
            m_teleportCount++;
            m_teleportRecoverTimer = -1;
        }

        if (m_teleportCount < TeleprotMaxCount && m_teleportRecoverTimer < 0)
            m_teleportRecoverTimer = 0;
    }

    public void SetStage2SwordsActive(bool enable)
    {
        foreach (var item in Stage2Swords)
        {
            if (item)
            {
                item.gameObject.SetActive(enable);
                if (!item.FSM)
                    item.FSM = this;
            }
        }
    }
    public void SetStage3SwordsActive(bool enable)
    {
        foreach (var item in Stage3Swords)
        {
            if (item)
                item.gameObject.SetActive(enable);
        }
    }

    /// <summary>
    /// Ϊ��ҿ�����������Ϣ�ַ��Ա���boss��ȡ���״̬
    /// </summary>
    /// <returns></returns>
    private IEnumerator setPlayerMsgCenters()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        Transform pmTrans = PlayerManager.Instance.transform;
        for (int i = 0; i < pmTrans.childCount; i++)
        {
            var item = pmTrans.GetChild(i).GetComponent<RealCharacterController>();
            if (!item)
                continue;
            CharacterMsgCenter playerMsgCenter = item.gameObject.AddComponent<CharacterMsgCenter>();
            item.gameObject.AddComponent<OnCollisionMsgBroadcast>();
            item.gameObject.AddComponent<OnDisableMsgBroadcast>();
            playerMsgCenter.RegisterMsgEvent<OnCollisionMsgBroadcast.MsgOnCollision2DEnter, Collision2D>(onPlayerCollisionEnterMsgBroadcast);
            playerMsgCenter.RegisterMsgEvent<OnDisableMsgBroadcast.MsgOnEnable, GameObject>(onPlayerEnableMsgBroadcast);
            _paleyrMsgCenters.Add(item.transform, playerMsgCenter);
            if (item.gameObject.activeSelf)
                _currActivePlayer = item.transform;
        }
    }

    protected override void InitAIState()
    {
        states.Add(StateType.Idle, new State.NarcissIdleState(this));
        states.Add(StateType.Chase, new State.NarcissChaseState(this));
        states.Add(StateType.Jump, new State.NarcissJumpState(this));
        states.Add(StateType.Hit, new State.NarcissHitState(this));
        states.Add(StateType.Attack, new State.NarcissAttackState(this));
        states.Add(StateType.Death,new State.CustomDeathState(this));
        states.Add(StateType.React, new State.NarcissReactState(this));
        TransitionState(StateType.Idle);
    }

    protected override void InitWayPointLogic()
    {
        //�󶨴�ǽ��Ϣ
        //msgCenter.RegisterMsgEvent<MsgOnTilemapCollisionEnter2D, TilemapCollider2D>((TilemapCollider2D co) =>
        //{
        //    //��������Ѱ·��
        //    spawnAIPoints();
        //});
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        foreach (var item in _paleyrMsgCenters.Values)
        {
            if (!item)
                continue;
            item.RemoveMsgEvent<OnCollisionMsgBroadcast.MsgOnCollision2DEnter, Collision2D>(onPlayerCollisionEnterMsgBroadcast);
            item.RemoveMsgEvent<OnDisableMsgBroadcast.MsgOnEnable, GameObject>(onPlayerEnableMsgBroadcast);
            OnCollisionMsgBroadcast ocemb = item.GetComponent<OnCollisionMsgBroadcast>();
            OnDisableMsgBroadcast odmb= item.GetComponent<OnDisableMsgBroadcast>();
            Destroy(ocemb);
            Destroy(odmb);
            Destroy(item);
        }
        if (BossHealthBar)
            UIManager.Instance.HideDynamicPanel(GetInstanceID());
        //Game.Instance.EventCenter.RemoveEventListener<DialogCompleteArgs>(Consts.B_BossDeath, talkCompleted);
    }

    protected override void overHitEvent()
    {
        if(parameter.animator.GetFloat("AttackType")!=2)
            base.overHitEvent();
    }

    private void onPlayerCollisionEnterMsgBroadcast(Collision2D co)
    {

        //�ж�����Ƿ�վ�ڵ�����
        var hit = Physics2D.Raycast(_currActivePlayer.position, new Vector2(0, -1), 1f, LayerMask.GetMask(new[] { "NormalPlatForm" }));

        //����ܵ�����ƽ̨
        if (hit && parameter.chasePoints.Count>0 && (
            _currActivePlayer.position.y != parameter.chasePoints[0].position.y ||
            _currActivePlayer.position.x < parameter.chasePoints[0].position.x ||
            _currActivePlayer.position.x > parameter.chasePoints[1].position.x))
        {
            switch (_narcissParameter.bossCurrStage)
            {
                case BossStage.Stage1:
                    if (m_teleportCount > 0)
                    {
                        m_teleportCount--;
                        TransitionState(StateType.Jump);
                    }
                    break;
                case BossStage.Stage2:
                    break;
                case BossStage.Stage3:
                    if (CurrentStateType != StateType.React&&CurrentStateType!=StateType.Hit && m_teleportCount > 0)
                    {
                        m_teleportCount--;
                        TransitionState(StateType.Jump);
                    }
                    break;
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// �������׶�
    /// </summary>
    private void startStage2()
    {
        MoveToResetAIPoint(Stage2HidePos.position);
        MoveToFartherChasePoint(Stage2HidePos.position);
        parameter.target = null;
        SetStage2SwordsActive(true);
        msgCenter.RegisterMsgEvent<MsgOnFSMFixedUpdate>(stage2FireFixedUpdate);
    }

    private void stage2FireFixedUpdate()
    {
        m_stage2FireTimer += Time.fixedDeltaTime * TimeScale;
        if (m_stage2FireTimer >= Stage2FireInterval)
        {
            foreach (var item in Stage2Swords)
            {
                if (item)
                {
                    item.target = _currActivePlayer;
                    void fireCallBack()
                    {
                        item.target = null;
                        item.FireCallback -= fireCallBack;
                    }
                    item.LaserShoot(Stage2ChargeSeconds, fireCallBack);
                }
            }
            m_stage2FireTimer = 0;
        }

        if(Stage2Swords.Count==0)
        {
            msgCenter.RemoveMsgEvent<MsgOnFSMFixedUpdate>(stage2FireFixedUpdate);
            TransitionStage(BossStage.Stage3);
        }
    }

    /// <summary>
    /// �������׶�
    /// </summary>
    private void startStage3()
    {
        _narcissParameter.target = _currActivePlayer;
        TransitionState(StateType.Chase);
        TransitionState(StateType.Jump);
    }

    /// <summary>
    /// �ƶ����������Զһ���ƽ̨�߽�
    /// </summary>
    public void MoveToFartherChasePoint(Vector2 pos)
    {
        Vector2 targetPos = Vector2.Distance(pos, parameter.chasePoints[0].position) >
            Vector2.Distance(pos, parameter.chasePoints[1].position) ?
            parameter.chasePoints[0].position :
            parameter.chasePoints[1].position;
        transform.position = targetPos;
    }

    //���������˻ص�
    private void onPlayerEnableMsgBroadcast(GameObject go)
    {
        _currActivePlayer = go.transform;
    }

    public virtual void MoveToResetAIPoint(Vector3 targetPos)
    {
        foreach (var item in m_recycleAIPointList)
        {
            Game.Instance.ObjectPool.RecycleObject(item);
        }
        parameter.chasePoints.Clear();
        foreach (var item in m_recycleAIPointList)
        {
            ObjectPool.Instance.RecycleObject(item);
        }
        m_recycleAIPointList.Clear();
        transform.position = targetPos;
        spawnAIPoints();
    }

    protected override void spawnAIPoints()
    {
        Vector3 underOffset = new Vector3(0, -0.5f, 0);
        Vector3 onOffset = new Vector3(0, 0.2f, 0);
        //������δԤ�� ChasePoint ���壬�ɹ����Զ�����
        if (parameter.chasePoints.Count == 0)
        {
            float radius = parameter.spawnChaseRadius;
            GameObject chaselPointL = Game.Instance.ObjectPool.GetObject("Prefabs/AIPoint/ChasePoint");

            Vector3 leftPos = transform.position + new Vector3(-radius, 0, 0);
            //ǽ���赲���
            RaycastHit2D hitLOnGround = Physics2D.Raycast(transform.position, new Vector3(-radius, 0, 0), radius + CharacterWidthRadius, 1 << LayerMask.NameToLayer("NormalPlatForm"));
            if (hitLOnGround && hitLOnGround.point.x > leftPos.x - CharacterWidthRadius)
                leftPos = hitLOnGround.point/* + new Vector2(CharacterWidthRadius, 0f)*/;

            //������
            RaycastHit2D hitLUnderGround = Physics2D.Raycast(leftPos + onOffset, new Vector2(0, -1f), 0.5f, 1 << LayerMask.NameToLayer("NormalPlatForm"));
            if (!hitLUnderGround)
            {
                var hit = Physics2D.Raycast(leftPos + underOffset, new Vector2(1, 0), radius, 1 << LayerMask.NameToLayer("NormalPlatForm"));
                if(hit)
                    leftPos = new Vector2( hit.point.x + 0.2f, leftPos.y);
            }
            chaselPointL.transform.position = leftPos + new Vector3(CharacterWidthRadius, 0f);

            GameObject chaselPointR = Game.Instance.ObjectPool.GetObject("Prefabs/AIPoint/ChasePoint");

            Vector3 rightPos = transform.position + new Vector3(radius, 0, 0);
            //ǽ���赲���
            RaycastHit2D hitROnGround = Physics2D.Raycast(transform.position, new Vector3(radius, 0, 0), radius + CharacterWidthRadius, 1 << LayerMask.NameToLayer("NormalPlatForm"));
            if (hitROnGround && hitROnGround.point.x < rightPos.x + CharacterWidthRadius)
                rightPos = hitROnGround.point/* + new Vector2(-CharacterWidthRadius, 0f)*/;

            //������
            RaycastHit2D hitRUnderGround = Physics2D.Raycast(rightPos + onOffset, new Vector2(0, -1f), 0.5f, 1 << LayerMask.NameToLayer("NormalPlatForm"));
            if (!hitRUnderGround)
            {
                var hit = Physics2D.Raycast(rightPos + underOffset, new Vector2(-1, 0), radius, 1 << LayerMask.NameToLayer("NormalPlatForm"));
                if(hit)
                    rightPos = new Vector2(hit.point.x - 0.2f, rightPos.y);
            }

                chaselPointR.transform.position = rightPos + new Vector3(-CharacterWidthRadius, 0f);

            parameter.chasePoints.Add(chaselPointL.transform);
            parameter.chasePoints.Add(chaselPointR.transform);
            m_recycleAIPointList.Add(chaselPointL);
            m_recycleAIPointList.Add(chaselPointR);
        }
    }

    float m_stage3LaserFireTimer = 0;
    int m_stage3LaserFireIdx = 0;
    public void Stage3LaserShoot()
    {
        transform.position = Stage3LaserStandPos.position;
        m_rigbody.velocity = Vector3.zero;
        m_rigbody.bodyType = RigidbodyType2D.Kinematic;
        m_stage3LaserFireTimer = 1;
        m_stage3LaserFireIdx = 0;
        SetStage3SwordsActive(true);
        msgCenter.RegisterMsgEvent<MsgOnFSMFixedUpdate>(stage3fixedUpdade);
    }

    void stage3fixedUpdade()
    {
        m_stage3LaserFireTimer += Time.fixedDeltaTime * TimeScale;
        if(m_stage3LaserFireTimer>=1f)
        {
            m_stage3LaserFireTimer = 0;
            var laser = Stage3Swords[m_stage3LaserFireIdx];
            //laser.gameObject.SetActive(true);
            laser.LaserShoot(Stage3ChargeSeconds);
            void stage3LaserCallback()
            {
                laser.gameObject.SetActive(false);
                laser.LaserEndCallback -= stage3LaserCallback;
            }
            Stage3Swords[m_stage3LaserFireIdx].LaserEndCallback += stage3LaserCallback;
            m_stage3LaserFireIdx++;
            if (m_stage3LaserFireIdx >= Stage3Swords.Count)
            {
                msgCenter.RemoveMsgEvent<MsgOnFSMFixedUpdate>(stage3fixedUpdade);
                m_rigbody.bodyType = RigidbodyType2D.Dynamic;
                parameter.animator.SetBool("Fly", false);
                MoveToResetAIPoint(parameter.target.position + new Vector3(0, 0.3f));
                MoveToFartherChasePoint(CurrActivePlayer.position + new Vector3(0, 0.3f));
                TransitionState(StateType.Chase);
            }
        }

        
    }

    

    void fire(int fireType)
    {
        switch (fireType)
        {
            case 0:
                fireStage1(0.5f, 4);
                break;
            case 2:
                fireStage3Melee();
                break;
            default:
                break;
        }
    }

    void fireTeleport(string str)
    {
        transform.position = _currActivePlayer.position;
    }

    private void fireStage3Melee()
    {
        m_melee.DoMeleeAttack(transform);
    }

    void fireStage1(float second,int times)
    {
        StartCoroutine(fireStage1Helper(new WaitForSeconds(second), times));
    }

    IEnumerator fireStage1Helper(WaitForSeconds waitFor,int times)
    {
        float spacing = 1.5f;
        float hight = CharacterHight / 4;
        for (int i = 0; i < times; i++)
        {
            float swordY = hight + spacing * i;
            
            var go= Game.Instance.ObjectPool.GetObject("Prefabs/Effect/Bullet/ColdBullet/Narciss_SwStage1");
            go.transform.position = transform.position + ((transform.rotation.x != 0 ? transform.right : -transform.right) * 1.5f) + new Vector3(0, swordY);
            go.transform.rotation = transform.rotation;
            go.GetComponent<NarcissSwordStage1>().TimeScale = this.TimeScale;
            yield return waitFor;
        }
    }

    public void TransitionStage(BossStage stage)
    {
        _narcissParameter.bossCurrStage = stage;
        switch (stage)
        {
            case BossStage.Stage1:
                break;
            case BossStage.Stage2:
                startStage2();
                break;
            case BossStage.Stage3:
                startStage3();
                break;
            default:
                break;
        }
        TransitionState(StateType.Jump);
    }

    protected override void die(string str)
    {
        base.die(str);
        EventCenter.Instance.EventTrigger(Consts.B_BossDeath);
    }

}

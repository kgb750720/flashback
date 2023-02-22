using ShootSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(OnCollisionMsgBroadcast))]
public abstract class CustomAerocraftFSM : FSM
{
    public bool hitFall = false;
    public Collider2D DieCushionCollider;
    protected new Rigidbody2D rigidbody2D;
    public FireController FireCtrl;
    private List<GameObject> m_recycleAIPointList = new List<GameObject>();

    protected override void Start()
    {
        base.Start();
        if(!rigidbody2D)
            rigidbody2D = GetComponent<Rigidbody2D>();
        rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
        if (!FireCtrl)
            FireCtrl = GetComponent<FireController>();

        if (!DieCushionCollider)
        {
            Transform dieCushion = transform.Find("DieCushion");
            DieCushionCollider = dieCushion.GetComponent<Collider2D>();
            
        }
        if (!DieCushionCollider)
        {
            Debug.LogWarning("DieCushionCollider 初始化时无绑定！");
        }
        else
        {
            if (!DieCushionCollider.GetComponent<OnCollisionMsgBroadcast>())
                DieCushionCollider.gameObject.AddComponent<OnCollisionMsgBroadcast>();
            DieCushionCollider.enabled = false;
        }


        //若敌人fireControler未被单独配置则利用参数进行配置
        IFireControllerStateParameter ifc = parameter as IFireControllerStateParameter;
        if (ifc != null && FireCtrl.bulletSpeed == 0)
            FireCtrl.bulletSpeed = ifc.bulletSpeed;
        if (ifc != null && FireCtrl.maxDistance == 0)
            FireCtrl.maxDistance = ifc.bulletRange;
    }
    protected override void spawnAIPoints()
    {
        Vector3 underOffset = new Vector3(0, -0.2f, 0);
        Vector3 onOffset = new Vector3(0, 0.2f, 0);

        //参数中未预设 PatrolPoint 物体，由怪物自动生成
        if (parameter.patrolPoints.Count == 0)
        {
            float radius = parameter.spawnPatrolRadius;
            GameObject patrolPointL = Game.Instance.ObjectPool.GetObject("Prefabs/AIPoint/PatrolPoint");


            Vector3 leftPos = transform.position + new Vector3(-radius, 0, 0);
            //墙面阻挡检测
            RaycastHit2D hitLOnGround = Physics2D.Raycast(transform.position, new Vector3(-radius, 0, 0), radius+CharacterWidthRadius, 1 << LayerMask.NameToLayer("NormalPlatForm"));
            if (hitLOnGround && hitLOnGround.point.x > leftPos.x-CharacterWidthRadius)
                leftPos = hitLOnGround.point + new Vector2(CharacterWidthRadius, 0);

            patrolPointL.transform.position = leftPos;

            GameObject patrolPointR = Game.Instance.ObjectPool.GetObject("Prefabs/AIPoint/PatrolPoint");

            Vector3 rightPos = transform.position + new Vector3(radius, 0, 0);
            //墙面阻挡检测
            RaycastHit2D hitROnGround = Physics2D.Raycast(transform.position, new Vector3(radius, 0, 0), radius+ CharacterWidthRadius, 1 << LayerMask.NameToLayer("NormalPlatForm"));
            if (hitROnGround && hitROnGround.point.x < rightPos.x+CharacterWidthRadius)
                rightPos = hitLOnGround.point + new Vector2(-CharacterWidthRadius, 0);



            patrolPointR.transform.position = rightPos;

            parameter.patrolPoints.Add(patrolPointL.transform);
            parameter.patrolPoints.Add(patrolPointR.transform);
            m_recycleAIPointList.Add(patrolPointL);
            m_recycleAIPointList.Add(patrolPointR);
        }

        //参数中未预设 ChasePoint 物体，由怪物自动生成
        if (parameter.chasePoints.Count == 0)
        {
            float radius = parameter.spawnChaseRadius;
            GameObject chaselPointL = Game.Instance.ObjectPool.GetObject("Prefabs/AIPoint/ChasePoint");

            Vector3 leftPos = transform.position + new Vector3(-radius, 0, 0);
            //墙面阻挡检测
            RaycastHit2D hitLOnGround = Physics2D.Raycast(transform.position, new Vector3(-radius, 0, 0), radius, 1 << LayerMask.NameToLayer("NormalPlatForm"));
            if (hitLOnGround && hitLOnGround.point.x > leftPos.x)
                leftPos = hitLOnGround.point;


            chaselPointL.transform.position = leftPos;

            GameObject chaselPointR = Game.Instance.ObjectPool.GetObject("Prefabs/AIPoint/ChasePoint");

            Vector3 rightPos = transform.position + new Vector3(radius, 0, 0);
            //墙面阻挡检测
            RaycastHit2D hitROnGround = Physics2D.Raycast(transform.position, new Vector3(radius, 0, 0), radius, 1 << LayerMask.NameToLayer("NormalPlatForm"));
            if (hitROnGround && hitROnGround.point.x < rightPos.x)
                rightPos = hitROnGround.point;


            chaselPointR.transform.position = rightPos;

            parameter.chasePoints.Add(chaselPointL.transform);
            parameter.chasePoints.Add(chaselPointR.transform);
            m_recycleAIPointList.Add(chaselPointL);
            m_recycleAIPointList.Add(chaselPointR);
        }
    }

    public void BindMsgOnCollision2DEnter(UnityAction<Collision2D> call)
    {
        msgCenter.RegisterMsgEvent<OnCollisionMsgBroadcast.MsgOnCollision2DEnter, Collision2D>(call);
    }

    public void SetRigBodyType(RigidbodyType2D rigbodyType)
    {
        rigidbody2D.bodyType = rigbodyType;
    }

    protected override void InitWayPointLogic()
    {
        spawnAIPoints();
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

    protected override void recycleAIPoints()
    {
        foreach (var point in m_recycleAIPointList)
        {
            Game.Instance.ObjectPool.RecycleObject(point);
        }
    }


    protected override void InitAIState()
    {
        //添加状态
        states.Add(StateType.Idle, new State.CustomIdleState(this));
        states.Add(StateType.Patrol, new State.CustomPatrolState(this));
        states.Add(StateType.Chase, new State.CustomAerocraftChaseState(this));
        states.Add(StateType.Attack, new State.CustomAttackState(this));
        states.Add(StateType.React, new State.CustomReactState(this));
        states.Add(StateType.Fall, new State.CustomAerocraftFallState(this));
        states.Add(StateType.Hit, new State.CustomAerocraftHitState(this));
        states.Add(StateType.Death, new State.CustomAerocraftDeathState(this));

        //选择初始状态
        TransitionState(StateType.Idle);
    }

    /// <summary>
    /// 动画事件回调
    /// </summary>
    /// <param name="str"></param>
    protected virtual void fire(string str)
    {
        if (parameter.target)
        {
            var direct = parameter.target.position - transform.position;
            //DoShoot有bug，需要补正
            //if (transform.localEulerAngles.y == 0)
            //    direct.y = -direct.y;
            FireCtrl.DoShoot(direct.normalized);
        }
    }

    protected override void InitCharacterWidthHight()
    {
        if (CharacterWidthRadius <= 0 || CharacterHight <= 0)
        {
            float maxWidth = 0, maxHight = 0;
            foreach (var collider2d in GetComponents<BoxCollider2D>())
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
    }

    protected override void overHitEvent()
    {
        if (!parameter.target || parameter.health <= 0)
            return;
        Vector2 selfPos = transform.position;
        Vector2 targetfPos = parameter.target.transform.position;
        if (targetfPos.x > selfPos.x - CharacterWidthRadius &&
            targetfPos.x < selfPos.x + CharacterWidthRadius &&
            targetfPos.y < selfPos.y + CharacterHight / 2 &&
            targetfPos.y > selfPos.y - CharacterHight / 2)
            parameter.target.GetComponent<RealCharacterController>().Hurt(selfPos.x - targetfPos.x > 0 ? 1 : -1, 1);
    }

    public Vector2 GetRigbodyVelocity()
    {
        return rigidbody2D.velocity;
    }
}

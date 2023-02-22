using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShootSystem;

[RequireComponent(typeof(FireController))]
public class PyxisFSM : FSM
{
    public enum Forward
    {
        Left,
        Right
    }


    public FireController FireCtrl;

    public Forward forward = Forward.Left;
    public Transform RightForward;
    protected BoxCollider2D _setHitBox;
    private List<GameObject> m_recycleAIPoints = new List<GameObject>();

    protected override void Start()
    {
        base.Start();
        _setHitBox = GetComponentInChildren<BoxCollider2D>();

        FireCtrl = GetComponent<FireController>();
        //若敌人fireControler未被单独配置则利用参数进行配置
        IFireControllerStateParameter ifc = parameter as IFireControllerStateParameter;
        if (ifc != null && FireCtrl.bulletSpeed == 0)
            FireCtrl.bulletSpeed = ifc.bulletSpeed;
        if (ifc != null && FireCtrl.maxDistance == 0)
            FireCtrl.maxDistance = ifc.bulletRange;

        if (!RightForward)
            RightForward = transform.Find("RightForward");
        if (forward == Forward.Right)
            FlipTo(RightForward);
    }
    protected override void InitParameter()
    {
        parameter = new PyxisStateParameter();
    }


    protected override void InitWayPointLogic()
    {

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


    protected override void InitAIState()
    {
        //添加状态
        states.Add(StateType.Idle, new State.PyxisIdleState(this));
        states.Add(StateType.React, new State.PyxisReactState(this));
        states.Add(StateType.Attack, new State.PyxisAttackState(this));
        states.Add(StateType.Hit, new State.PyxisHitState(this));
        states.Add(StateType.Death, new State.CustomDeathState(this));

        //选择初始状态
        TransitionState(StateType.Idle);
    }


    protected override void recycleAIPoints()
    {
        foreach (var point in m_recycleAIPoints)
        {
            if (point)
                Game.Instance.ObjectPool.RecycleObject(point);
        }
    }

    /// <summary>
    /// 动画事件回调
    /// </summary>
    /// <param name="str"></param>
    protected virtual void fire(string str)
    {
        if (parameter.target)
        {
            Vector2 direct = parameter.target.position - transform.position;
            var f = Vector2.Dot(forward == Forward.Right ? new Vector2(1,0): new Vector2(-1, 0), direct);
            if (f > 0.5)
            {
                FireCtrl.DoShoot(direct.normalized);
            }
        }
    }

}

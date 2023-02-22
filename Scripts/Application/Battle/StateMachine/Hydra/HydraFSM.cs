using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShootSystem;

[RequireComponent(typeof(Rifle))]
public class HydraFSM : FSM
{
    public Transform spAttackPoint;
    protected Rifle _rifle;
    protected override void InitParameter()
    {
        parameter = new HydraStateParameter();
    }

    protected override void Start()
    {
        base.Start();
        _rifle = GetComponent<Rifle>();

        //若敌人fireControler未被单独配置则利用参数进行配置
        IFireControllerStateParameter ifc = parameter as IFireControllerStateParameter;
        if (ifc != null && _rifle.bulletSpeed == 0)
            _rifle.bulletSpeed = ifc.bulletSpeed;
        if (ifc != null && _rifle.maxDistance == 0)
            _rifle.maxDistance = ifc.bulletRange;
    }

    protected override void InitAIState()
    {
        //添加状态
        states.Add(StateType.Idle, new State.CustomIdleState(this));
        states.Add(StateType.Patrol, new State.CustomPatrolState(this));
        states.Add(StateType.Chase, new State.CustomChaseState(this));
        states.Add(StateType.React, new State.CustomReactState(this));
        states.Add(StateType.Attack, new State.HydraAttackState(this));
        states.Add(StateType.Hit, new State.CustomHitState(this));
        states.Add(StateType.Death, new State.CustomDeathState(this));

        //选择初始状态
        TransitionState(StateType.Idle);
    }




    protected virtual void fire(string str)
    {
        Vector2 fireDir = parameter.target.position - transform.position;

        _rifle.DoShoot(fireDir.normalized);
    }
}

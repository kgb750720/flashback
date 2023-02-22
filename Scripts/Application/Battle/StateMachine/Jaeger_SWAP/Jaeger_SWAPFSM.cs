using ShootSystem;
using SkillSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rifle))]
public class Jaeger_SWAPFSM : FSM
{
    public Rifle RifleControl;
    
    protected override void InitParameter()
    {
        parameter = new Jaeger_SWAPStateParameter();

    }

    protected override void InitAIState()
    {
        //���״̬
        states.Add(StateType.Idle, new State.CustomIdleState(this));
        states.Add(StateType.Patrol, new State.CustomPatrolState(this));
        states.Add(StateType.Chase, new State.CustomChaseState(this));
        states.Add(StateType.React, new State.CustomReactState(this));
        states.Add(StateType.Attack, new State.CustomAttackState(this));
        states.Add(StateType.Hit, new State.CustomHitState(this));
        states.Add(StateType.Death, new State.CustomDeathState(this));

        //ѡ���ʼ״̬
        TransitionState(StateType.Idle);
    }


    protected override void Start()
    {
        base.Start();


        if (!RifleControl)
            RifleControl = GetComponent<Rifle>();
        //������fireControlerδ���������������ò�����������
        IFireControllerStateParameter ifc = parameter as Jaeger_SWAPStateParameter;
        if (RifleControl.bulletSpeed == 0)
            RifleControl.bulletSpeed = ifc.bulletSpeed;
        if (RifleControl.maxDistance == 0)
            RifleControl.maxDistance = ifc.bulletRange;
    }


    /// <summary>
    /// �����¼��ص�
    /// </summary>
    /// <param name="str"></param>
    protected virtual void fire(string str)
    {
        if(parameter.target)
            RifleControl.DoShoot(parameter.target.position-transform.position);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunishFSM : FSM
{
    protected override void InitParameter()
    {
        parameter = new PunishStateParameter();
    }

    protected override void InitAIState()
    {
        //���״̬
        states.Add(StateType.Idle, new State.CustomIdleState(this));
        states.Add(StateType.Patrol, new State.CustomPatrolState(this));
        states.Add(StateType.Chase, new State.PunishChaseState(this));
        states.Add(StateType.React, new State.CustomReactState(this));
        states.Add(StateType.Attack, new State.CustomAttackState(this));
        states.Add(StateType.Hit, new State.PunishHitState(this));
        states.Add(StateType.Death, new State.CustomDeathState(this));

        //ѡ���ʼ״̬
        TransitionState(StateType.Idle);
    }

    protected virtual void fire(string str)
    {
        msgCenter.BroadcastMsg<EnemyMeleeAttack.DoMeleeAttackMsg,Transform>(AttackPoint);
    }
}

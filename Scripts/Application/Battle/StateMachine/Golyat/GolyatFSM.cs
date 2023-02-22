using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GolyatFSM : FSM
{
    protected override void InitAIState()
    {
        //���״̬
        states.Add(StateType.Idle, new State.GolyatIdleState(this));
        states.Add(StateType.Patrol, new State.GolyatPatrolState(this));
        states.Add(StateType.Chase, new State.GolyatChaseState(this));
        states.Add(StateType.Hit, new State.GolyatHitState(this));
        states.Add(StateType.Attack, new State.GolyatAttackState(this));
        states.Add(StateType.React, new State.GolyatReactState(this));
        states.Add(StateType.Death, new State.GolyatDeathState(this));

        //ѡ���ʼ״̬
        TransitionState(StateType.Idle);
    }

    protected override void InitParameter()
    {
        parameter = new GolyatStateParameter();
    }

    protected virtual void fire(string str)
    {
        msgCenter.BroadcastMsg<EnemyMeleeAttack.DoMeleeAttackMsg, Transform>(AttackPoint);
    }

}

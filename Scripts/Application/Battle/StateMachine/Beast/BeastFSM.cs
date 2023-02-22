using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyMeleeAttack))]
public class BeastFSM : FSM
{
    protected override void InitParameter()
    {
        parameter = new BeastStateParameter();
    }

    protected override void InitAIState()
    {
        //Ìí¼Ó×´Ì¬
        states.Add(StateType.Idle, new State.CustomIdleState(this));
        states.Add(StateType.Patrol, new State.CustomPatrolState(this));
        states.Add(StateType.Chase, new State.CustomChaseState(this));
        states.Add(StateType.React, new State.CustomReactState(this));
        states.Add(StateType.Attack, new State.CustomAttackState(this));
        states.Add(StateType.Hit, new State.CustomHitState(this));
        states.Add(StateType.Death, new State.CustomDeathState(this));

        //Ñ¡Ôñ³õÊ¼×´Ì¬
        TransitionState(StateType.Idle);
    }



    protected virtual void fire(string str)
    {
        msgCenter.BroadcastMsg<EnemyMeleeAttack.DoMeleeAttackMsg,Transform>(AttackPoint);
    }
}

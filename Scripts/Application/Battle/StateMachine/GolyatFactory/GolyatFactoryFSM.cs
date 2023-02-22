using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolyatFactoryFSM : FSM
{
    protected override void InitParameter()
    {
        parameter = new GolyatFactoryStateParameter();
    }

    protected override void InitAIState()
    {
        states.Add(StateType.Idle, new State.GolyatFactoryIdleState(this));
        states.Add(StateType.Patrol, new State.GolyatFactoryPatrolState(this));
        states.Add(StateType.Chase, new State.GolyatFactoryChaseState(this));
        states.Add(StateType.React, new State.GolyatFactoryReactState(this));
        states.Add(StateType.Attack, new State.GolyatFactoryAttackState(this));
        states.Add(StateType.Hit, new State.GolyatFactoryHitState(this));
        states.Add(StateType.Death, new State.GolyatFactoryDeathState(this));

        //Ñ¡Ôñ³õÊ¼×´Ì¬
        TransitionState(StateType.Idle);
    }


}

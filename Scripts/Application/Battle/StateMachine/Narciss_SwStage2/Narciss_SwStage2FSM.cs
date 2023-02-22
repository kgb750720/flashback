using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NarcissSwordStage2))]
public class Narciss_SwStage2FSM : FSM
{
    Narciss_SwStage2StateParameter _parameter;
    protected override void InitParameter()
    {
        parameter = new Narciss_SwStage2StateParameter();
        _parameter = parameter as Narciss_SwStage2StateParameter;
    }
    protected override void InitAIState()
    {
        states.Add(StateType.Idle, new State.Narciss_SwStage2IdleState(this));
        states.Add(StateType.Hit, new State.Narciss_SwStage2HitState(this));

        TransitionState(StateType.Idle);
    }
}

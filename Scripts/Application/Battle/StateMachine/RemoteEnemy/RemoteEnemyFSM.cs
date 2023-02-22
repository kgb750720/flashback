using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteEnemyFSM : FSM
{
    protected override void InitParameter()
    {
        parameter = new RemoteEnemyStateParameter();
    }

    protected override void InitAIState()
    {
        base.InitAIState();
    }
}

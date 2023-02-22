using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier_NewFSM : Jaeger_SWAPFSM
{
    protected override void InitParameter()
    {
        parameter = new Soldier_NewStateParameter();
    }
}

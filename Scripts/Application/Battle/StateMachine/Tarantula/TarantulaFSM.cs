using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TarantulaFSM : DinergateFSM
{
    protected override void InitParameter()
    {
        parameter = new TarantulaStateParameter();
    }
}

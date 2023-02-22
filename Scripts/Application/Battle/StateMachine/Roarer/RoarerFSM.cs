using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoarerFSM : BeastFSM
{
    protected override void InitParameter()
    {
        parameter = new RoarerStateParameter();
    }

}

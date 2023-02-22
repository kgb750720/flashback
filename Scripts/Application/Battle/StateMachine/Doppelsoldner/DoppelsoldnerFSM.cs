using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoppelsoldnerFSM : HydraFSM
{
    protected override void InitParameter()
    {
        parameter = new DoppelsoldnerStateParameter();
    }
}

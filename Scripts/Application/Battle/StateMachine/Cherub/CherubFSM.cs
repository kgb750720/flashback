using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShootSystem;

[RequireComponent(typeof(Rifle))]
public class CherubFSM : CustomAerocraftFSM
{
    protected override void InitParameter()
    {
        parameter = new CherubStateParameter();
    }


}

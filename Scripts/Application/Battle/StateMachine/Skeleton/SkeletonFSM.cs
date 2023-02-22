using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SkeletonFSM : FSM
{
    protected override void InitParameter()
    {
        parameter = new SkeletonStateParameter();
    }

    protected override void Update()
    {
        base.Update();
        if (Input.GetKeyDown(KeyCode.Return))
        {
            parameter.getHit = true;
        }
    }
    
    private void OnDrawGizmos()
    {
        parameter.attackPoint = AttackPoint;
        Gizmos.DrawWireSphere(parameter.attackPoint.position, parameter.attackArea);
    }


}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolyatStateParameter : StateParameter
{
    
    public bool deadByBoom = false; //��ը���µ�����
    public GolyatStateParameter()
    {
        health = 30f;
        moveSpeed = 5.5f;
        chaseSpeed = 5.5f;
        idleTime = 0f;
        attackCfgCD = 1f;
        attackArea = 2f;
        spawnPatrolRadius = 15f;
        spawnChaseRadius = 15f;
        lostTargetTime = 8f;
        attackAnimaTime = 1.5f;
        fallResetTime = 8f;
    }
}

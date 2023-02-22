using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeastStateParameter : StateParameter
{
    public BeastStateParameter()
    {
        health = 100f;
        moveSpeed = 5f;
        chaseSpeed = 5f;
        idleTime = 1.5f;
        attackCfgCD = 1f;
        attackArea = 2f;
        spawnPatrolRadius = 7f;
        spawnChaseRadius = 10f;
        lostTargetTime = 2f;
        attackAnimaTime = 0.5f;
        fallResetTime = 8f;
    }
}

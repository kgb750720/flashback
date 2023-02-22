using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoarerStateParameter : BeastStateParameter
{
    public RoarerStateParameter()
    {
        health = 150f;
        moveSpeed = 4f;
        chaseSpeed = 4f;
        idleTime = 2f;
        attackCfgCD = 1f;
        attackArea = 1.5f;
        spawnPatrolRadius = 6f;
        spawnChaseRadius = 8f;
        lostTargetTime = 8f;
        attackAnimaTime = 1f;
        fallResetTime = 8f;
    }
}

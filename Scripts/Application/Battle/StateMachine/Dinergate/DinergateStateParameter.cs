using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DinergateStateParameter : StateParameter
{
    public DinergateStateParameter()
    {
        health = 30f;
        moveSpeed = 6.5f;
        chaseSpeed = 6.5f;
        idleTime = 0f;
        attackCfgCD = 0f;
        attackArea = 1.5f;
        spawnPatrolRadius = 10f;
        spawnChaseRadius = 10f;
        lostTargetTime = 0f;
        attackAnimaTime = 0.1f;
        fallResetTime = 8f;
    }
}
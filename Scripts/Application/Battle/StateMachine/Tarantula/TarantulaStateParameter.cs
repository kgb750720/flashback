using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TarantulaStateParameter : DinergateStateParameter
{
    public TarantulaStateParameter()
    {
        health = 50f;
        moveSpeed = 6f;
        chaseSpeed = 6f;
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

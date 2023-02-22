using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CherubStateParameter : ScoutsStateParameter
{
    public CherubStateParameter()
    {
        health = 80f;
        moveSpeed = 5f;
        chaseSpeed = 5f;
        idleTime = 2.5f;
        attackCfgCD = 1.25f;
        attackArea = 15f;
        spawnPatrolRadius = 5f;
        spawnChaseRadius = 5f;
        lostTargetTime = 8f;
        bulletRange = 15f;
        bulletSpeed = 7f;
        attackAnimaTime = 1f;
        fallResetTime = 8f;
    }
}

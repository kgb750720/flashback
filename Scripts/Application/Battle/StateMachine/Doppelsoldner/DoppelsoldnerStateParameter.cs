using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoppelsoldnerStateParameter : HydraStateParameter
{
    public DoppelsoldnerStateParameter()
    {
        health = 200f;
        moveSpeed = 4f;
        chaseSpeed = 4f;
        idleTime = 2.5f;
        attackCfgCD = 1.5f;
        attackArea = 5f;
        spawnPatrolRadius = 5f;
        spawnChaseRadius = 5f;
        lostTargetTime = 8f;
        bulletRange = 20f;
        bulletSpeed = 7f;
        attackAnimaTime = 0.75f;
        fallResetTime = 8f;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HydraStateParameter : StateParameter,IFireControllerStateParameter
{

    public float bulletRange { get; set; }
    public float bulletSpeed { get; set; }

    public HydraStateParameter()
    {
        health = 160f;
        moveSpeed = 4f;
        chaseSpeed = 4f;
        idleTime = 2.5f;
        attackCfgCD = 1.5f;
        attackArea = 5f;
        spawnPatrolRadius = 5f;
        spawnChaseRadius = 5f;
        lostTargetTime = 8f;
        bulletRange = 20f;
        bulletSpeed = 5f;
        attackAnimaTime = 0.75f;
        fallResetTime = 8f;
    }

}

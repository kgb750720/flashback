using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolyatFactoryStateParameter : StateParameter
{
    public GolyatFactoryStateParameter()
    {
        health = 350f;
        moveSpeed = 0.5f;
        chaseSpeed = 0.5f;
        idleTime = 1f;
        attackCfgCD = 2f;
        attackArea = 15f;
        spawnPatrolRadius = 0f;
        spawnChaseRadius = 0f;
        lostTargetTime = 8f;
        attackAnimaTime = 1f;
        fallResetTime = 8f;
    }
}

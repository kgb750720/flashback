using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunishStateParameter : StateParameter
{
    public PunishStateParameter()
    {
        health = 250f;
        moveSpeed = 4f;
        chaseSpeed = 4f;
        idleTime = 2.5f;
        attackCfgCD = 0.75f;
        attackArea = 2.5f;
        spawnPatrolRadius = 6f;
        spawnChaseRadius = 8f;
        lostTargetTime = 8f;
        attackAnimaTime = 1f;
        fallResetTime = 8f;
    }
}

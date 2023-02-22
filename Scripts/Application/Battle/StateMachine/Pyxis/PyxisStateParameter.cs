using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PyxisStateParameter : StateParameter,IFireControllerStateParameter
{

    float _bulletRange;
    float _bulletSpeed;
    public float bulletRange { get => _bulletRange; set => _bulletRange = value; }
    public float bulletSpeed { get => _bulletSpeed; set => _bulletSpeed = value; }
    public PyxisStateParameter()
    {
        health = 80f;
        moveSpeed = 0f;
        chaseSpeed = 0f;
        idleTime = 0f;
        attackCfgCD = 1f;
        attackArea = 15f;
        spawnPatrolRadius = 5f;
        spawnChaseRadius = 5f;
        lostTargetTime = 8f;
        bulletRange = 15f;
        bulletSpeed = 5f;
        attackAnimaTime = 1f;
        fallResetTime = 8f;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jaeger_SWAPStateParameter : StateParameter,IFireControllerStateParameter
{
    float _bulletRange;
    float _bulletSpeed;
    public float bulletRange { get => _bulletRange; set => _bulletRange = value; }
    public float bulletSpeed { get => _bulletSpeed; set => _bulletSpeed = value; }
    public Jaeger_SWAPStateParameter()
    {
        health = 60f;
        moveSpeed = 4f;
        chaseSpeed = 4f;
        idleTime = 2.5f;
        attackCfgCD = 2f;
        attackArea = 10f;
        spawnPatrolRadius = 5f;
        spawnChaseRadius = 5f;
        lostTargetTime = 5f;
        bulletRange = 20f;
        bulletSpeed = 7f;
        attackAnimaTime = 0.5f;
        fallResetTime = 8f;
    }

}

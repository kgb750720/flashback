using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoutsStateParameter : StateParameter,IFireControllerStateParameter
{

    float _bulletRange;
    float _bulletSpeed;
    public float bulletRange { get => _bulletRange; set => _bulletRange = value; }
    public float bulletSpeed { get => _bulletSpeed; set => _bulletSpeed = value; }
    public ScoutsStateParameter()
    {
        health = 50f;
        moveSpeed = 3.5f;
        chaseSpeed = 3.5f;
        idleTime = 2.5f;
        attackCfgCD = 1.75f;
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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeBody : MonoBehaviour
{
    private float timeBeforeAffected; //主角产生的物体过段时间才被影响暂停
    private Rigidbody2D rb;
    private Vector3 recordedVelocity;
    private float recordedMagnitude;

    private float timeBeforeAffectedTimer;
    private bool canBeAffected;
    private bool isStopped;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        timeBeforeAffected = Game.Instance.TimeManager.timeBeforeAffected;
        timeBeforeAffectedTimer = timeBeforeAffected;
    }

    private void OnEnable()
    {
        timeBeforeAffectedTimer = timeBeforeAffected;
    }
    
    void Update()
    {
        timeBeforeAffectedTimer -= Time.deltaTime; 
        if(timeBeforeAffectedTimer <= 0f)
        {
            canBeAffected = true; // 这之后能被时间暂停影响
        }

        if(canBeAffected && !isStopped)
        {
            if(rb.velocity.magnitude >= 0f) //如果物体在移动
            {
                recordedVelocity = rb.velocity.normalized; //记录速度方向
                recordedMagnitude = rb.velocity.magnitude; //记录速度大小

                rb.velocity = Vector3.zero; //让rigidbody停止
                rb.isKinematic = true; //不再施加力
                isStopped = true; // 防止循环
            }
        }
    }
    
    public void ContinueTime()
    {
        rb.isKinematic = false;
        isStopped = false;
        rb.velocity = recordedVelocity * recordedMagnitude; //加回之前的速度
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class NarcissSwordStage2 : NarcissShootSword
{
    public Transform target;
    public float AimMoveSpeed = 5f;
    private Transform m_AimTrans;
    [HideInInspector]
    public NarcissFSM FSM;

    protected override void AdditionalInit()
    {
        m_AimTrans = new GameObject("AimPoint").transform;
        m_AimTrans.position = transform.position;
    }

    private void FixedUpdate()
    {
        if (target)
        {
            m_AimTrans.position = Vector3.Lerp(m_AimTrans.position, target.position, AimMoveSpeed * Time.fixedDeltaTime * m_timeScale);
            //transform.LookAt(m_AimTrans);
            //transform.Rotate(new Vector3(0, 90));
            Vector2 dir = (m_AimTrans.position - transform.position).normalized;
            transform.Rotate(new Vector3(0, 0, 90 * Vector2.Dot(-transform.up, dir)));
        }
    }

    protected override void AdditionalDestroyProcess()
    {
        if (m_AimTrans)
            Destroy(m_AimTrans.gameObject);
        if (FSM)
            FSM.Stage2Swords.Remove(this);
    }
}

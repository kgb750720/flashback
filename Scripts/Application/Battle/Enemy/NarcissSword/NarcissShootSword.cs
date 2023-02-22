using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NarcissShootSword : MonoBehaviour
{
    public float ChargeScale = 5;

    public float FireRange = 10;

    public float LaserSpeed = 1f;

    public float SustainTime = 10f;

    public UnityAction FireCallback;

    public UnityAction LaserEndCallback;

    protected float m_timeScale = 1;

    protected Transform m_firePoint;

    protected LaserLauncher laser;

    protected float m_chargeTimer = -1;

    protected float m_chargeSeconds = -1;



    // Start is called before the first frame update
    void Start()
    {
        m_firePoint = transform.Find("FirePoint");
        laser = m_firePoint.GetComponentInChildren<LaserLauncher>();
        laser.Range = 0;

        EventCenter.Instance.AddEventListener<SlowTimeArgs>(Consts.E_BUFF_TimeSlow, timeSlowEvent);
        EventCenter.Instance.AddEventListener(Consts.E_BUFF_TimeNormal, timeNormalEvent);

        AdditionalInit();
    }

    private void timeNormalEvent()
    {
        m_timeScale = 1;
    }

    private void timeSlowEvent(SlowTimeArgs arg)
    {
        m_timeScale = arg.magnification;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_chargeTimer >= 0)
        {
            m_chargeTimer += Time.deltaTime * m_timeScale;
            if (m_chargeTimer < m_chargeSeconds)
            {
                float fireScale = Mathf.Clamp(ChargeScale * m_chargeTimer / m_chargeSeconds, 0, ChargeScale);
                m_firePoint.transform.localScale = new Vector3(fireScale, fireScale, 1);
            }
        }

        if (m_chargeTimer > 0 && m_chargeTimer >= m_chargeSeconds)
        {
            fire();
            FireCallback?.Invoke();
            m_chargeTimer = -1;
        }
    }

    IEnumerator m_fireRoutine;

    private void fire()
    {
        if (m_fireRoutine != null)
            StopCoroutine(m_fireRoutine);
        m_fireRoutine = fireHelper();
        StartCoroutine(m_fireRoutine);
    }
    public void LaserShoot(float chargeSeconds, UnityAction fireCallback=null,UnityAction laserEndCallback=null)
    {
        if (m_chargeTimer < 0)
        {
            resetLaser();
            m_chargeSeconds = chargeSeconds;
            m_chargeTimer = 0;
            FireCallback += fireCallback;
            LaserEndCallback += laserEndCallback;
        }
    }

    WaitForFixedUpdate m_wtffu = new WaitForFixedUpdate();

    float m_fireSustainCount = 0f;
    IEnumerator fireHelper()
    {
        while (m_fireSustainCount < SustainTime)
        {
            m_fireSustainCount += Time.fixedDeltaTime * m_timeScale;
            float rang= Mathf.Clamp(laser.Range + Time.fixedDeltaTime * m_timeScale * LaserSpeed, 0, FireRange); ;
            laser.Range = rang;
            yield return m_wtffu;
        }
        resetLaser();
        LaserEndCallback?.Invoke();
    }

    private void resetLaser()
    {
        m_fireSustainCount = 0;
        m_chargeTimer = -1;
        laser.Range = 0;
        m_firePoint.localScale = new Vector3(0, 0, 1);
        if (m_fireRoutine != null)
        {
            StopCoroutine(m_fireRoutine);
            m_fireRoutine = null;
        }
    }

    private void OnDestroy()
    {
        EventCenter.Instance.RemoveEventListener<SlowTimeArgs>(Consts.E_BUFF_TimeSlow, timeSlowEvent);
        EventCenter.Instance.RemoveEventListener(Consts.E_BUFF_TimeNormal, timeNormalEvent);
        AdditionalDestroyProcess();
    }

    protected virtual void AdditionalInit()
    {
        
    }

    protected virtual void AdditionalDestroyProcess()
    {

    }
}

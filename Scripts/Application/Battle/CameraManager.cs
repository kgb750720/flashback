using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;


public enum ShakeType
{
    CameraHurtNoise,
    CameraRifleAttackNoise,
    CameraRifleSkillNoise,
    CameraShotGunAttackNoise,
    CameraShotGunSkillNoise,
    CameraUltBoomNoise
}


/// <summary>
/// 相机管理模块，需要在进入场景时获得当前场景的相机
/// </summary>
public class CameraManager : SingletonMono<CameraManager>
{
    public List<NoiseSettings> NoiseProfiles;

    [HideInInspector]
    public GameObject playerCamera;
    [HideInInspector]
    public GameObject otherCamera;
    [HideInInspector]
    public GameObject PPCamera;
    [HideInInspector]
    public Camera mainCamera;
    [HideInInspector] 
    public GameObject postProcessing;
    [HideInInspector]
    public CinemachineVirtualCamera PlayerCinemachine;
    [HideInInspector]
    public CinemachineVirtualCamera OtherCinemachine;
    private CinemachineBasicMultiChannelPerlin PlayerConfig;
    private CinemachineBasicMultiChannelPerlin OtherConfig;
    private GameObject SceneFadeInAndOut;
    private RawImage SceneFade;
    [HideInInspector]
    public Volume volume;
    
    //屏幕后处理特效
    [HideInInspector]
    public ColorAdjustments ColorAdjustments;
    [HideInInspector] 
    public ChromaticAberration ChromaticAberration;
    private Vignette vignette;
    private bool isInDanger = false;
    private static float dagerTimer = 0f;
    private bool isGravity;
    private float gravityTimer;
    private float gravityTimerLeft;
    private float gravityAmplitude;
    private float gravityFrequency;
    private float timerFade;
    
    public float smooth = 0.1f;
    public Transform followTarget;
    //每次切换场景时调用
    private void Start()
    {
        playerCamera = GameObject.Find("PlayerCamera");
        otherCamera = GameObject.Find("OtherCamera");
        PPCamera = GameObject.Find("PPCamera");
        postProcessing = GameObject.Find("GlobalVolume");
        PlayerCinemachine = playerCamera.gameObject.GetComponent<CinemachineVirtualCamera>();
        OtherCinemachine = otherCamera.gameObject.GetComponent<CinemachineVirtualCamera>();
        PlayerConfig = PlayerCinemachine.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        OtherConfig = OtherCinemachine.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        mainCamera = playerCamera.GetComponent<Camera>();
        volume = postProcessing.GetComponent<Volume>();
        volume.profile.TryGet<ColorAdjustments>(out ColorAdjustments);
        volume.profile.TryGet<Vignette>(out vignette);
        volume.profile.TryGet<ChromaticAberration>(out ChromaticAberration);
        ColorAdjustments.active = false;
        vignette.active = false;
        ChromaticAberration.active = false;
        
        
        

        Object.DontDestroyOnLoad(playerCamera);
        Object.DontDestroyOnLoad(otherCamera);
        Object.DontDestroyOnLoad(postProcessing);
        Object.DontDestroyOnLoad(PPCamera);
    }

    private void Update()
    {
        PPUpdate();
        FollowUpdate();
    }

    #region 后处理

    private void PPUpdate()
    {
        if (isInDanger)
        {
            var num = (float) Math.Sin((Time.time - dagerTimer)*2) * 0.5f + 0.5f;
            vignette.smoothness.value = num;
        }

        if (isGravity)
        {
            if (gravityTimerLeft >= 0)
            {
                var num = Math.Cos((gravityTimerLeft - Time.deltaTime) / gravityTimer * gravityFrequency)*0.5f + 0.5f;
                ChromaticAberration.intensity.value = (float)num*gravityAmplitude;
                gravityTimerLeft -= Time.deltaTime;
            }
            else
            {
                isGravity = false;
                ChromaticAberration.intensity.value = 0;
                ChromaticAberration.active = false;
            }
        }
    }

    public void Endangered(bool isDanger)
    {
        if (isDanger)
        {
            isInDanger = isDanger;
            dagerTimer = Time.time;
            vignette.active = true;
        }
        else
        {
            isInDanger = isDanger;
            ClampedFloatParameter smoothnessParam = new ClampedFloatParameter(0f, 0f, 1f);
            vignette.smoothness.SetValue(smoothnessParam);
            vignette.active = false;
        }
    }

    public void GravityBullet(float time, float amplitude, float frequency)
    {
        if (time > 0)
        {
            isGravity = true;
            gravityTimer = time;
            gravityTimerLeft = gravityTimer;
            gravityAmplitude = amplitude;
            gravityFrequency = frequency;
        }
    }
    
    

    #endregion
    
    #region 淡入淡出
    public IEnumerator FadeIn(float time, Color color)
    {
        if (SceneFadeInAndOut == null)
        {
            SceneFadeInAndOut = Game.Instance.UIManager.CreateFadeUI();
            if(SceneFadeInAndOut == null) Debug.LogError("SceneFadeInAndOut is Null!!!");
            SceneFade = SceneFadeInAndOut.transform.Find("RawImage").GetComponent<RawImage>();;
            SceneFade.color = new Color(color.r, color.g, color.b, 0);
        }

        SceneFadeInAndOut.SetActive(true);

        float timer = 0f;
        while (timer < time)
        {
            SceneFade.color = Color.Lerp(SceneFade.color, color, timer / (time));
            
            timer += 0.01f;
            yield return new WaitForSeconds(0.01f);
        }
        
        SceneFade.color = color;
    }
    
    public IEnumerator FadeOut(float time, Color color)
    {
        if (SceneFadeInAndOut == null)
        {
            SceneFadeInAndOut = Game.Instance.UIManager.CreateFadeUI();
            if(SceneFadeInAndOut == null) Debug.LogError("SceneFadeInAndOut is Null!!!");
            SceneFade = SceneFadeInAndOut.GetComponent<RawImage>();
            SceneFade.color = new Color(color.r, color.g, color.b, 255);
        }

        float timer = 0f;
        color = new Color(color.r, color.g, color.b, 0);

        while (timer < time)
        {
            SceneFade.color = Color.Lerp(SceneFade.color, color, timer / (time));
            
            timer += 0.01f;
            yield return new WaitForSeconds(0.01f);
        }
        SceneFade.color = Color.clear;
        SceneFadeInAndOut.SetActive(false);
    }
    #endregion

    #region Cinemachine震动

    public void DoShake(ShakeType type, float shakeTime, float amplitude = 1, float frequency = 1)
    {
        StopCoroutine(DoShakeTimer(PlayerConfig, OtherConfig, shakeTime, amplitude, frequency));

        PlayerConfig.m_AmplitudeGain = amplitude;
        OtherConfig.m_AmplitudeGain = amplitude;
        PlayerConfig.m_FrequencyGain = frequency;
        OtherConfig.m_FrequencyGain = frequency;
        PlayerConfig.m_NoiseProfile = NoiseProfiles[(int)type];
        OtherConfig.m_NoiseProfile = NoiseProfiles[(int)type];

        StartCoroutine(DoShakeTimer(PlayerConfig,OtherConfig, shakeTime, amplitude, frequency));
    }

    IEnumerator DoShakeTimer(CinemachineBasicMultiChannelPerlin shakeConfig1, CinemachineBasicMultiChannelPerlin shakeConfig2,
        float shakeTime, float amplitude, float frequency)
    {
        float deltaTime = 0.1f;  //振幅和频率的设置时间单位
        float currentAmplitude = amplitude; //当前帧振幅
        int frame = (int) (shakeTime / deltaTime); //执行总帧数
        float amplitudeFrame = amplitude / frame; //振幅每帧变化值
        while (frame > 0)
        {
            yield return new WaitForSecondsRealtime(deltaTime);
            //随时间振幅缓慢消减
            if (currentAmplitude > 0)
                currentAmplitude -= amplitudeFrame;
            else
                currentAmplitude = 0;
            shakeConfig1.m_AmplitudeGain = currentAmplitude;
            shakeConfig2.m_AmplitudeGain = currentAmplitude;
            frame -= 1;
        }

        shakeConfig1.m_NoiseProfile = null;
        shakeConfig2.m_NoiseProfile = null;
    }
    #endregion
    
    #region 原始震动代码
    /// <summary>
    /// 屏幕抖动
    /// </summary>
    /// <param name="range"></param>
    /// <param name="time"></param>
    /// <param name="axis"> 0 两个轴都晃动
    ///                     1 x轴晃动 Horizontal 
    ///                     2 y轴晃动</param>
    public void DoShake(Transform trans, float range, float time, int axis = 0)
    {
        switch (axis)
        {
            case 0:
                StartCoroutine(Shake(trans,range, time));
                break;
            case 1:
                StartCoroutine(ShakeHorizontal(trans,range, time));
                break;
            case 2:
                StartCoroutine(Shakevertical(trans,range, time));
                break;
        }
    }

    public IEnumerator Shake(Transform trans, float range, float time)
    {
        Debug.Log("shaking!");
        
        Vector3 prePos = trans.position; //记录摄像机抖动前位置

        while (time >= 0)
        {
            time -= Time.deltaTime;

            if (time <= 0)
            {
                break;
            }

            Vector3 pos = trans.position;
            pos.x += Random.Range(-range, range);
            pos.y += Random.Range(-range, range);
            trans.position = pos;

            yield return null;
        }

        trans.position = prePos;
    }
    
    public IEnumerator ShakeHorizontal(Transform trans, float range, float time)
    {
        Vector3 prePos = trans.position; //记录摄像机抖动前位置

        while (time >= 0)
        {
            time -= Time.deltaTime;

            if (time <= 0)
            {
                break;
            }

            Vector3 pos = trans.position;
            pos.x += Random.Range(-range, range);
            trans.position = pos;

            yield return null;
        }

        trans.position = prePos;
    }
    
    public IEnumerator Shakevertical(Transform trans, float range, float time)
    {
        Vector3 prePos = trans.position; //记录摄像机抖动前位置

        while (time >= 0)
        {
            time -= Time.deltaTime;

            if (time <= 0)
            {
                break;
            }

            Vector3 pos = trans.position;
            pos.y += Random.Range(-range, range);
            trans.position = pos;

            yield return null;
        }

        trans.position = prePos;
    }
    #endregion

    #region 相机跟随

    private void FollowUpdate()
    {
        if (followTarget != null)
        {
            //Vector2 cameraFollow = Vector2.Lerp(playerCamera.transform.position, followTarget.transform.position, smooth);
            otherCamera.transform.position = followTarget.transform.position;
            playerCamera.transform.position = followTarget.transform.position;
        }
    }

    #endregion
}
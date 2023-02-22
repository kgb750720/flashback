using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;


public enum SlowType
{
    TimeScaleRifleBullet,
    TimeScaleShotGunBullet,
    TimeScaleArtillery,
    TimeScaleGravity,
    TimeScaleHurt
}

[Serializable]
public struct BulletTimeParam
{
    public SlowType type;
    public float inTime;
    public float keepTime;
    public float outTime;
    public float timeSlowScale;
    public bool isChromaticAberration;
    public float strength;
}

/// <summary>
/// 时间管理器
/// 优先级：游戏暂停(timescale) > 角色时停(事件分发) > 子弹时间(timescale)
/// </summary>
public class TimeManager : SingletonMono<TimeManager>
{
    public List<BulletTimeParam> BulletTimeParams;
    
    [HideInInspector]
    public float timeBeforeAffected = 0.5f;

    [HideInInspector]
    public bool timeIsSlowedInGame;

    private float timeSlowScaleBullet;
    
    private float timeScaleBeforePause = 1f;
    private float defaultFixedDeltaTime;
    private float timer;
    [HideInInspector]
    public bool isPause = false;

    private bool chromaticAberration = false;
    private float effectStrength = 1f;
    private void Start()
    {
        defaultFixedDeltaTime = Time.fixedDeltaTime;
    }

    public void Reset()
    {
        timeIsSlowedInGame = false;
        Game.Instance.CameraManager.ColorAdjustments.active = false;
        Game.Instance.CameraManager.ChromaticAberration.active = false;
        Game.Instance.MusicManager.SetMusicSpeed(1f);
        Game.Instance.MusicManager.isCutoff(false);
    }

    public void ResetBulletTime()
    {
        StopAllCoroutines();

        Time.timeScale = 1f;
        Time.fixedDeltaTime = defaultFixedDeltaTime * Time.timeScale;   
        Game.Instance.CameraManager.ChromaticAberration.active = false;
    }
    
    
    //角色结束缓慢时间
    public void ContinueTime()
    {
        if (!isPause)
        {
            timeIsSlowedInGame = false;
            
            Debug.Log("Continue Time");
            
            Game.Instance.CameraManager.ColorAdjustments.active = false;
            Game.Instance.MusicManager.SetMusicSpeed(1f);
            Game.Instance.MusicManager.isCutoff(false);
            Game.Instance.EventCenter.EventTrigger(Consts.E_BUFF_TimeNormal);
            /*var objects = FindObjectsOfType<TimeBody>();  //找到所有挂在TimeBody的物体
            for (var i = 0; i < objects.Length; i++)
            {
                objects[i].GetComponent<TimeBody>().ContinueTime(); //继续每一个角色的时间
            }*/
        }
    }
    
    //角色时缓时间
    public void SlowTime(float magnification, Transform father = null)
    {
        if (!isPause)
        {
            timeIsSlowedInGame = true;

            ResetBulletTime();
            
            Game.Instance.CameraManager.ColorAdjustments.active = true;
            Game.Instance.MusicManager.SetMusicSpeed(0.7f);
            Game.Instance.MusicManager.isCutoff(true);

            SlowTimeArgs e = new SlowTimeArgs();
            e.magnification = magnification;
            Game.Instance.EventCenter.EventTrigger(Consts.E_BUFF_TimeSlow, e);
        }
    }

    //游戏暂停
    public void Pause()
    {
        if (isPause) return;
        timeScaleBeforePause = Time.timeScale;
        Time.timeScale = 0f;
        isPause = true;
    }

    //游戏继续
    public void UnPause()
    {
        if (!isPause) return;
        Time.timeScale = timeScaleBeforePause;
        isPause = false;
    }
    
    //子弹时间
    public void SetBulletTime(SlowType type)
    {
        if (!isPause && !timeIsSlowedInGame)
        {
            if (BulletTimeParams[(int) type].type != type)
            {
                Debug.LogError("配置文件不正确！");
                return;
            }
            else
            {
                BulletTime(BulletTimeParams[(int) type].inTime, BulletTimeParams[(int) type].keepTime, 
                    BulletTimeParams[(int) type].outTime, BulletTimeParams[(int) type].timeSlowScale, BulletTimeParams[(int) type].isChromaticAberration,
                    BulletTimeParams[(int) type].strength);
            }
        }
    }
    
    //子弹时间
    public void BulletTime(float duration, float scale)
    {
        if (!isPause && !timeIsSlowedInGame)
        {
            timeSlowScaleBullet = scale;
            StartCoroutine(SlowOutCoroutine(duration));
        }
    }
    
    //子弹时间
    public void BulletTime(float inDuration, float outDuration, float scale)
    {
        if (!isPause && !timeIsSlowedInGame)
        {
            timeSlowScaleBullet = scale;
            StartCoroutine(SlowInAndOutCoroutine(inDuration, outDuration));
        }
    }
    
    //子弹时间
    public void BulletTime(float inDuration, float keepingDuration, float outDuration, float scale, bool isChromaticAberration, float strenth)
    {
        if (!isPause && !timeIsSlowedInGame)
        {
            timeSlowScaleBullet = scale;
            chromaticAberration = isChromaticAberration;
            effectStrength = strenth;
            StartCoroutine(SlowInKeepAndOutCoroutine(inDuration, keepingDuration, outDuration));
        }
    }
    
    public void SlowIn(float duration)
    {
        if(!timeIsSlowedInGame) StartCoroutine(SlowInCoroutine(duration));
    }
    
    public void SlowOut(float duration)
    {
        if(!timeIsSlowedInGame) StartCoroutine(SlowOutCoroutine(duration));
    }
    
    IEnumerator SlowInKeepAndOutCoroutine(float inDuration, float keepingDuration, float outDuration)
    {
        yield return StartCoroutine(SlowInCoroutine(inDuration));
        yield return new WaitForSecondsRealtime(keepingDuration);

        if (chromaticAberration) Game.Instance.CameraManager.ChromaticAberration.intensity.value = effectStrength;

        StartCoroutine(SlowOutCoroutine(outDuration));
    }

    IEnumerator SlowInAndOutCoroutine(float inDuration, float outDuration)
    {
        yield return StartCoroutine(SlowInCoroutine(inDuration));
        StartCoroutine(SlowOutCoroutine(outDuration));
    }
    
    IEnumerator SlowInCoroutine(float duration)
    {
        Game.Instance.CameraManager.ChromaticAberration.active = true;
        timer = 0f;
        while (timer < 1f)
        {
            if (!isPause)
            {
                if(chromaticAberration) Game.Instance.CameraManager.ChromaticAberration.intensity.value = timer*effectStrength;
                timer += Time.unscaledTime / duration;
                Time.timeScale = Mathf.Lerp(1f, timeSlowScaleBullet, timer);
                Time.fixedDeltaTime = defaultFixedDeltaTime * Time.timeScale;   
            }
            if(timeIsSlowedInGame) break;
        }
        if(chromaticAberration) Game.Instance.CameraManager.ChromaticAberration.intensity.value = effectStrength;
        if (timeIsSlowedInGame)
        {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = defaultFixedDeltaTime * Time.timeScale;   
        }

        yield return null;
    }
    
    IEnumerator SlowOutCoroutine(float duration)
    {
        timer = 0f;
        while (timer < 1f)
        {
            if (!isPause)
            {
                if(chromaticAberration) Game.Instance.CameraManager.ChromaticAberration.intensity.value = ((1-timer) >= 0? (1-timer) : 0)*effectStrength;
                timer += Time.unscaledTime / duration;
                Time.timeScale = Mathf.Lerp(timeSlowScaleBullet, 1f, timer);
                Time.fixedDeltaTime = defaultFixedDeltaTime * Time.timeScale;   
            }
            if(timeIsSlowedInGame) break;
        }

        if (chromaticAberration)
        {
            Game.Instance.CameraManager.ChromaticAberration.intensity.value = 0f;
            Game.Instance.CameraManager.ChromaticAberration.active = false;
        }
        
        if (timeIsSlowedInGame)
        {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = defaultFixedDeltaTime * Time.timeScale;   
        }
        
        yield return null;
    }
}

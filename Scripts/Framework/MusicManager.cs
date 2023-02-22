using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;


public class MusicManager : SingletonMono<MusicManager>
{
    public float BGM = 1;
    
    public float RifleShot = 1;
    public float RifleHit = 1;
    public float EnergyCannon = 1;
    public float ShotgunShot = 1;
    public float ShotgunHit = 1;
    public float Gravity = 1;
    public float GravityTime = 1;
    public float HitWall = 1;
    public float Jump = 1;
    public float Dash = 1;
    public float FootStep = 1;
    public float Flash = 1;
    public float OnHit = 1;
    public float Flesh = 1;
    public float Debris = 1;
    public float Boom = 1;
    public float Hot = 1;
    public float SlowTime = 1;
    public float StartSpeed = 1;
    public float Shield = 1;
    public float HpRecover = 1;
    public float StartBoom = 1;
    public float StartHot = 1;
    public float StartSlowTime = 1;
    public float StartShield = 1;
    public float SE_Gunfight = 1;
    public float SE_explode = 1;
    public float SE_gunshot_room = 1;
    public float SE_alarm = 1;
    public float SE_alarm1 = 1;
    public float SE_ElecNoise_Soft = 1;
    public float SE_ElecNoise_Hard = 1;
    public float SE_Doorkick = 1;
    public float SE_Hit = 1;
    
    //背景音乐组件
    private AudioSource bgSound = null;

    //背景音乐声音大小
    private float bgValue = 1;

    private Coroutine coroutine;
    
    //音效依附对象
    private GameObject soundPool = null;
    //音效列表
    private List<AudioSource> soundList = new List<AudioSource>();
    
    //音效大小
    private float soundValue = 1;

    private Dictionary<string, float> volumeSetting = new Dictionary<string, float>();

    private void Start()
    {
        volumeSetting.Add("RifleShot", RifleShot);
        volumeSetting.Add("RifleHit", RifleHit);
        volumeSetting.Add("EnergyCannon", EnergyCannon);
        volumeSetting.Add("ShotgunShot", ShotgunShot);
        volumeSetting.Add("ShotgunHit", ShotgunHit);
        volumeSetting.Add("Gravity", Gravity);
        volumeSetting.Add("GravityTime", GravityTime);
        volumeSetting.Add("HitWall", HitWall);
        volumeSetting.Add("Jump", Jump);
        volumeSetting.Add("Dash", Dash);
        volumeSetting.Add("FootStep", FootStep);
        volumeSetting.Add("Flash", Flash);
        volumeSetting.Add("OnHit", OnHit);
        volumeSetting.Add("Flesh", Flesh);
        volumeSetting.Add("Debris", Debris);
        volumeSetting.Add("Boom", Boom);
        volumeSetting.Add("Hot", Hot);
        volumeSetting.Add("SlowTime", SlowTime);
        volumeSetting.Add("StartSpeed", StartSpeed);
        volumeSetting.Add("Shield", Shield);
        volumeSetting.Add("HpRecover", HpRecover);
        volumeSetting.Add("StartBoom", StartBoom);
        volumeSetting.Add("StartHot", StartHot);
        volumeSetting.Add("StartSlowTime", StartSlowTime);
        volumeSetting.Add("StartShield", StartShield);
        volumeSetting.Add("SE_Gunfight", SE_Gunfight);
        volumeSetting.Add("SE_explode", SE_explode);
        volumeSetting.Add("SE_gunshot_room", SE_gunshot_room);
        volumeSetting.Add("SE_alarm", SE_alarm);
        volumeSetting.Add("SE_alarm1", SE_alarm1);
        volumeSetting.Add("SE_ElecNoise_Soft", SE_ElecNoise_Soft);
        volumeSetting.Add("SE_ElecNoise_Hard", SE_ElecNoise_Hard);
        volumeSetting.Add("SE_Doorkick", SE_Doorkick);
        volumeSetting.Add("SE_Hit", SE_Hit);
    }
    
    private void Update()
    {
        for( int i = soundList.Count - 1; i >=0; --i )
        {
            if (soundList[i] == null)
            {
                soundList.RemoveAt(i);
                continue;
            }
            if(!soundList[i].isPlaying)
            {
                if(soundList[i] != null) GameObject.Destroy(soundList[i]);
                soundList.RemoveAt(i);
            }
        }
    }

    public void SetMusicSpeed(float speed)
    {
        for( int i = soundList.Count - 1; i >=0; --i )
        {
            if(soundList[i].isPlaying)
            {
                soundList[i].pitch = speed;
            }
        }

        bgSound.pitch = speed;
    }

    public void isCutoff(bool active)
    {
        Game.Instance.PlayerManager.audioLowPassFilter.enabled = active;
    }

    /// <summary>
    /// 播放背景音乐
    /// </summary>
    /// <param name="name"></param>
    public void PlayBgMusic(string name)
    {
        if (name == null) return;
        if (name == "") return;
        if(bgSound == null)
        {
            //GameObject obj = new GameObject();
            //obj.name = "BgMusic";
            bgSound = this.gameObject.AddComponent<AudioSource>();
        }
        //异步加载背景音乐 加载完成后 播放
        Game.Instance.AssetsManager.LoadAsync<AudioClip>("Music/Bg/" + name, (clip) =>
        {
            if(coroutine != null) StopCoroutine(coroutine);
            bgSound.spatialBlend = 0f;
            bgSound.clip = clip;
            bgSound.loop = true;
            bgSound.volume = bgValue*BGM;
            coroutine = StartCoroutine(PlayBgMusicCoroutine());
        });
    }

    private IEnumerator PlayBgMusicCoroutine()
    {
        bgSound.volume = 0f;
        bgSound.Play();

        while (bgSound.volume < bgValue*BGM)
        {
            bgSound.volume += 0.01f;
            yield return new WaitForSecondsRealtime(0.01f);
        }

        bgSound.volume = bgValue*BGM;

    }
    
    /// <summary>
    /// 暂停背景音乐
    /// </summary>
    public void PauseBgMusic()
    {
        if(coroutine != null) StopCoroutine(coroutine);
        if (bgSound == null)
            return;
        coroutine = StartCoroutine(PauseBgMusicCoroutine());
    }

    private IEnumerator PauseBgMusicCoroutine()
    {
        while (bgSound.volume > 0)
        {
            bgSound.volume -= 0.01f;
            yield return new WaitForSecondsRealtime(0.01f);
        }
        bgSound.Pause();
        bgSound.volume = bgValue*BGM;
    }

    /// <summary>
    /// 停止背景音乐
    /// </summary>
    public void StopBgMusic()
    {
        if(coroutine != null) StopCoroutine(coroutine);
        if (bgSound == null)
            return;
        coroutine = StartCoroutine(StopBgMusicCoroutine());
    }
    
    private IEnumerator StopBgMusicCoroutine()
    {
        while (bgSound.volume > 0)
        {
            bgSound.volume -= 0.01f;
            yield return new WaitForSecondsRealtime(0.01f);
        }
        bgSound.Stop();
        bgSound.volume = bgValue*BGM;
    }

    /// <summary>
    /// 改变背景音乐 音量大小
    /// </summary>
    /// <param name="v"></param>
    public void ChangeBgValue(float v)
    {
        bgValue = v;
        if (bgSound == null)
            return;
        bgSound.volume = bgValue*BGM;
    }

    /// <summary>
    /// 播放音效
    /// </summary>
    public void PlaySound(string name,GameObject soundObj, bool isLoop, float isSurround = 1f, UnityAction<AudioSource> callBack = null)
    {
        if (name == String.Empty || name == "") return;

        soundObj = null;
        if(soundObj == null)
        {
            if (soundPool == null)
            {
                soundPool = new GameObject("SoundPool");
            }

            soundObj = soundPool;
        }
        
        //当音效资源异步加载结束后 再添加一个音效
        Game.Instance.AssetsManager.LoadAsync<AudioClip>("Music/Sound/" + name, (clip) =>
        {
            AudioSource source = soundObj.AddComponent<AudioSource>();
            if (soundObj.name != "SoundPool")
                source.spatialBlend = isSurround;
            source.rolloffMode = AudioRolloffMode.Logarithmic;
            source.clip = clip;
            source.loop = isLoop;
            source.volume = soundValue * volumeSetting[name];
            source.Play();
            soundList.Add(source);
            if(callBack != null)
                callBack(source);
        });
    }

    /// <summary>
    /// 改变音效声音大小
    /// </summary>
    /// <param name="value"></param>
    public void ChangeSoundValue( float value )
    {
        Debug.Log("SoundValue" + value);
        soundValue = value;
        for (int i = 0; i < soundList.Count; ++i)
            soundList[i].volume = value;
    }

    /// <summary>
    /// 停止音效
    /// </summary>
    public void StopSound(AudioSource source)
    {
        if( soundList.Contains(source) )
        {
            soundList.Remove(source);
            source.Stop();
            GameObject.Destroy(source);
        }
    }
}
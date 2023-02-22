using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class ScenesManager : SingletonMono<ScenesManager>
{
    public string currentScene = "0.Init";
    
    //加载场景（同步）
    [Obsolete("同步方法已不推荐使用，请使用LoadSceneAsyn方法异步加载场景")]
    public void LoadScene(string name, UnityAction action)
    {
        //---退出旧场景
        //事件参数
        SceneArgs e = new SceneArgs();
        e.sceneName = SceneManager.GetActiveScene().name;

        //发布事件
        Game.Instance.EventCenter.EventTrigger(Consts.E_ExitScene, e);

        //---加载新场景
        SceneManager.LoadScene(name, LoadSceneMode.Single);
        
        //进入新场景清空事件中心事件和对象池
        //Game.Instance.EventCenter.Clear();
        //Game.Instance.ObjectPool.Clear();

        //加载完成后才会继续执行
        action();
    }    
    
    [Obsolete("同步方法已不推荐使用，请使用LoadSceneAsyn方法异步加载场景")]
    public void LoadScene(int scene, UnityAction action)
    {
        //显示场景加载画面
        UILoadingArgs loadingArgs = new UILoadingArgs();
        Game.Instance.UIManager.ShowPanel<UILoading>(Consts.UILoading, loadingArgs, EUILayer.Top);

        //---退出旧场景
        //事件参数
        SceneArgs e = new SceneArgs();
        e.sceneName = SceneManager.GetActiveScene().name;

        //发布事件
        Game.Instance.EventCenter.EventTrigger(Consts.E_ExitScene, e);

        //---加载新场景
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
        
        //进入新场景清空事件中心事件和对象池
        //Game.Instance.EventCenter.Clear();
        //Game.Instance.ObjectPool.Clear();
        
        Game.Instance.UIManager.HidePanel(Consts.UILoading);
        action();
    }
    
    /// <summary>
    /// 加载场景，异步
    /// </summary>
    /// <param name="name"></param>
    /// <param name="action"></param>
    public void LoadSceneAsyn(string name, UnityAction action)
    {
        StartCoroutine(ILoadSceneAsyn(name, action));
    }

    private IEnumerator ILoadSceneAsyn(string name, UnityAction action, bool showDialog = false)
    {
        Game.Instance.MusicManager.StopBgMusic();

        currentScene = name;
        
        //显示场景加载画面
        int displayProgress = 0;
        int toProgress = 0;
        UILoadingArgs loadingArgs = new UILoadingArgs();
        Game.Instance.UIManager.ShowPanel<UILoading>(Consts.UILoading, loadingArgs, EUILayer.Top);
        
        //进入新场景清空事件中心事件和对象池
        //Game.Instance.EventCenter.Clear();
        //Game.Instance.ObjectPool.Clear();
        
        AsyncOperation ao = SceneManager.LoadSceneAsync(name);
        ao.allowSceneActivation = false;
        
        //可以得到场景加载的进度
        while (ao.progress < 0.9f)
        {
            toProgress = (int)ao.progress * 100;
            while (displayProgress < toProgress)
            {
                ++displayProgress;
                yield return new WaitForEndOfFrame();//ui渲染完成之后
            }
        }
        toProgress = 100;
        
        while (displayProgress < toProgress)
        {
            ++displayProgress;
            yield return new WaitForEndOfFrame();
        }
        
        ao.allowSceneActivation = true;
        
        // 等两帧，直到新场景Awake执行完毕
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        
        action();

        Debug.Log("LoadScene:" + name);
        if (Game.Instance.StaticData.sceneDialogHash.ContainsKey(name))
        {
            int dialogIndex = Int32.Parse(Game.Instance.StaticData.sceneDialogHash[name].Substring(Game.Instance.StaticData.sceneDialogHash[name].Length - 1, 1));
            if(dialogIndex > Game.Instance.SaveSystem.currentSave.dialog)
                showDialog = true;
        }
        if (showDialog)
        {
            StartCoroutine(Game.Instance.UIManager.DialogStartShow(Game.Instance.StaticData.sceneDialogHash[name]));
        }
        else
        {
            if(Game.Instance.StaticData.sceneMusicHash.ContainsKey(name))
                Game.Instance.MusicManager.PlayBgMusic(Game.Instance.StaticData.sceneMusicHash[name]);
        }
        
        yield return new WaitForSecondsRealtime(0.5f);

        Game.Instance.UIManager.HidePanel(Consts.UILoading);

        SceneArgs e = new SceneArgs();
        e.sceneName = name;
        Game.Instance.EventCenter.EventTrigger(Consts.E_EnterScene, e);
    }
}

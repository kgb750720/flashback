using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum EUILayer
{
    Dynamic,
    Battle,
    ForeScreen,
    Dialog,
    Back,
    Mid,
    Top
}

public class UIManager : SingletonMono<UIManager>
{
    public Dictionary<string, BasePanel> panelDic = new Dictionary<string, BasePanel>();
    
    public Dictionary<int, BasePanel> dynamicDic = new Dictionary<int, BasePanel>();
    
    public Stack<BasePanel> panelStack = new Stack<BasePanel>();

    public Image gameMask;

    public EUILayer currentLayer = EUILayer.Back;
    
    private RectTransform canvas;

    public bool isDialogEnd = false;
    
    private Transform battle;
    private Transform dialog;
    private Transform back;
    private Transform mid;
    private Transform top;
    private Transform dynamic;
    private Transform foreScreen;

    public GameObject vidoePlayer;

    private GameObject SceneFadeInAndOut = null;

    public void Start()
    {
        GameObject can = GameObject.Find("Canvas");
        canvas = can.transform as RectTransform;

        //找到UI各层
        battle = canvas.Find("Battle");
        dialog = canvas.Find("Dialog");
        back = canvas.Find("Back");
        mid = canvas.Find("Mid");
        top = canvas.Find("Top");
        dynamic = canvas.Find("Dynamic");
        foreScreen = canvas.Find("ForeScreen");
        vidoePlayer = mid.transform.Find("VidoePlayer").gameObject;
        gameMask = back.GetComponent<Image>();
        gameMask.enabled = false;
        
        DontDestroyOnLoad(can);

        GameObject ES = GameObject.Find("EventSystem");
        DontDestroyOnLoad(ES);
        
        foreScreen.gameObject.SetActive(false);
    }
    
    /// <summary>
    /// 显示面板
    /// </summary>
    /// <typeparam name="T">面板脚本类型</typeparam>
    /// <param name="panelName">面板名</param>
    /// <param name="layer">显示在哪一层</param>
    /// <param name="callBack">当面板预设体创建成功后 你想做的事</param>
    public void ShowPanel<T>(string panelName,UIArgs args =null, EUILayer layer=EUILayer.Back, BasePanel callUI = null, 
        bool isPause = false, UnityAction<T> callBack=null) where T : BasePanel
    {
      
        if(panelName != Consts.StartMenu && panelName != Consts.UILoading && panelName != Consts.UIBattle
           && panelName != Consts.UIPhoneBattle && panelName != Consts.DialogueSystemPanel)
        {
            gameMask.enabled = true;
        }
        else if(panelName == Consts.DialogueSystemPanel)
        {
            currentLayer = EUILayer.Dialog;
        }
        
        if(isPause) Game.Instance.TimeManager.Pause();

        if (panelDic.ContainsKey(panelName))
        {
            panelDic[panelName].ShowPanel(args);
            // 处理面板创建完成后的逻辑
            if (callBack != null)
                callBack(panelDic[panelName] as T);
            if (callUI != null)
            {
                if(callUI.name != Consts.DialogueSystemPanel) callUI.HidePanel();
                panelStack.Push(callUI);
            }
            //避免面板重复加载 如果存在该面板 即直接显示 调用回调函数后  直接return 不再处理后面的异步加载逻辑
            return;
        }
        
        Game.Instance.AssetsManager.LoadAsync<GameObject>(Consts.UIDir + panelName, (obj) =>
        {
            //把新的面板作为Canvas的子对象进行管理
            //并设置相对位置
            Transform father = back;
            switch (layer)
            {
                case EUILayer.Dynamic:
                    father = dynamic;
                    break;
                case EUILayer.Back:
                    father = back;
                    break;
                case EUILayer.Dialog:
                    father = dialog;
                    break;
                case EUILayer.Mid:
                    father = mid;
                    break;
                case EUILayer.Top:
                    father = top;
                    break;
                case EUILayer.Battle:
                    father = battle;
                    break;
            }
            //设置父对象  设置相对位置和大小
            obj.name = panelName;
            obj.transform.SetParent(father);

            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one;
            
            (obj.transform as RectTransform).offsetMax = Vector2.zero;
            (obj.transform as RectTransform).offsetMin = Vector2.zero;
            
            //得到预设体身上的面板脚本
            T panel = obj.GetComponent<T>();
            
            // 处理面板创建完成后的逻辑
            if (callBack != null)
                callBack(panel);
            
            panel.ShowPanel(args);

            if (callUI != null)
            {
                if(callUI.name != Consts.DialogueSystemPanel) callUI.HidePanel();
                panelStack.Push(callUI);
            }
            
            //把面板存起来
            panelDic.Add(panelName, panel);
        } );
    }

        
    /// <summary>
    /// 隐藏面板
    /// </summary>
    /// <param name="panelName"></param>
    public void HidePanel(string panelName)
    {
        if(panelDic.ContainsKey(panelName))
        {
            if (panelName == Consts.DialogueSystemPanel)
            {
                StartCoroutine(HideDialoguePanel(panelName));
            }
            else
            {
                panelDic[panelName].HidePanel();
                if (panelName != Consts.StartMenu && panelName != Consts.UILoading)
                {
                    if (panelStack.Count != 0)
                    {
                        BasePanel panel = panelStack.Pop();
                        if (panel.name != Consts.DialogueSystemPanel)
                        {
                            panel.ShowPanel(new UIArgs());
                            currentLayer = EUILayer.Back;
                        }
                        else
                        {
                            currentLayer = EUILayer.Dialog;
                        }
                        
                    }
                    else
                    {
                        Game.Instance.TimeManager.UnPause();
                        gameMask.enabled = false;
                        currentLayer = EUILayer.Battle;
                    }
                }
            }
        }
    }
    
    public T GetDynamicUI<T>(string panelName,int instanceId, UIArgs args = null, UnityAction<T> callBack = null) where T : UIDynamic
    {
        if (dynamicDic.ContainsKey(instanceId))
        {
            dynamicDic[instanceId].ShowPanel(args);
            // 处理面板创建完成后的逻辑
            if (callBack != null)
                callBack(dynamicDic[instanceId] as T);
            //避免面板重复加载 如果存在该面板 即直接显示 调用回调函数后  直接return 不再处理后面的异步加载逻辑
            return dynamicDic[instanceId] as T;
        }

        GameObject obj = Game.Instance.ObjectPool.GetObject(Consts.UIDir + panelName, (args as UIDynamicArgs).father);
        
        //设置父对象  设置相对位置和大小
        obj.name = panelName;
        obj.transform.SetParent(dynamic);
        
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = Vector3.one;
        (obj.transform as RectTransform).offsetMax = Vector2.zero;
        (obj.transform as RectTransform).offsetMin = Vector2.zero;

        //得到预设体身上的面板脚本
        T panel = obj.GetComponent<T>();
        
        panel.ShowPanel(args);
        
        // 处理面板创建完成后的逻辑
        if (callBack != null)
            callBack(panel);

        //把面板存起来
        dynamicDic.Add(instanceId, panel);

        return panel;
    }

    public void HideDynamicPanel(int instanceId)
    {
        if (dynamicDic.ContainsKey(instanceId))
        {
            dynamicDic[instanceId].HidePanel();
            Game.Instance.ObjectPool.RecycleObject(dynamicDic[instanceId].gameObject);
            dynamicDic.Remove(instanceId);
        }
    }


    public GameObject CreateFadeUI()
    {
        if (SceneFadeInAndOut != null) return SceneFadeInAndOut;
        else
        {
            SceneFadeInAndOut = Game.Instance.AssetsManager.Load<GameObject>("UI/SceneFadeInAndOut");
            SceneFadeInAndOut.name = "SceneFadeInAndOut";
            SceneFadeInAndOut.transform.SetParent(top);
            SceneFadeInAndOut.transform.localPosition = Vector3.zero;
            SceneFadeInAndOut.transform.localScale = Vector3.one;
            (SceneFadeInAndOut.transform as RectTransform).offsetMax = Vector2.zero;
            (SceneFadeInAndOut.transform as RectTransform).offsetMin = Vector2.zero;
            SceneFadeInAndOut.SetActive(false);

            return SceneFadeInAndOut;
        }
    }


    //世界坐标转成UI中父节点的坐标, 并设置子节点的位置
    public Vector3 World2UI(Vector3 wpos){
        
        var viewPos = Camera.main.WorldToViewportPoint(wpos);

        if (viewPos.x > 0f && viewPos.x < 1f && viewPos.y > 0f && viewPos.y < 1f)
        {
            ;
        }
        else
        {
            viewPos = Vector3.up * 30f;//目标不在视野内，把它移到视野之外
        }
        var spos = new Vector3((viewPos.x - 0.5f)*Screen.width, (viewPos.y -  0.5f)*Screen.height, viewPos.z);
        return spos;
    }

    /// <summary>
    /// 得到某一个已经显示的面板 方便外部使用
    /// </summary>
    public T GetPanel<T>(string name) where T:BasePanel
    {
        if (panelDic.ContainsKey(name))
            return panelDic[name] as T;
        return null;
    }
    
    private IEnumerator HideDialoguePanel(string panelName)
    {
        Game.Instance.InputManager.canControl = false;
        yield return Game.Instance.CameraManager.FadeIn(0.5f, Color.black);
        panelDic[panelName].HidePanel();
        if (panelStack.Count != 0)
        {
            panelStack.Pop().ShowPanel(new UIArgs());
            currentLayer = EUILayer.Back;
        }
        else
        {
            Game.Instance.TimeManager.UnPause();
            gameMask.enabled = false;
            currentLayer = EUILayer.Battle;
        }
        
        yield return Game.Instance.CameraManager.FadeOut(0.5f, Color.black);
        
        Game.Instance.InputManager.canControl = true;
        Game.Instance.InputManager.inputState = InputState.Gaming;
    }

    public void Endangered(bool isInDanger)
    {
        foreScreen.gameObject.SetActive(isInDanger);
    }
    
    public IEnumerator DialogStartShow(string dialogTag)
    {
        Game.Instance.MusicManager.StopBgMusic();
        Game.Instance.InputManager.canControl = false;
        yield return Game.Instance.CameraManager.FadeIn(0.5f, Color.black);
        
        DialogArgs e = new DialogArgs();
        e.dialogName = dialogTag;
        Game.Instance.UIManager.ShowPanel<DialogueSystemPanel>(Consts.DialogueSystemPanel, e, EUILayer.Dialog);
    }
}

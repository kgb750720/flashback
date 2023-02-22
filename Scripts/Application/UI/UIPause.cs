using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPause : UIButtonController
{

    private EUILayer callLayer = EUILayer.Battle;
    protected override void Awake()
    {
        base.Awake();
    }

    
    protected override void Update()
    {
        if (Game.Instance.UIManager.currentLayer == EUILayer.Back)
        {
            if (Game.Instance.InputManager.UpPress)
            {
                ChangeSelect(currentIndex-1 >= 0 ? currentIndex-1 : 0);
            }

            if (Game.Instance.InputManager.DownPress)
            {
                ChangeSelect(currentIndex+1 < 4 ? currentIndex+1 : 3);
            }

            if (Game.Instance.InputManager.EnterPress)
            {
                Select(currentIndex);
            }

            if (Game.Instance.InputManager.EscPress)
            {
                Game.Instance.UIManager.currentLayer = EUILayer.Battle;
                Game.Instance.UIManager.HidePanel(Consts.UIPause);
            }
        }
    }

    public override void ChangeSelect(int index)
    {
        base.ChangeSelect(index);
    }

    public override void Select(int index)
    {
        base.Select(index);

        switch (index)
        {
            case 0: //返回游戏
                Game.Instance.UIManager.HidePanel(Consts.UIPause);
                Game.Instance.InputManager.inputState = InputState.Gaming;
                break;
            case 1: //Setting
                UICallType argsSetting = new UICallType();
                argsSetting.callUI = Consts.UIPause;
                Game.Instance.UIManager.ShowPanel<UISetting>(Consts.UISetting, argsSetting, EUILayer.Back, this, true, null);
                break;
            case 2: //Help
                UICallType argsHelp = new UICallType();
                argsHelp.callUI = Consts.UIPause;
                Game.Instance.UIManager.ShowPanel<UIHelper>(Consts.UIHelper, argsHelp, EUILayer.Back, this, true, null);
                break;
            case 3: //返回主菜单
                Game.Instance.UIManager.HidePanel(Consts.UIPause);
                Game.Instance.UIManager.HidePanel(Consts.UIBattle);

                if (EffectSystem.EffectManager.Instance.effectPool.Count != 0)
                {
                    foreach (var values in EffectSystem.EffectManager.Instance.effectPool.Values)
                    {
                        for (int i = 0; i < values.Count; ++i)
                        { 
                            ObjectPool.Instance.RecycleObject(values[i].gameObject);
                        }
                    }
                }

                Game.Instance.player.SetActive(false);
                Game.Instance.ScenesManager.LoadSceneAsyn(Consts.StartMenu, () => 
                {
                    Game.Instance.UIManager.ShowPanel<UIStart>(Consts.UIStart, null, EUILayer.Back);
                });
                break;
        }
    }

    public override void ShowPanel(UIArgs args)
    {
        //callLayer = (args as UIPauseArgs).callLayer;
        base.ShowPanel(args);
        Game.Instance.InputManager.inputState = InputState.UI;
    }

    public override void HidePanel()
    {
        base.HidePanel();
        if(callLayer == EUILayer.Battle)
            Game.Instance.InputManager.inputState = InputState.Gaming;
        else if(callLayer == EUILayer.Dialog)
            Game.Instance.InputManager.inputState = InputState.Dialogue;
    }
}

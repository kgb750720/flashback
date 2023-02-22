using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIDead : UIButtonController
{
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
                //TODO：角色重生！！！
                Game.Instance.UIManager.panelStack.Clear();
                Game.Instance.UIManager.HidePanel(Consts.UIDead);
                Game.Instance.ScenesManager.LoadSceneAsyn(Game.Instance.SaveSystem.currentSave.level, () =>
                {
                    Game.Instance.UIManager.currentLayer = EUILayer.Battle;
                    Game.Instance.InputManager.inputState = InputState.Gaming;
                    Game.Instance.EventCenter.EventTrigger(Consts.C_RewindCharacter);
                    
                });
                break;
            case 1: //Setting
                Game.Instance.UIManager.ShowPanel<UISetting>(Consts.UISetting, null, EUILayer.Back, this,true, null);
                break;
            case 2: //Help
                Game.Instance.UIManager.ShowPanel<UIHelper>(Consts.UIHelper, null, EUILayer.Back, this, true, null);
                break;
            case 3: //返回主菜单
                Game.Instance.UIManager.HidePanel(Consts.UIDead);
                Game.Instance.UIManager.HidePanel(Consts.UIBattle);
                Game.Instance.player.SetActive(false);
                Game.Instance.ScenesManager. LoadSceneAsyn(Consts.StartMenu, () =>
                {
                    Game.Instance.UIManager.ShowPanel<UIStart>(Consts.UIStart, null, EUILayer.Back);
                    Game.Instance.EventCenter.EventTrigger(Consts.C_RewindCharacter);
                });
                break;
        }
    }

    public override void ShowPanel(UIArgs args)
    {
        base.ShowPanel(args);
        Game.Instance.InputManager.inputState = InputState.UI;
    }

    public override void HidePanel()
    {
        base.HidePanel();
        Game.Instance.InputManager.inputState = InputState.Gaming;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIStart : UIButtonController
{
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        Game.Instance.InputManager.inputState = InputState.UI;
    }

    // Update is called once per frame
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
            case 0: //进入存档界面
                Game.Instance.UIManager.ShowPanel<UILoad>(Consts.UILoad, null, EUILayer.Back, this, false, arg0 =>
                {
                    arg0.Init();
                });
                break;
            case 1: //Setting
                UICallType argsSetting = new UICallType();
                argsSetting.callUI = Consts.UIStart;
                Game.Instance.UIManager.ShowPanel<UISetting>(Consts.UISetting, argsSetting, EUILayer.Back, this, true, null);
                break;
            case 2: //Help
                UICallType argsHelp = new UICallType();
                argsHelp.callUI = Consts.UIStart;
                Game.Instance.UIManager.ShowPanel<UIHelper>(Consts.UIHelper, argsHelp, EUILayer.Back, this, true, null);
                break;
            case 3: //ExitGame
                Application.Quit();//退出应用
                break;
        }
    }

    public override void ShowPanel(UIArgs args)
    {
        base.ShowPanel(args);
        Game.Instance.InputManager.inputState = InputState.UI;
        Game.Instance.UIManager.currentLayer = EUILayer.Back;
    }

    public override void HidePanel()
    {
        base.HidePanel();
        Game.Instance.InputManager.inputState = InputState.Gaming;
    }
}

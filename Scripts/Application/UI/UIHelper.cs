using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIHelper : BasePanel
{
    private GameObject bg;
    private UnityAction<UIButtonArgs> buttonDown, buttonUp;
    private void Start()
    {
        if(bg == null) bg = this.transform.Find("BackGround").gameObject;
    }

    protected void Update()
    {
        if (Game.Instance.InputManager.EscPress && Game.Instance.UIManager.currentLayer == EUILayer.Back)
        {
            Game.Instance.UIManager.HidePanel(Consts.UIHelper);
        }
    }
    
    public override void ShowPanel(UIArgs args)
    {
        base.ShowPanel(args);
        buttonDown = OnButtonDown;
        Game.Instance.EventCenter.AddEventListener(Consts.M_Button_Down, buttonDown);
        if(bg == null) bg = this.transform.Find("BackGround").gameObject;
        Debug.Log((args as UICallType).callUI);
        if ((args as UICallType).callUI == Consts.UIPause)
        {
            bg.SetActive(false);
        }
        else if ((args as UICallType).callUI == Consts.UIStart)
        {
            bg.SetActive(true);
        }
        else if ((args as UICallType).callUI == Consts.UIBattle)
        {
            bg.SetActive(false);
        }
        Game.Instance.InputManager.inputState = InputState.UI;
    }

    public override void HidePanel()
    {
        base.HidePanel();
        Game.Instance.InputManager.inputState = InputState.Gaming;
        Game.Instance.EventCenter.RemoveEventListener(Consts.M_Button_Down, buttonDown);
    }
    
    private void OnButtonDown(UIButtonArgs args)
    {
        if(args.currentUI == Consts.UIHelper && args.actionType == ActionType.Esc)
            Game.Instance.UIManager.HidePanel(Consts.UIHelper);
    }
}

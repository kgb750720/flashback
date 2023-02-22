using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class UIBackpack : BasePanel
{
    public RectTransform RectItemViewContent;
    public TMP_Text TMP_ItemTittle;
    public TMP_Text TMP_ItemDescribe;

    public BackpackItemIcon defaultIcon;

    private UnityAction<UIButtonArgs> buttonDown, buttonUp;
    
    private void Start()
    {
    }

    private void Update()
    {
        InputCheck();
    }

    void InputCheck()
    {
        if((Game.Instance.InputManager.EscPress|| Game.Instance.InputManager.BackpackPress)&& Game.Instance.UIManager.currentLayer == EUILayer.Mid)
        {
            Game.Instance.UIManager.HidePanel(Consts.UIBackpackPanel);
        }
    }

    public override void ShowPanel(UIArgs args)
    {
        base.ShowPanel(args);
        buttonDown = OnButtonDown;
        Game.Instance.EventCenter.AddEventListener(Consts.M_Button_Down, buttonDown);
        defaultIcon.InvokClick();
    }

    public override void HidePanel()
    {
        base.HidePanel();
        Game.Instance.EventCenter.RemoveEventListener(Consts.M_Button_Down, buttonDown);
    }

    private void OnButtonDown(UIButtonArgs args)
    {
        if(args.currentUI == Consts.UIBackpackPanel && args.actionType == ActionType.Esc)
            Game.Instance.UIManager.HidePanel(Consts.UIBackpackPanel);
    }
}

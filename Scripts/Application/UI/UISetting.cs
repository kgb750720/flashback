using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UISetting : BasePanel
{
    private UISlider[] uiSliders;
    private float allSountRate = 1;
    private float BGRate = 1;
    private float SERate = 1;
    private GameObject bg;
    private UnityAction<UIButtonArgs> buttonDown, buttonUp;
    protected virtual void Start()
    {
        uiSliders = GetComponentsInChildren<UISlider>();
        if(bg == null) bg = this.transform.Find("BackGround").gameObject;
    }

    protected void Update()
    {
        allSountRate = uiSliders[0].value;
        BGRate = uiSliders[1].value;
        SERate = uiSliders[2].value;
        
        Game.Instance.MusicManager.ChangeBgValue(allSountRate*BGRate);
        Game.Instance.MusicManager.ChangeSoundValue(allSountRate*SERate);

        if (Game.Instance.InputManager.EscPress)
        {
            Game.Instance.UIManager.HidePanel(Consts.UISetting);
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
        if(args.currentUI == Consts.UISetting && args.actionType == ActionType.Esc)
            Game.Instance.UIManager.HidePanel(Consts.UISetting);
    }
}

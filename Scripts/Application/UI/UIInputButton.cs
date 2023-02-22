using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIInputButton :MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    public ActionType buttonType;

    public string CurrentUI;
    public bool isTouch = false;
    public bool isDown = false;

    private bool isInTouch = false;
    private Image image;
    void Start()
    {
        if (buttonType != ActionType.None)
        {
            Game.Instance.InputManager.ButtonDic.Add(buttonType, this);
        }

        image = this.transform.GetComponent<Image>();
    }

    
    public void OnPointerDown(PointerEventData eventData)
    {
        if (Game.Instance.InputManager.canControl == false) return;
        UIButtonArgs e = new UIButtonArgs();
        e.actionType = buttonType;
        e.currentUI = CurrentUI;
        Debug.Log(buttonType + CurrentUI);
        Game.Instance.EventCenter.EventTrigger(Consts.M_Button_Down, e);
        isTouch = true;
        image.color = Color.grey;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (Game.Instance.InputManager.canControl == false) return;
        if (Game.Instance.isGameOver) return;
        UIButtonArgs e = new UIButtonArgs();
        e.actionType = buttonType;
        e.currentUI = CurrentUI;
        Game.Instance.EventCenter.EventTrigger(Consts.M_Button_Up, e);
        isTouch = false;
        image.color = Color.white;
    }
}

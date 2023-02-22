using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButtonController : BasePanel
{
    protected int currentIndex = 0;
    protected UIButton[] uiButtons;

    protected virtual void Awake()
    {
        uiButtons = GetComponentsInChildren<UIButton>();
    }

    protected virtual void Update()
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

    public virtual void ChangeSelect(int index)
    {
        uiButtons[currentIndex].DisSelect();
        currentIndex = index;
        uiButtons[index].Select();
    }

    public virtual void Select(int index)
    {
        
    }
}

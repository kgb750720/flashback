using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILoading : BasePanel
{
    public override void ShowPanel(UIArgs args)
    {
        base.ShowPanel(args);
        Game.Instance.InputManager.canControl = false;
    }

    public override void HidePanel()
    {
        base.HidePanel();
        Game.Instance.InputManager.canControl = true;
    }
}

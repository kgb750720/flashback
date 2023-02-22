using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIItemTipArgs : UIArgs
{
    public string Title;
    public string Text;
    public Sprite Icon;
    public Sprite Background;
    public Vector2 BeginPos;
    public Vector2 TargetPos;
    public float moveSpeed = -1;
    public float stayTime = -1;
    public float fadeOutSpeed = -1;
}

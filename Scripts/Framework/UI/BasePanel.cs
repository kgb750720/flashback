using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 面板基类
/// 找到所有自己面板下的控件对象
/// 提供UI的显示和隐藏
/// </summary>
public class BasePanel : MonoBehaviour
{
    public virtual void ShowPanel(UIArgs args)
    {
        this.gameObject.SetActive(true);
    }

    public virtual void HidePanel()
    {
        this.gameObject.SetActive(false);
    }
}


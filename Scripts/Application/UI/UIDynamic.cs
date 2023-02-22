using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDynamic : BasePanel
{
    private Transform father;
    private RectTransform trans;
    private Vector2 offset;

    private Vector3 currentPosition;

    protected virtual void Awake()
    {
        trans = this.transform.GetComponent<RectTransform>();
    }

    public virtual void ShowPanel(UIArgs args)
    {
        base.ShowPanel(args);

        father = (args as UIDynamicArgs).father;
        offset = (args as UIDynamicArgs).offset;

        SetPosition(father.position);
    }

    public void SetPosition(Vector3 pos)
    {
        Vector3 spos = Game.Instance.UIManager.World2UI(new Vector3(pos.x+offset.x, pos.y+offset.y, pos.z));
        currentPosition = spos;
        
        trans.localPosition = currentPosition;
        trans.localScale = Vector3.one;
    }

    protected virtual void Update()
    {
        SetPosition(father.position);
    }
}

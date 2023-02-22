using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(Image))]
public class BackpackItemIcon : MonoBehaviour
{
    Button button;
    Image img;
    public int ItemId = 0;
    public UIBackpack BackpackUI;
    private void Awake()
    {
        button = GetComponent<Button>();
        img = GetComponent<Image>();
        button.onClick.AddListener(() => 
        {
            if((SaveSystem.Instance.currentSave.getedGoods& (1 << ItemId))==0)
            {
                BackpackUI.TMP_ItemTittle.text = "???";
                BackpackUI.TMP_ItemDescribe.text = "???";
            }
            else
            {
                BackpackUI.TMP_ItemTittle.text = StaticData.Instance.itemDatas[ItemId].itemName;
                BackpackUI.TMP_ItemDescribe.text = StaticData.Instance.itemDatas[ItemId].itemDescribe;
            }
        });
    }

    public void InvokClick()
    {
        button.onClick?.Invoke();
    }

    private void OnEnable()
    {
        int goods = SaveSystem.Instance.currentSave.getedGoods;
        if ((goods & (1 << ItemId)) != 0)
        {
            img.sprite = Sprite.Instantiate(Resources.Load<Sprite>("Image/UI/" + StaticData.Instance.itemDatas[ItemId].itemSprite));
        }
        else
        {
            img.sprite = Sprite.Instantiate(Resources.Load<Sprite>("Image/UI/Item_Unknown"));
        }
    }
}

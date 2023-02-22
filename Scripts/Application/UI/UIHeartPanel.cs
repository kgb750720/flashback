using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHeartPanel : MonoBehaviour
{

    private int hp = 5;
    private int maxHp = 5;
    private float slider;
    private List<GameObject> slot = new List<GameObject>();
    public Image HealthBarRed;
    public int Hp
    {
        get => hp;
        set
        {
            slider = (float)value / (float)MaxHp;

            hp = value;
        }
    }

    public int MaxHp
    {
        get => maxHp;
        set
        {
            maxHp = value;
            int count = slot.Count+1;
            if (count < value)
            {
                int i = value - count;
                while (i > 0)
                {
                    var obj = Game.Instance.ObjectPool.GetObject(Consts.UIDir + Consts.Slot, this.transform);
                    //obj.GetComponent<RectTransform>().sizeDelta = new Vector2(0.1f, 0.5f);
                    slot.Add(obj);
                    
                    i--;
                }
            }
            else if (count > value)
            {
                int i = count - value;
                while (i > 0)
                {
                    Game.Instance.ObjectPool.RecycleObject(slot[slot.Count-1]);
                    slot.RemoveAt(slot.Count-1);
                    i--;
                }
            }
        }
    }

    private void Update()
    {
        HealthBarRed.fillAmount = Mathf.Lerp(HealthBarRed.fillAmount, slider, Time.deltaTime*10);
    }
}

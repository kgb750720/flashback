using System;
using System.Collections;
using System.Collections.Generic;
using InteractiveSystem;
using UnityEngine;

[Serializable]
public struct SaveData
{
    public int playerMaxHp;
    public string level;
    public int dialog;
    public float SavePointX;
    public float SavePointY;
    public float SavePointZ;
    public bool getedSkill;
    public int getedGoods;
    public string characString;

    public bool laster102;
    public bool laster204;

    public int DifficultyMode;  // 0：卡咩模式   1：老马模式
}


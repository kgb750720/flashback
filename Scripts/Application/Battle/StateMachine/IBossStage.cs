using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BossStage
{
    Stage1,
    Stage2,
    Stage3,
    Stage4,
    Stage5,
    Stage7,
    Stage8,
    Stage9,
    Stage10,
}

public interface IBossStage
{
    public BossStage bossCurrStage{get;set;}    //Bossµ±Ç°×´Ì¬
}

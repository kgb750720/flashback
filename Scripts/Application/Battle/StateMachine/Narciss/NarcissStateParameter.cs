using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NarcissStateParameter : StateParameter,IBossStage
{
    public BossStage bossCurrStage { get; set; }
    public NarcissStateParameter()
    {
        spawnChaseRadius = 5;
        health = 1750F;
    }
}

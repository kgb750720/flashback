using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMemuBackGround : MonoBehaviour
{
    private Transform charac;

    private void Start()
    {
        charac = Game.Instance.player.transform.Find("Buff");
        this.transform.position = charac.position;
    }
}

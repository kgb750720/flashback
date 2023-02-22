using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAfterImageSprite : MonoBehaviour
{
    [SerializeField]
    private float activeTime = 0.1f;
    private float timeActivated;
    private float alpha;
    [SerializeField]
    private float alphaSet = 0.8f;
    private float alphaMultiplier = 0.85f;
    
    private Transform player;
    private MeshRenderer MR;

    private Color color;

    private void Start()
    {
        Debug.Log(MR);
        MR = GetComponent<MeshRenderer>();

    }

    private void OnEnable()
    {
        alpha = alphaSet;
        timeActivated = Time.time;
    }

    private void Update()
    {
        alpha *= alphaMultiplier;
        Debug.Log(alpha);
        color = new Color(1f,1f,1f,alpha);

        MR.materials[0].color = color;

        if (Time.time >= timeActivated + activeTime)
        {
            //Game.Instance.ObjectPool.RecycleObject(this.gameObject);
        }
    }
}

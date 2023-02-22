using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISign : MonoBehaviour
{
    private Vector3 originPosition;
    private Vector3 currenPosition;

    private float f = 10f;
    private float HZ = 1f;
    
    private void Awake()
    {
        originPosition = transform.localPosition;
    }

    private void Update()
    {
        currenPosition = originPosition;
        currenPosition.y = Mathf.Sin(Time.fixedTime * Mathf.PI * HZ) * f + originPosition.y;
        transform.localPosition = currenPosition;
    }
}

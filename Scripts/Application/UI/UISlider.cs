using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISlider : MonoBehaviour
{
    public float value;
    private TMP_Text count;
    private Slider slider;
    
    void Start()
    {
        slider = gameObject.GetComponent<Slider>();
        count = transform.Find("Count").GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        value = slider.value;
        count.text = ((int)(slider.value * 100)).ToString();
    }
}

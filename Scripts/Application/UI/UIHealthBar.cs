using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHealthBar : UIDynamic
{
    private Image healthBottom;
    private Image health;
    private float targetFillAmount;
    private float currentFillAmount = 1;

    public bool delayFill = true;
    public float fillDelay = 0.5f;
    public float fiilSpeed = 0.5f;
    
    private WaitForSeconds waitForDelayFill;

    private Coroutine coroutine;
    
    private float t;


    private void Awake()
    {
        base.Awake();
        healthBottom = transform.Find("healthBottom").GetComponent<Image>();
        health = transform.Find("health").GetComponent<Image>();
        healthBottom.fillAmount = 1;
        health.fillAmount = 1;
        
        waitForDelayFill = new WaitForSeconds(fillDelay);
    }

    public void UpdateHealth(float healthPercent)
    {
        if (!this.gameObject.activeSelf)
        {
            Debug.LogError("怪物血条已被回收，调用非法！");
            return;
        }
        //if (healthPercent < 0 || healthPercent > 1 || currentFillAmount == healthPercent) return; 
        if (coroutine != null) StopCoroutine(coroutine);

        targetFillAmount = healthPercent;
        if (currentFillAmount > targetFillAmount)
        {
            health.fillAmount = targetFillAmount;
            coroutine = StartCoroutine(BufferedFillingCoroutine(healthBottom));
        }

        if (currentFillAmount < targetFillAmount)
        {
            healthBottom.fillAmount = targetFillAmount;
            coroutine = StartCoroutine(BufferedFillingCoroutine(health));
        }
        
    }

    public override void ShowPanel(UIArgs args)
    {
        base.ShowPanel(args);
        healthBottom.fillAmount = 1;
        health.fillAmount = 1;
    }

    IEnumerator BufferedFillingCoroutine(Image image)
    {
        if (delayFill) yield return waitForDelayFill;
        
        t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * fiilSpeed;
            currentFillAmount = Mathf.Lerp(currentFillAmount, targetFillAmount, t);
            image.fillAmount = currentFillAmount;

            yield return null;
        }
    }
}

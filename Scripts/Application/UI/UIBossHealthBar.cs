 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 using UnityEngine.UI;

 public class UIBossHealthBar : UIDynamic
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
    protected override void Awake()
    {
        base.Awake();
        healthBottom = transform.Find("healthBottom").GetComponent<Image>();
        health = transform.Find("health").GetComponent<Image>();
        healthBottom.fillAmount = 0;
        health.fillAmount = 0;
        targetFillAmount = 1;
        coroutine = StartCoroutine(BufferedFillingCoroutine(health, healthBottom));
        waitForDelayFill = new WaitForSeconds(fillDelay);
    }

    public override void ShowPanel(UIArgs args)
    {
        this.gameObject.SetActive(true);
    }

    public void UpdateHealth(float healthPercent)
    {
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
    
    IEnumerator BufferedFillingCoroutine(Image imageTop, Image imageBottom)
    {
        if (delayFill) yield return waitForDelayFill;
        
        t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * fiilSpeed;
            currentFillAmount = Mathf.Lerp(currentFillAmount, targetFillAmount, t);
            imageTop.fillAmount = currentFillAmount;
            imageBottom.fillAmount = currentFillAmount;

            yield return null;
        }
    }

    protected override void Update()
    {
        
    }
}

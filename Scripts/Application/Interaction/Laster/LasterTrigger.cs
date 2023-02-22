using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LasterTrigger : MonoBehaviour
{
    public GameObject laster;
    public GameObject moveFather;
    public GameObject moveSon;

    private float time;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.LogError(time);

            if (!laster.activeInHierarchy && time > 1F)
            {
                laster.SetActive(true);
                moveFather.SetActive(true);
                moveSon.SetActive(true);

                enabled = false;
            }
        }
    }

    private void Update()
    {
        time += Time.deltaTime;
    }
}

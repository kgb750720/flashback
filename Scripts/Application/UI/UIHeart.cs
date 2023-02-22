using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UIHeart : MonoBehaviour
{
    private GameObject heart;
    
    // Start is called before the first frame update
    public void Init()
    {
        heart = this.transform.Find("RedHeart").gameObject;
    }

    public void SetActive(bool active)
    {
        heart.SetActive(active);
    }
}

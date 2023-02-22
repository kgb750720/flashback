using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[Serializable]
public enum UIButtonType
{
    Normal,
    HaveSelected
}

public class UIButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    public UIButtonController controller;
    public int index;
    public Sprite UnSelectImage;
    public Sprite SelectImage;

    public UIButtonType ButtonType;
    private Button btn;
    private GameObject selected;
    private Image Light;
    private Sprite currentSprite;
    
    // Start is called before the first frame update
    void Awake()
    {
        btn = GetComponent<Button>();
        selected = this.transform.Find("Select").gameObject;
        Light = this.transform.Find("Light").GetComponent<Image>();
        currentSprite = Light.sprite;
    }

    public void Select()
    {
        selected.SetActive(true);
        Light.gameObject.SetActive(true);
    }

    public void DisSelect()
    {
        selected.SetActive(false);
        Light.gameObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        controller.ChangeSelect(index);
        if(ButtonType == UIButtonType.HaveSelected) Light.sprite = SelectImage;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(ButtonType == UIButtonType.HaveSelected) Light.sprite = UnSelectImage;
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        controller.Select(index);
    }
}

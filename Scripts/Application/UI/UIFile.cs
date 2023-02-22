using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIFile : BasePanel
{
    private GameObject goNewGame;
    private TMP_Text level;
    private GameObject goCharac;
    private Image charac;
    private GameObject heartPanel; 
    
    
    // Start is called before the first frame update
    void Awake()
    {
        goNewGame = transform.Find("NewGame").gameObject;
        level = transform.Find("Level").GetComponent<TMP_Text>();
        goCharac = transform.Find("charac").gameObject;
        charac = goCharac.GetComponent<Image>();
        heartPanel = transform.Find("HeartPanel").gameObject;
    }

    public void Init(bool hasFile, Sprite characSprite = null, int heart = 0, string levelstr = "1-1")
    {
        if (!hasFile)
        {
            goNewGame.SetActive(true);
            level.gameObject.SetActive(false);
            goCharac.SetActive(false);
            heartPanel.SetActive(false);
        }
        else
        {
            goNewGame.SetActive(false);
            level.gameObject.SetActive(true);
            goCharac.SetActive(true);
            heartPanel.SetActive(true);
            if(characSprite != null) charac.sprite = characSprite;
            while(heart > 0)
            {
                heart--;
                var go = Game.Instance.AssetsManager.Load<GameObject>("Heart");
                if (go != null)
                {
                    go.GetComponent<RectTransform>().sizeDelta = new Vector2(50, 50);
                    go.transform.parent = heartPanel.transform;
                }
            }

            level.text = levelstr;
        }
    }
}

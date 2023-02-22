using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public struct Dialog
{
    public int performMode;
    public float fadeIn;
    public float fadeKeep;
    public float fadeOut;
    public string name;
    public List<string> sprite;
    public List<int> speaker;
    public string BGM;
    public List<string> SE;
    public string content;
    public string BG;
    public List<float> BGShake;
    
    public Dialog(XmlNode xmlContent)
    {
        sprite = new List<string>();
        speaker = new List<int>();
        SE = new List<string>();
        BGShake = new List<float>();
        
        string[] temp;
        if (xmlContent.Attributes.GetNamedItem("performMode") == null) performMode = -1;
        else performMode = int.Parse(xmlContent.Attributes.GetNamedItem("performMode").Value);
        if (xmlContent.Attributes.GetNamedItem("fadein") == null) fadeIn = -1;
        else fadeIn = float.Parse(xmlContent.Attributes.GetNamedItem("fadein").Value);        
        if (xmlContent.Attributes.GetNamedItem("fadeKeep") == null) fadeKeep = -1;
        else fadeKeep = float.Parse(xmlContent.Attributes.GetNamedItem("fadeKeep").Value);
        if (xmlContent.Attributes.GetNamedItem("fadeout") == null) fadeOut = -1;
        else fadeOut = float.Parse(xmlContent.Attributes.GetNamedItem("fadeout").Value);
        if (xmlContent.Attributes.GetNamedItem("name") == null) name = "";
        else name = xmlContent.Attributes.GetNamedItem("name").Value;

        if (xmlContent.Attributes.GetNamedItem("sprite") != null)
        {
            temp = xmlContent.Attributes.GetNamedItem("sprite").Value.Split('|');
            foreach (string var in temp)
            {
                sprite.Add(var);
            }
        }

        if (xmlContent.Attributes.GetNamedItem("speaker") != null)
        {
            temp = xmlContent.Attributes.GetNamedItem("speaker").Value.Split('|');
            foreach (string var in temp)
            {
                speaker.Add(int.Parse(var));
            }
        }

        if (xmlContent.Attributes.GetNamedItem("BGM") == null) BGM = "";
        else BGM = xmlContent.Attributes.GetNamedItem("BGM").Value;

        if (xmlContent.Attributes.GetNamedItem("SE") != null)
        {
            temp = xmlContent.Attributes.GetNamedItem("SE").Value.Split('|');
            foreach (string var in temp)
            {
                SE.Add(var);
            }
        }

        if (xmlContent.Attributes.GetNamedItem("BG") == null) BG = "";
        else BG = xmlContent.Attributes.GetNamedItem("BG").Value;

        if (xmlContent.Attributes.GetNamedItem("BGShake") != null)
        {
            temp = xmlContent.Attributes.GetNamedItem("BGShake").Value.Split('|');
            foreach (string var in temp)
            {
                BGShake.Add(float.Parse(var));
            }
        } 
        
        content = xmlContent.InnerText;
    }
}


public class DialogueSystemPanel : BasePanel
{

    private RawImage bg;
    private List<RawImage> character = new List<RawImage>();
    private TMP_Text nameTmp;
    private TMP_Text contentTmp;
    private TMP_Text mainTmp;
    private TMP_Text realTmp;
    private GameObject characterGroup;
    private GameObject mainPanel;
    private GameObject sign;

    private XmlNodeList dialogContent;
    private int index = 0;

    private bool isShowByCharCompleted = true;
    private bool isShowCompleted = true;
    private bool isSkip = false;
    private string currentDialogName;

    [Header("对话播放速度")]
    public float playSpeed = 1f;
    
    private void Awake()
    {
        bg = this.transform.Find("BackGround").GetComponent<RawImage>();
        mainTmp = this.transform.Find("mainTmp").GetComponent<TMP_Text>();
        characterGroup = this.transform.Find("SpriteGroup").gameObject;
        character.Add(characterGroup.transform.Find("character1").GetComponent<RawImage>());
        character.Add(characterGroup.transform.Find("character2").GetComponent<RawImage>());
        mainPanel = this.transform.Find("mainPanel").gameObject;
        nameTmp = mainPanel.transform.Find("nameTmp").GetComponent<TMP_Text>();
        contentTmp = mainPanel.transform.Find("contentTmp").GetComponent<TMP_Text>();
        sign = mainPanel.transform.Find("Sign").gameObject;
    }

    private void InitContent()
    {
        mainTmp.text = string.Empty;
        nameTmp.text = string.Empty;
        contentTmp.text = string.Empty;
        sign.SetActive(false);
    }

    private void Update()
    {
        if (Game.Instance.InputManager.inputState != InputState.Dialogue) return;

        if (Game.Instance.InputManager.AnyKeyPress && !isShowByCharCompleted)
        {
            isSkip = true;
        }
        else if (Game.Instance.InputManager.AnyKeyPress && isShowCompleted)
        {
            StartCoroutine(Talk());
        }
        /*else if(Game.Instance.InputManager.EscPress)
        {
            if(!isShowByCharCompleted) isSkip = true;
            index = dialogContent.Count;
            StartCoroutine(Talk());
        }*/
        
        /*if (Game.Instance.InputManager.EscPress && Game.Instance.UIManager.currentLayer == EUILayer.Dialog)
        {
            Game.Instance.UIManager.currentLayer = EUILayer.Back;
            UIPauseArgs e = new UIPauseArgs();
            e.callLayer = EUILayer.Dialog;
            Game.Instance.UIManager.ShowPanel<UIPause>(Consts.UIPause, e, EUILayer.Back, this, true, null);
        }*/
    }

    private IEnumerator Talk()
    {
        sign.SetActive(false);
        if (index < dialogContent.Count)
        {
            isShowCompleted = false;
            Dialog dialog = new Dialog(dialogContent[index]); //读取当前文件
            yield return (SetDialogShow(dialog));    //处理场景设置

            yield return ShowDialogByChar(dialog.content);//逐字显示
            isShowCompleted = true;
            index++;
        }
        else
        {
            DialogCompleteArgs e = new DialogCompleteArgs();
            e.dialogName = currentDialogName;
            Game.Instance.EventCenter.EventTrigger(Consts.E_TalkCompleted, e);
            Game.Instance.UIManager.isDialogEnd = true;
            index = 0;
            Game.Instance.UIManager.HidePanel(Consts.DialogueSystemPanel);
            
            Game.Instance.SaveSystem.currentSave.dialog = Int32.Parse(currentDialogName.Substring(currentDialogName.Length - 1, 1));
            if(Game.Instance.StaticData.dialogSceneHash.ContainsKey(currentDialogName)) Game.Instance.SaveSystem.Save();
            
            if (Game.Instance.StaticData.sceneMusicHash.ContainsKey(Game.Instance.ScenesManager.currentScene))
            {
                Game.Instance.MusicManager.PlayBgMusic(Game.Instance.StaticData.sceneMusicHash[Game.Instance.ScenesManager.currentScene]);
            }
        }
    }

    private IEnumerator SetDialogShow(Dialog dialog)
    {
        if (dialog.fadeIn > 0)
        {
            Game.Instance.InputManager.canControl = false;
            yield return Game.Instance.CameraManager.FadeIn(dialog.fadeIn, Color.black);
        }

        if (dialog.BG != string.Empty)
        {
            if (dialog.BG == "Null") bg.color = Color.clear;
            else
            {
                Texture bgTexture = Game.Instance.AssetsManager.Load<Texture>(Consts.DialogSpriteDir + dialog.BG);
                if (bgTexture == null) Debug.Log("Can't Get The Texture!");
                else bg.texture = bgTexture;
                bg.color = Color.white;
            }
        }

        if (dialog.performMode == 1)
        {
            characterGroup.SetActive(false);
            mainPanel.SetActive(false);
            mainTmp.gameObject.SetActive(true);
            bg.color = Color.gray;
            realTmp = mainTmp;
        }
        else if(dialog.performMode == 0)
        {
            characterGroup.SetActive(true);
            mainPanel.SetActive(true);
            mainTmp.gameObject.SetActive(false);
            bg.color = Color.white;
            realTmp = contentTmp;
        }
        
        SetMainPanel(dialog);

        SetMusic(dialog);

        if (dialog.fadeKeep > 0)
        {
            yield return new WaitForSeconds(dialog.fadeKeep);
        }
        
        if (dialog.fadeOut > 0)
        {
            yield return Game.Instance.CameraManager.FadeOut(dialog.fadeOut, Color.black);
        }

        if (dialog.BGShake.Count != 0)
        {
            Game.Instance.CameraManager.DoShake(bg.transform,dialog.BGShake[0],dialog.BGShake[1], (int)dialog.BGShake[2]);
        }

        if (dialog.speaker.Count != 0)
        {
            for(int i = 0;i < dialog.speaker.Count;i++)
            {
                if (dialog.speaker[i] == 2)
                {
                    Game.Instance.CameraManager.DoShake(character[i].transform,2f,1f);
                }
            }
        }
        
        Game.Instance.InputManager.canControl = true;
    }

    private void SetMainPanel(Dialog dialog)
    {
        if (dialog.name != string.Empty)
        {
            if (dialog.name == "Null") nameTmp.text = "";
            else nameTmp.text = dialog.name;
        }

        if (dialog.sprite.Count != 0)
        {
            for (int i = 0; i < dialog.sprite.Count; i++)
            {
                if (dialog.sprite[i] == "Null")
                {
                    character[i].gameObject.SetActive(false);
                }
                else
                {
                    character[i].gameObject.SetActive(true);
                    character[i].texture = Game.Instance.AssetsManager.Load<Texture>(Consts.DialogSpriteDir + dialog.sprite[i]);
                }
            }
        }

        if (dialog.speaker.Count != 0)
        {
            for (int i = 0; i < dialog.speaker.Count; i++)
            {
                if (dialog.speaker[i] == 0)
                {
                    character[i].color = Color.gray;
                }
                else if(dialog.speaker[i] == 1)
                {
                    character[i].color = Color.white;
                }
                else if (dialog.speaker[i] == 2)
                {
                    character[i].color = Color.white;
                }
            }
        }
    }

    private void SetMusic(Dialog dialog)
    {
        if (dialog.BGM != null)
        {
            if (dialog.BGM == "Null")
            {
                Game.Instance.MusicManager.StopBgMusic();
            }
            else if(dialog.BGM != "")
            {
                Game.Instance.MusicManager.PlayBgMusic(dialog.BGM);
            }
        }

        if (dialog.SE.Count > 0)
        {
            Debug.Log("SE Number:" + dialog.SE.Count);
            for (int i = 0; i < dialog.SE.Count; i++)
            {
                Debug.Log(dialog.SE[i]);
                if (dialog.SE[i] != "Null")
                {
                    Game.Instance.MusicManager.PlaySound(dialog.SE[i], this.gameObject, false, 0f);
                }
            }
        }
    }
    
    IEnumerator ShowDialogByChar(string _content)
    {
        isShowByCharCompleted = false;
        realTmp.text = string.Empty;
        string rich = string.Empty;
        for (int i = 0; i < _content.Length; i++)
        {
            if (isSkip)
            {
                realTmp.text = _content;
                isSkip = false;
                break;
            }
            ///使用富文本标签
            if (_content[i] == "<".ToCharArray()[0])
            {
                int t = i;
                for (int j = t; j < _content.Length; j++, i++)
                {
                    if (_content[j] == ">".ToCharArray()[0])
                    {
                        rich += ">";
                        realTmp.text += rich;
                        break;
                    }
                    else
                    {
                        rich += _content[j];
                    }
                }
            }
            else
            {
                realTmp.text += _content[i];
            }
            
            yield return new WaitForSeconds(0.1f/(playSpeed <= 0 ? 1 : playSpeed));
        }
        sign.SetActive(true);
        isShowByCharCompleted = true;
    }


    public override void ShowPanel(UIArgs args)
    {
        Debug.Log("Show dialog");
        DialogArgs e = args as DialogArgs;
        Game.Instance.InputManager.inputState = InputState.Dialogue;
        currentDialogName = e.dialogName;
        dialogContent = XmlSystemHelper.GetDialogFromXml(Consts.DialogXmlFileName, e.dialogName);
        base.ShowPanel(e);
        InitContent();
        StartCoroutine(Talk());
    }
    public override void HidePanel()
    {
        base.HidePanel();
    }
}

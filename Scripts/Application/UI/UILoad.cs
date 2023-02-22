using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Events;

public class UILoad : UIButtonController
{
    private UIFile uiFile1;
    private UIFile uiFile2;
    private SaveData s1;
    private SaveData s2;
    private string dirPath1;
    private string dirPath2;
    private bool hasFile1;
    private bool hasFile2;
    private UnityAction<UIButtonArgs> buttonDown, buttonUp;
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Update()
    {
        if (Game.Instance.InputManager.UpPress)
        {
            if(currentIndex == 3 && hasFile1) ChangeSelect(1);
            if(currentIndex == 2) ChangeSelect(0);
        }

        if (Game.Instance.InputManager.DownPress)
        {
            if(currentIndex == 0) ChangeSelect(2);
            if(currentIndex == 1 && hasFile2) ChangeSelect(3);
        }

        if (Game.Instance.InputManager.LeftPress)
        {
            if(currentIndex == 1) ChangeSelect(0);
            if(currentIndex == 3) ChangeSelect(2);
        }
        
        if (Game.Instance.InputManager.RightPress)
        {
            if(currentIndex == 0 && hasFile1) ChangeSelect(1);
            if(currentIndex == 2 && hasFile2) ChangeSelect(3);
        }

        if (Game.Instance.InputManager.EnterPress)
        {
            Select(currentIndex);
        }

        if (Game.Instance.InputManager.EscPress)
        {
            Game.Instance.UIManager.HidePanel(Consts.UILoad);
        }
    }

    public void Init()
    {
        dirPath1 = Application.persistentDataPath + "/Save/flashback.sav1";
        dirPath2 = Application.persistentDataPath + "/Save/flashback.sav2";
        uiFile1 = this.transform.Find("UIFile1").GetComponent<UIFile>();
        uiFile2 = this.transform.Find("UIFile2").GetComponent<UIFile>();
        if (!System.IO.File.Exists(dirPath1))
        {
            hasFile1 = false;
            s1 = Game.Instance.StaticData.initialData;
            uiButtons[1].gameObject.SetActive(false);
            uiFile1.Init(false);
        }
        else
        {
            hasFile1 = true;
            Debug.Log(dirPath1);
            s1 = Game.Instance.SaveSystem.LoadFromJson<SaveData>(dirPath1);
            uiButtons[1].gameObject.SetActive(true);
            Sprite sprite = Game.Instance.AssetsManager.Load<Sprite>(Consts.UISpriteDir + s1.characString);
            uiFile1.Init(true, sprite, s1.playerMaxHp, Game.Instance.StaticData.sceneHash[s1.level]);
        }
        if (!System.IO.File.Exists(dirPath2))
        {
            hasFile2 = false;
            s2 = Game.Instance.StaticData.initialData;
            uiButtons[3].gameObject.SetActive(false);
            uiFile2.Init(false);
        }
        else
        {
            hasFile2 = true;
            s2 = Game.Instance.SaveSystem.LoadFromJson<SaveData>(dirPath2);
            uiButtons[3].gameObject.SetActive(true);
            Sprite sprite = Game.Instance.AssetsManager.Load<Sprite>(Consts.UISpriteDir + s2.characString);
            uiFile2.Init(true, sprite, s2.playerMaxHp, Game.Instance.StaticData.sceneHash[s2.level]);
        }
        //ChangeSelect(0);
    }
    
    
    public override void ChangeSelect(int index)
    {
        if(uiButtons[currentIndex]!=null) uiButtons[currentIndex].DisSelect();
        currentIndex = index;
        if(uiButtons[index]!=null) uiButtons[index].Select();
    }

    public override void Select(int index)
    {
        switch (uiButtons[index].gameObject.name)
        {
            case "UIFile1":
                Game.Instance.SaveSystem.currentSave = s1;
                Game.Instance.SaveSystem.currentSaveDir = "flashback.sav1";
                Game.Instance.UIManager.panelStack.Clear();
                Game.Instance.UIManager.HidePanel(Consts.UILoad);
                Game.Instance.ScenesManager.LoadSceneAsyn(Game.Instance.SaveSystem.currentSave.level, () =>
                {
                    Game.Instance.UIManager.currentLayer = EUILayer.Battle;
                    Game.Instance.InputManager.inputState = InputState.Gaming;
                    Game.Instance.player.SetActive(true);
                    var position = new Vector3(Game.Instance.SaveSystem.currentSave.SavePointX, Game.Instance.SaveSystem.currentSave.SavePointY, Game.Instance.SaveSystem.currentSave.SavePointZ);
                    Game.Instance.player.transform.position = position;
#if UNITY_STANDALONE_WIN
                    Game.Instance.UIManager.ShowPanel<UIBattle>(Consts.UIBattle, null, EUILayer.Battle, null,false,
                        arg0 =>
                        {
                            PlayerManager pm = Game.Instance.player.GetComponent<PlayerManager>();
                            pm.InitUI(arg0);
                            pm.Init();
                            pm.SetCharacterPosition(position);
                            pm.RefreshCurrentCharacter();
                            Game.Instance.SaveSystem.Load();
                            Game.Instance.SaveSystem.currentSave.DifficultyMode = 0;
                        });
#endif
#if UNITY_ANDROID
                    Game.Instance.UIManager.ShowPanel<UIPhoneBattle>(Consts.UIPhoneBattle, null, EUILayer.Battle, null,false,
                        arg0 =>
                        {
                            PlayerManager pm = Game.Instance.player.GetComponent<PlayerManager>();
                            pm.InitUI(arg0);
                            pm.Init();
                            pm.SetCharacterPosition(position);
                            pm.RefreshCurrentCharacter();
                            Game.Instance.SaveSystem.Load();
                            Game.Instance.SaveSystem.currentSave.DifficultyMode = 0;
                        });
#endif
                });
                break;
            case "Delete1":
                Game.Instance.SaveSystem.DeleteSaveFile(dirPath1);
                Init();
                break;
            case "UIFile2":
                Game.Instance.SaveSystem.currentSave = s2;
                Game.Instance.SaveSystem.currentSaveDir = "flashback.sav2";
                Game.Instance.UIManager.panelStack.Clear();
                Game.Instance.UIManager.HidePanel(Consts.UILoad);
                Game.Instance.ScenesManager.LoadSceneAsyn(Game.Instance.SaveSystem.currentSave.level, () =>
                {
                    Game.Instance.UIManager.currentLayer = EUILayer.Battle;
                    Game.Instance.InputManager.inputState = InputState.Gaming;
                    Game.Instance.player.SetActive(true);
                    var position = new Vector3(Game.Instance.SaveSystem.currentSave.SavePointX, Game.Instance.SaveSystem.currentSave.SavePointY, Game.Instance.SaveSystem.currentSave.SavePointZ);
                    Game.Instance.player.transform.position = position;
#if UNITY_STANDALONE_WIN
                    Game.Instance.UIManager.ShowPanel<UIBattle>(Consts.UIBattle, null, EUILayer.Battle, null,false,
                        arg0 =>
                        {
                            PlayerManager pm = Game.Instance.player.GetComponent<PlayerManager>();
                            pm.InitUI(arg0);
                            pm.Init();
                            pm.SetCharacterPosition(position);
                            pm.RefreshCurrentCharacter();
                            Game.Instance.SaveSystem.Load();
                            Game.Instance.SaveSystem.currentSave.DifficultyMode = 1;
                        });
#endif
#if UNITY_ANDROID
                    Game.Instance.UIManager.ShowPanel<UIPhoneBattle>(Consts.UIPhoneBattle, null, EUILayer.Battle, null,false,
                        arg0 =>
                        {
                            PlayerManager pm = Game.Instance.player.GetComponent<PlayerManager>();
                            pm.InitUI(arg0);
                            pm.Init();
                            pm.SetCharacterPosition(position);
                            pm.RefreshCurrentCharacter();
                            Game.Instance.SaveSystem.Load();
                            Game.Instance.SaveSystem.currentSave.DifficultyMode = 1;
                        });
#endif
                });
                break;
            case "Delete2":
                Game.Instance.SaveSystem.DeleteSaveFile(dirPath2);
                Init();
                break;
        }
    }

    public override void ShowPanel(UIArgs args)
    {
        buttonDown = OnButtonDown;
        Game.Instance.EventCenter.AddEventListener(Consts.M_Button_Down, buttonDown);
        base.ShowPanel(args);
        Game.Instance.InputManager.inputState = InputState.UI;
    }

    public override void HidePanel()
    {
        base.HidePanel();
        Game.Instance.InputManager.inputState = InputState.Gaming;
        Game.Instance.EventCenter.RemoveEventListener(Consts.M_Button_Down, buttonDown);
    }
    
    private void OnButtonDown(UIButtonArgs args)
    {
        if(args.currentUI == Consts.UILoad && args.actionType == ActionType.Esc)
            Game.Instance.UIManager.HidePanel(Consts.UILoad);
    }
}

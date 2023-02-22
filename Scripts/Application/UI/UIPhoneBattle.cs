using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class UIPhoneBattle : UIBattle
{

    private GameObject interactiveButton;
    //private Image Icon;
    //private Image Fill;
    //private UIHeartPanel heartPanel;
    private UIPhoneSkillPanel skillPanel;
    //private float target;

    //private List<Sprite> IconSprite = new List<Sprite>();

    //private UnityAction<SwitchCharacterArgs> cacheSwitchCharacterHandler;
    //private UnityAction<SkillLaunchedArgs> cacheSkillLaunchedHandler;

    private VariableJoystick variableJoystick;

    private Image dashSlider;
    //private UISkillPanel uiSkillPanel;

    private void Awake()
    {
        cacheSwitchCharacterHandler = (e) => SwitchSprite(e);
        cacheSkillLaunchedHandler = (args) => SkillLaunched(args);
    }

    public override void Init()
    {
        Icon = this.transform.Find("Icon").GetComponent<Image>();
        Fill = this.transform.Find("Switch").GetComponent<Image>();
        heartPanel = this.transform.Find("HeartPanel").GetComponent<UIHeartPanel>();
        skillPanel = this.transform.Find("UISkillPanel").GetComponent<UIPhoneSkillPanel>();
        dashSlider = this.transform.Find("Dash").GetComponent<Image>();
        variableJoystick = this.transform.Find("Move").GetComponent<VariableJoystick>();
        interactiveButton = this.transform.Find("Interactive").gameObject;
        interactiveButton.SetActive(false);
        
        Game.Instance.InputManager.variableJoystick = variableJoystick;

        IconSprite.Add(Game.Instance.AssetsManager.Load<Sprite>(Consts.UISpriteDir + "Avatar_0"));
        IconSprite.Add(Game.Instance.AssetsManager.Load<Sprite>(Consts.UISpriteDir + "Avatar_1"));
    }

    private void OnEnable()
    {
        Game.Instance.EventCenter.AddEventListener<SwitchCharacterArgs>(Consts.C_SwitchCharacter, cacheSwitchCharacterHandler);
        Game.Instance.EventCenter.AddEventListener<SkillLaunchedArgs>(Consts.C_SkillLaunched, cacheSkillLaunchedHandler);
        Game.Instance.EventCenter.AddEventListener(Consts.T_D_Start, ShowInteractive);
        Game.Instance.EventCenter.AddEventListener(Consts.T_D_Over, HideInteractive);
    }

    private void OnDisable()
    {
        Game.Instance.EventCenter.RemoveEventListener<SwitchCharacterArgs>(Consts.C_SwitchCharacter, cacheSwitchCharacterHandler);
        Game.Instance.EventCenter.RemoveEventListener<SkillLaunchedArgs>(Consts.C_SkillLaunched, cacheSkillLaunchedHandler);
        Game.Instance.EventCenter.RemoveEventListener(Consts.T_D_Start, ShowInteractive);
        Game.Instance.EventCenter.RemoveEventListener(Consts.T_D_Over, HideInteractive);
    }

    public void SetDashCd(float slider)
    {
        dashSlider.fillAmount = slider;
    }
    
    public override void SetHpMax(int hp)
    {
        heartPanel.MaxHp = hp;
    }
    
    public override void SetHp(int hp)
    {
        heartPanel.Hp = hp;
    }

    public override void SwitchSprite(SwitchCharacterArgs e)
    {
        Icon.sprite = IconSprite[e.CharacId];
        skillPanel.SetSkill(e.characSkillName);
    }

    public override void SetSkillCd(float cd)
    {
        skillPanel.SetSkillCd(cd);
    }

    public override void SetUltimateSkillCd(float cd)
    {
        skillPanel.SetUltimateSkillCd(cd);
    }

    public override void SkillLaunched(SkillLaunchedArgs args)
    {
        skillPanel.SetNumber(args.skillList, args.cacheSkill);
    }

    public override void SetSwitchCd(float slider)
    {
        Fill.fillAmount = slider;
    }

    private void ShowInteractive()
    {
        Debug.Log("Show!!!!!!!");
        interactiveButton.SetActive(true);
    }

    private void HideInteractive()
    {
        interactiveButton.SetActive(false);
    }
}

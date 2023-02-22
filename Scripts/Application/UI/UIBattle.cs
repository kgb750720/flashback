using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class UIBattle : BasePanel
{
    protected Image Icon;
    protected Image Fill;
    protected UIHeartPanel heartPanel;
    private UISkillPanel skillPanel;
    protected float target;

    protected List<Sprite> IconSprite = new List<Sprite>();

    protected UnityAction<SwitchCharacterArgs> cacheSwitchCharacterHandler;
    protected UnityAction<SkillLaunchedArgs> cacheSkillLaunchedHandler;
     
    //private UISkillPanel uiSkillPanel;

    private void Awake()
    {
        cacheSwitchCharacterHandler = (e) => SwitchSprite(e);
        cacheSkillLaunchedHandler = (args) => SkillLaunched(args);
    }

    public virtual void Init()
    {
        Icon = this.transform.Find("Icon").GetComponent<Image>();
        Fill = this.transform.Find("Fill").GetComponent<Image>();
        heartPanel = this.transform.Find("HeartPanel").GetComponent<UIHeartPanel>();
        skillPanel = this.transform.Find("UISkillPanel").GetComponent<UISkillPanel>();
        
        IconSprite.Add(Game.Instance.AssetsManager.Load<Sprite>(Consts.UISpriteDir + "Avatar_0"));
        IconSprite.Add(Game.Instance.AssetsManager.Load<Sprite>(Consts.UISpriteDir + "Avatar_1"));
    }

    private void OnEnable()
    {
        Game.Instance.EventCenter.AddEventListener<SwitchCharacterArgs>(Consts.C_SwitchCharacter, cacheSwitchCharacterHandler);
        Game.Instance.EventCenter.AddEventListener<SkillLaunchedArgs>(Consts.C_SkillLaunched, cacheSkillLaunchedHandler);
    }

    private void OnDisable()
    {
        Game.Instance.EventCenter.RemoveEventListener<SwitchCharacterArgs>(Consts.C_SwitchCharacter, cacheSwitchCharacterHandler);
        Game.Instance.EventCenter.RemoveEventListener<SkillLaunchedArgs>(Consts.C_SkillLaunched, cacheSkillLaunchedHandler);
    }
    
    public virtual void SetHpMax(int hp)
    {
        heartPanel.MaxHp = hp;
    }
    
    public virtual void SetHp(int hp)
    {
        heartPanel.Hp = hp;
    }

    public virtual void SwitchSprite(SwitchCharacterArgs e)
    {
        Icon.sprite = IconSprite[e.CharacId];
        skillPanel.SetSkill(e.characSkillName);
    }

    public virtual void SetSkillCd(float cd)
    {
        skillPanel.SetSkillCd(cd);
    }

    public virtual void SetUltimateSkillCd(float cd)
    {
        skillPanel.SetUltimateSkillCd(cd);
    }

    public virtual void SkillLaunched(SkillLaunchedArgs args)
    {
        skillPanel.SetNumber(args.skillList, args.cacheSkill);
    }

    public virtual void SetSwitchCd(float slider)
    {
        Fill.fillAmount = slider;
    }
    
    // Update is called once per frame
    void Update()
    {

    }
}

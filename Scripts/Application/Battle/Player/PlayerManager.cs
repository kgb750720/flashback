using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using EffectSystem;
using InteractiveSystem;
using SkillSystem;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerManager : SingletonMono<PlayerManager>, ISaveable
{
    [HideInInspector]
    public UIBattle uiBattle;
    private SkillManager sm;
    private CDController cc;
    private GameObject buff;
    private UnityAction<SkillBuffArgs> accelerateSkillBuffAcyion;
    private UnityAction<PropPickUp> unlockPower, unlockItem;
    private UnityAction<SaveDataArgs> archive;
    private UnityAction<UIButtonArgs> buttonDown, buttonUp;
    private List<RealCharacterController> characterControllers = new List<RealCharacterController>();
    private int controlCharacterIndex = 0;
    [HideInInspector]
    public AudioLowPassFilter audioLowPassFilter;

    private float lastSwitchTime = -100f;
    private float switchTimeLeft;
    private float witchTimeLeft;
    private float lastWitchTime = -100f;
    private float witchTimeTimer;

    private bool isSwitching;
    private bool isInWitchTime = false;

    private int skillList = 0;
    private int cacheSkill = 0;
    private BuffType currentBuff;

    //[CustomLabel("换人动画时间,可能涉及表现层")]
    public float switchTime;   //换人动画时间
    //[CustomLabel("换人冷却")]
    public float switchCoolDown;  //换人冷却
    //[CustomLabel("角色最大血量")]
    public int playerMaxHp = 10;
    //[CustomLabel("角色当前血量")]
    public int playerHp;
    //[CustomLabel("角色攻击力（目前还未接入）")]
    public int playerAttack = 1;
    //[CustomLabel("读入的dialog编号")]
    public string DialogDir = Consts.dia_0;
    //[CustomLabel("冲刺中魔女时间检测时间")] 
    public float checkWitchTime;
    //[CustomLabel("魔女时间冷却cd")] 
    public float witchTimeCoolDown;    
    //[CustomLabel("魔女时间计算倍率")] 
    public float witchTimeMagnification = 20f;    
    //[CustomLabel("魔女时间计算偏移")] 
    public float witchTimeOffset = 1f;
    //[CustomLabel("魔女时间时缓倍率")] 
    public float witchTimeSlowTimeMagnification = 1f;

    public int PlayerHp
    {
        get
        {
            return playerHp;
        }
        set
        {
            if (characterControllers[controlCharacterIndex].isDead) return;
            if (value > playerMaxHp) playerHp = PlayerMaxHp;
            else if (value < 0) playerHp = 0;
            else playerHp = value;
            uiBattle.SetHp(playerHp);
            if(playerHp == 1) Game.Instance.CameraManager.Endangered(true);
            else Game.Instance.CameraManager.Endangered(false);
        }
    }

    public int PlayerMaxHp
    {
        get => playerMaxHp;
        set
        {
            if (value <= 0) return;
            playerMaxHp = value;
            uiBattle.SetHpMax(value);
        }
    }

    private void Start()
    {
        //希望在隐藏时也能响应事件
        unlockPower = unlockPowerApply;
        unlockItem = unlockItemApply;
        archive = archiveApplay;
        accelerateSkillBuffAcyion = ChangeSpeed;
        buttonDown = OnButtonDown;
        Game.Instance.EventCenter.AddEventListener(Consts.E_BUFF_Accelerate, accelerateSkillBuffAcyion);
        Game.Instance.EventCenter.AddEventListener(Consts.C_RewindCharacter, Rewind);
        Game.Instance.EventCenter.AddEventListener(Consts.P_UNLOCK, unlockPower);
        Game.Instance.EventCenter.AddEventListener(Consts.P_NORMAL, unlockItem);
        Game.Instance.EventCenter.AddEventListener<SaveDataArgs>(Consts.S_SAVE, archiveApplay);
        Game.Instance.EventCenter.AddEventListener(Consts.M_Button_Down, buttonDown);
    }

    private void Update()
    {
        CheckInput();
        CheckSwitch();
        CheckSkill();
        CheckUltimateSkill();
        SetBattleUI();
        RefreshBuff();
        CheckWitchTime();
    }

    public bool Init()
    {
        if (characterControllers.Count != 0) return true;

        sm = GetComponent<SkillManager>();
        cc = GetComponent<CDController>();
        buff = this.transform.Find("Buff").gameObject;
        audioLowPassFilter = buff.AddComponent<AudioLowPassFilter>();
        audioLowPassFilter.cutoffFrequency = 500f;
        audioLowPassFilter.enabled = false;
        SaveableRegister();

        int index = 0;

        GameObject characterEntity = Game.Instance.AssetsManager.Load<GameObject>("Prefabs/Player/Character95");
        if (characterEntity == null) return false;
        RealCharacterController characterController = characterEntity.GetComponent<RealCharacterController>();
        characterController.Init(this, cc, index++);
        characterControllers.Add(characterController);
        characterController.transform.parent = this.transform;
        characterController.transform.localPosition = new Vector3(0f, 0f, 0f);
        Game.Instance.CameraManager.PlayerCinemachine.Follow = characterController.transform;
        //Game.Instance.CameraManager.OtherCinemachine.Follow = characterController.transform;
        //Game.Instance.CameraManager.followTarget = buff.transform;
        
        characterEntity = Game.Instance.AssetsManager.Load<GameObject>("Prefabs/Player/Character97");
        if (characterEntity == null) return false;
        characterEntity.SetActive(false);
        characterController = characterEntity.GetComponent<RealCharacterController>();
        characterController.Init(this,cc, index++);
        characterController.transform.parent = this.transform;
        characterController.transform.localPosition = new Vector3(0f, 0f, 0f);
        characterControllers.Add(characterController);
        
        if(Game.Instance.SaveSystem.currentSave.characString != characterControllers[controlCharacterIndex].characName)
            InnerSwitch();
        PlayerHp = PlayerMaxHp;

        return true;
    }

    private void RefreshBuff()
    {
        buff.transform.position = new Vector3(characterControllers[controlCharacterIndex].transform.position.x,
            characterControllers[controlCharacterIndex].transform.position.y+0.5f);
        characterControllers[1 - controlCharacterIndex].transform.position =
            characterControllers[controlCharacterIndex].transform.position;
    }

    public void RefreshCurrentCharacter()
    {
        if(Game.Instance.SaveSystem.currentSave.characString != characterControllers[controlCharacterIndex].characName)
            InnerSwitch();
    }

    public void InitUI(UIBattle battle)
    {
        uiBattle = battle;
        uiBattle.Init();
        uiBattle.SetHpMax(PlayerMaxHp);
        uiBattle.SetHp(PlayerHp);
        uiBattle.SwitchSprite(new SwitchCharacterArgs()
        {
            CharacId = controlCharacterIndex,
            characSkillName = "Skill" + controlCharacterIndex.ToString()
        });
        uiBattle.SkillLaunched(new SkillLaunchedArgs()
        {
            skillList = 0, 
            cacheSkill = 0
        });
    }

    private void SetBattleUI()
    {
        //Debug.Log("Set UI");
        //float cdLeft = characterControllers[controlCharacterIndex].sm.Skills[0].skillCD;
        //float cd = characterControllers[controlCharacterIndex].sm.skillDic[characterControllers[controlCharacterIndex].sm.Skills[0].skillID].skillCD;
        //Debug.Log("Set Battle UI:cdLeft=" + cdLeft + ",cd=" + cd);
        //uiBattle.SetSkillCd(cdLeft / cd);
    }

    private void CheckInput()
    {
        if (Game.Instance.InputManager.inputState != InputState.Gaming) return;
        
        if (Game.Instance.InputManager.SwitchPress)
        {
            AttempToSwitchCharacter();
        }

        if (Game.Instance.InputManager.AttackPress)
        {
            AttempToUltimateSkill();
        }

        if (Game.Instance.InputManager.HelpPress)
        {
            Game.Instance.UIManager.currentLayer = EUILayer.Back;
            UICallType e = new UICallType();
            e.callUI = Consts.UIBattle;
            Game.Instance.TimeManager.ResetBulletTime();
            Game.Instance.UIManager.ShowPanel<UIHelper>(Consts.UIHelper, e, EUILayer.Back, null, true, null);
        }

        if (Game.Instance.InputManager.EscPress && Game.Instance.UIManager.currentLayer == EUILayer.Battle)
        {
            OnPausePress();
        }

        if(Game.Instance.InputManager.BackpackPress && Game.Instance.UIManager.currentLayer == EUILayer.Battle)
        {
            OnBackPackPress();
        }
    }

    private void OnPausePress()
    {
        Game.Instance.UIManager.currentLayer = EUILayer.Back;
        UIPauseArgs e = new UIPauseArgs();
        e.callLayer = EUILayer.Battle;
        Game.Instance.TimeManager.ResetBulletTime();
        Game.Instance.UIManager.ShowPanel<UIPause>(Consts.UIPause, e, EUILayer.Back, null, true, null);
    }

    private void OnBackPackPress()
    {
        Game.Instance.UIManager.currentLayer = EUILayer.Mid;
        Game.Instance.UIManager.ShowPanel<UIBackpack>(Consts.UIBackpackPanel, null, EUILayer.Mid, null, true, null);
    }
    
    private void AttempToSwitchCharacter()
    {
        if (Time.time >= (lastSwitchTime + switchCoolDown))
        {
            //滑墙时和技能时不能切换角色
            if (!characterControllers[controlCharacterIndex].isWallSliding && !characterControllers[controlCharacterIndex].isSkilling && 
                !characterControllers[controlCharacterIndex].isDashing && !characterControllers[controlCharacterIndex].isHurting && 
                !characterControllers[controlCharacterIndex].isDead && Game.Instance.SaveSystem.currentSave.dialog >= 1)
            {
                isSwitching = true;
                switchTimeLeft = switchTime;
                lastSwitchTime = Time.time;

                characterControllers[controlCharacterIndex].character.SetActive(false);
                controlCharacterIndex = 1 - controlCharacterIndex;

                characterControllers[controlCharacterIndex].character.SetActive(true);
                characterControllers[controlCharacterIndex].transform.localPosition =
                    characterControllers[1 - controlCharacterIndex].transform.localPosition;
                if (characterControllers[controlCharacterIndex].isFacingRight !=
                    characterControllers[1 - controlCharacterIndex].isFacingRight)
                {
                    characterControllers[controlCharacterIndex].Flip();
                }

                characterControllers[controlCharacterIndex].transform.GetComponent<Rigidbody2D>().velocity =
                    characterControllers[1 - controlCharacterIndex].transform.GetComponent<Rigidbody2D>().velocity;
                characterControllers[controlCharacterIndex].amountOfJumpsLeft =
                    characterControllers[1 - controlCharacterIndex].amountOfJumpsLeft;
                Game.Instance.CameraManager.PlayerCinemachine.Follow =
                    characterControllers[controlCharacterIndex].transform;
                //Game.Instance.CameraManager.OtherCinemachine.Follow = characterControllers[controlCharacterIndex].transform;

                Game.Instance.MusicManager.PlaySound("Flash", characterControllers[controlCharacterIndex].gameObject,
                    false, 1f);

                //ToDo:换人特效生成
                EffectManager.Instance.AddWorldEffect2D("ChangeEffect",
                    characterControllers[controlCharacterIndex].transform.position);

                //换人事件的广播
                SwitchCharacterArgs e = new SwitchCharacterArgs();
                e.CharacId = controlCharacterIndex;
                e.characSkillName = "Skill" + controlCharacterIndex.ToString();
                Game.Instance.EventCenter.EventTrigger<SwitchCharacterArgs>(Consts.C_SwitchCharacter, e);
            }
        }
    }

    private void InnerSwitch()
    {
        characterControllers[controlCharacterIndex].character.SetActive(false);
        controlCharacterIndex = 1 - controlCharacterIndex;

        characterControllers[controlCharacterIndex].character.SetActive(true);
        characterControllers[controlCharacterIndex].transform.localPosition =
            characterControllers[1 - controlCharacterIndex].transform.localPosition;
        if (characterControllers[controlCharacterIndex].isFacingRight !=
            characterControllers[1 - controlCharacterIndex].isFacingRight)
        {
            characterControllers[controlCharacterIndex].Flip();
        }
        characterControllers[controlCharacterIndex].transform.GetComponent<Rigidbody2D>().velocity=
            characterControllers[1 - controlCharacterIndex].transform.GetComponent<Rigidbody2D>().velocity;
        characterControllers[controlCharacterIndex].amountOfJumpsLeft =
            characterControllers[1 - controlCharacterIndex].amountOfJumpsLeft;
        Game.Instance.CameraManager.PlayerCinemachine.Follow = characterControllers[controlCharacterIndex].transform;
        //Game.Instance.CameraManager.OtherCinemachine.Follow = characterControllers[controlCharacterIndex].transform;
    }
    
    private void CheckSwitch()
    {
        if (isSwitching)
        {
            if (switchTimeLeft > 0)
            {
                switchTimeLeft -= Time.deltaTime;
            }

            if (switchTimeLeft <= 0)
            {
                isSwitching = false;
            }
        }

        if (uiBattle != null)
        {
            uiBattle.SetSwitchCd((Time.time - lastSwitchTime) > switchCoolDown
                ? 1f
                : (Time.time - lastSwitchTime) / switchCoolDown);
        }
    }

    public void ChangeSpeed(SkillBuffArgs args)
    {
        if (args.buffType == BuffType.Accelerate)
        {
            if (args.buffChange >= 1)
            {
                characterControllers[controlCharacterIndex].movementSpeed =
                    characterControllers[controlCharacterIndex].initMoveSpeed * args.buffChange;
                characterControllers[controlCharacterIndex].SetAnimSpeed("move", 0, args.buffChange);
                characterControllers[controlCharacterIndex].aie.SetEnable(true);
                characterControllers[1-controlCharacterIndex].movementSpeed =
                    characterControllers[1-controlCharacterIndex].initMoveSpeed * args.buffChange;
                characterControllers[1-controlCharacterIndex].SetAnimSpeed("move", 0, args.buffChange);
                characterControllers[1-controlCharacterIndex].aie.SetEnable(true);
            }
            else
            {
                characterControllers[controlCharacterIndex].movementSpeed =
                    characterControllers[controlCharacterIndex].initMoveSpeed;
                characterControllers[controlCharacterIndex].ResetAllAnimSpeed();
                characterControllers[controlCharacterIndex].aie.SetEnable(false);
                characterControllers[1-controlCharacterIndex].movementSpeed =
                    characterControllers[1-controlCharacterIndex].initMoveSpeed;
                characterControllers[1-controlCharacterIndex].ResetAllAnimSpeed();
                characterControllers[1-controlCharacterIndex].aie.SetEnable(false);
            }
        }
    }


    public void TriggerWitchTime()
    {
        if (characterControllers[controlCharacterIndex].isDashing && 
            characterControllers[controlCharacterIndex].dashTimeLeft > characterControllers[controlCharacterIndex].dashTime - checkWitchTime &&
            Time.time > lastSwitchTime + witchTimeCoolDown)
        {
            Debug.Log(characterControllers[controlCharacterIndex].dashTimeLeft);
            
            witchTimeTimer = (characterControllers[controlCharacterIndex].dashTimeLeft-checkWitchTime) * witchTimeMagnification +
                             witchTimeOffset;
            Debug.Log("witchTime:" + witchTimeTimer);
            lastSwitchTime = Time.time;
            isInWitchTime = true;
            Game.Instance.MusicManager.PlaySound("StartSlowTime", buff.gameObject, false);
            Game.Instance.TimeManager.SlowTime(witchTimeSlowTimeMagnification);
        }
    }

    private void CheckWitchTime()
    {
        if (isInWitchTime)
        {
            if (witchTimeTimer > 0)
            {
                witchTimeTimer -= Time.deltaTime;
                Debug.Log("witching:" + witchTimeTimer);
            }
            else
            {
                isInWitchTime = false;
                Game.Instance.TimeManager.ContinueTime();
            }
        }
    }
    
    
    public void GetHurt(int damage)
    {
        PlayerHp -= damage;
        Game.Instance.MusicManager.PlaySound("OnHit", buff.gameObject, false);
        Game.Instance.TimeManager.SetBulletTime(SlowType.TimeScaleHurt);
        if (PlayerHp <= 0)
        {
            StartCoroutine(DeadEvent());
        }
    }

    public void OnSkill(int characterIndex)
    {
        skillList = (skillList << 1) + characterIndex;
        skillList = skillList & 7;
        if(cacheSkill < 3) cacheSkill++;
        SkillLaunchedArgs e = new SkillLaunchedArgs();
        float facingRight = characterControllers[controlCharacterIndex].isFacingRight ? 1f : -1f;
        Vector2 direct = new Vector2(characterControllers[controlCharacterIndex].skillDirection == 0 ? facingRight : 0f,
            characterControllers[controlCharacterIndex].skillDirection);
        e.skillList = skillList;
        e.cacheSkill = cacheSkill;
        e.skillId = characterControllers[controlCharacterIndex].sm.Skills[0].skillID;
        e.director = direct;
        Game.Instance.EventCenter.EventTrigger(Consts.C_SkillLaunched, e);
    }

    private void AttempToUltimateSkill()
    {
        if (cacheSkill == 3)
        {
            int skillIndex = 0;

            if ((skillList & 1) == 1) skillIndex++;
            if ((skillList & 2) == 2) skillIndex++;
            if ((skillList & 4) == 4) skillIndex++;

            switch (skillIndex)
            {
                case 0:
                    characterControllers[controlCharacterIndex].UltimateSkillStart(101, buff.transform);
                    Game.Instance.MusicManager.PlaySound("StartSpeed", buff.gameObject, false);
                    currentBuff = BuffType.Accelerate;
                    break;
                case 1:
                    characterControllers[controlCharacterIndex].UltimateSkillStart(102, buff.transform);
                    Game.Instance.MusicManager.PlaySound("StartShield", buff.gameObject, false);
                    currentBuff = BuffType.Shield;
                    break;
                case 2:
                    characterControllers[controlCharacterIndex].UltimateSkillStart(103, buff.transform);
                    Game.Instance.MusicManager.PlaySound("StartHot", buff.gameObject, false);
                    currentBuff = BuffType.RifleShootSpeed;
                    break;
                case 3:
                    characterControllers[controlCharacterIndex].UltimateSkillStart(104, buff.transform);
                    Game.Instance.MusicManager.PlaySound("StartBoom", buff.gameObject, false);
                    currentBuff = BuffType.FullScreenBomb;
                    break;
            }
            cacheSkill = 0;
            skillList = skillList & 0;
            
            uiBattle.SkillLaunched(new SkillLaunchedArgs()
            {
                skillList = skillList,
                cacheSkill = cacheSkill
            });
        }
    }

    private void CheckSkill()
    {
        if (uiBattle != null && cc != null && characterControllers.Count > 0)
        {
            if (characterControllers[controlCharacterIndex].sm != null)
            {
                uiBattle.SetSkillCd(cc.GetNormalCDPer(characterControllers[controlCharacterIndex].sm.Skills[0].skillID));
            }
        }
    }
    
    private void CheckUltimateSkill()
    {
        uiBattle.SetUltimateSkillCd(cc.GetBuffCDPre(currentBuff));
    }

    public void SetCharacterPosition(Vector3 position)
    {
        characterControllers[controlCharacterIndex].transform.position = position;
        characterControllers[1-controlCharacterIndex].transform.position = position;
    }
    
    private IEnumerator DeadEvent()
    {
        characterControllers[controlCharacterIndex].isDead = true;
        characterControllers[1-controlCharacterIndex].isDead = true;
        
        Game.Instance.InputManager.canControl = false;
        yield return new WaitForSecondsRealtime(2f);
        Game.Instance.InputManager.canControl = true;

        Game.Instance.UIManager.currentLayer = EUILayer.Back;
        Game.Instance.UIManager.ShowPanel<UIDead>(Consts.UIDead, null, EUILayer.Back, null, true, null);
    }


    public void Rewind()
    {
        characterControllers[controlCharacterIndex].isDead = false;
        characterControllers[1-controlCharacterIndex].isDead = false;
        if (PlayerHp <= 0)
        {
            PlayerHp = PlayerMaxHp;
        }
        characterControllers[controlCharacterIndex].ResetAllAnimSpeed();
        characterControllers[1-controlCharacterIndex].ResetAllAnimSpeed();

        cacheSkill = 0;
        skillList = skillList & 0;
            
        uiBattle.SkillLaunched(new SkillLaunchedArgs()
        {
            skillList = skillList,
            cacheSkill = cacheSkill
        });
        
        //Todo:设置回溯的父对象位置
        var position = new Vector3(Game.Instance.SaveSystem.currentSave.SavePointX, Game.Instance.SaveSystem.currentSave.SavePointY, Game.Instance.SaveSystem.currentSave.SavePointZ);
        this.transform.position = position;
        
        SetCharacterPosition(position);
    }

    public void unlockPowerApply(PropPickUp data)
    {
        if (data.itemData.itemType == ItemType.Unlock)
        {
            if (!Game.Instance.SaveSystem.currentSave.getedSkill)
            {
                Game.Instance.SaveSystem.currentSave.getedSkill = true;
                characterControllers[controlCharacterIndex].amountOfJumps = 2;
                characterControllers[1-controlCharacterIndex].amountOfJumps = 2;
                characterControllers[controlCharacterIndex].getSlideWall = true;
                characterControllers[1-controlCharacterIndex].getSlideWall = true;
            }
        }
    }
    
    public void unlockItemApply(PropPickUp data)
    {
        if (data.itemData.itemType == ItemType.Normal)
        {
            Game.Instance.SaveSystem.currentSave.getedGoods |= (1 << data.itemData.itemID);
        }
    }

    public void archiveApplay(SaveDataArgs data)
    {
        Debug.Log("Save position:" + data.rivivePos);
        Game.Instance.SaveSystem.currentSave.level = data.sceneName;
        Game.Instance.SaveSystem.currentSave.SavePointX = data.rivivePos.x;
        Game.Instance.SaveSystem.currentSave.SavePointY = data.rivivePos.y;
        Game.Instance.SaveSystem.currentSave.SavePointZ = data.rivivePos.z;
        Game.Instance.SaveSystem.currentSave.characString = characterControllers[controlCharacterIndex].characName;
        Game.Instance.EventCenter.EventTrigger(Consts.E_SaveData);
    }
    
    public void SaveableRegister()
    {
        Game.Instance.SaveSystem.RegisterSaveable(this);
    }

    public void SaveData()
    {
        Game.Instance.SaveSystem.currentSave.playerMaxHp = PlayerMaxHp;
        Game.Instance.SaveSystem.currentSave.characString = characterControllers[controlCharacterIndex].characName;
        Debug.Log(characterControllers[controlCharacterIndex].characName);
    }

    public void ChangePlayerLayer(bool invincible)
    {
        if (invincible)
        {
            ChangeLayer(characterControllers[controlCharacterIndex].transform, LayerMask.NameToLayer("InvinciblePlayer"));
            ChangeLayer(characterControllers[1-controlCharacterIndex].transform, LayerMask.NameToLayer("InvinciblePlayer"));
        }
        else
        {
            ChangeLayer(characterControllers[controlCharacterIndex].transform, LayerMask.NameToLayer("Player"));
            ChangeLayer(characterControllers[1-controlCharacterIndex].transform, LayerMask.NameToLayer("Player"));
        }
    }
    
    public void LoadData()
    {
        skillList = 0;
        cacheSkill = 0;
        PlayerMaxHp = Game.Instance.SaveSystem.currentSave.playerMaxHp;
        PlayerHp = PlayerMaxHp;

        if (Game.Instance.SaveSystem.currentSave.getedSkill)
        {
            characterControllers[controlCharacterIndex].amountOfJumps = 2;
            characterControllers[1-controlCharacterIndex].amountOfJumps = 2;
            characterControllers[controlCharacterIndex].getSlideWall = true;
            characterControllers[1-controlCharacterIndex].getSlideWall = true;
        }
        else
        {
            characterControllers[controlCharacterIndex].amountOfJumps = 1;
            characterControllers[1-controlCharacterIndex].amountOfJumps = 1;
            characterControllers[controlCharacterIndex].getSlideWall = false;
            characterControllers[1-controlCharacterIndex].getSlideWall = false;
        }
    }

    private void ChangeLayer(Transform transform, int layer)
    {
        if (transform.childCount>0)//如果子物体存在
        {
            for (int i = 0; i < transform.childCount; i++)//遍历子物体是否还有子物体
            {
                ChangeLayer(transform.GetChild(i), layer);//这里是只将最后一个无子物体的对象设置层级
            }
            transform.gameObject.layer = layer;//将存在的子物体遍历结束后需要把当前子物体节点进行层级设置
        }
        else					//无子物体
        {
            transform.gameObject.layer = layer;
        }
    }

    private void OnButtonDown(UIButtonArgs args)
    {
        if (args.currentUI == Consts.UIPhoneBattle)
        {
            switch (args.actionType)
            {
                case ActionType.Backpack:
                    OnBackPackPress();
                    break;
                case ActionType.Esc:
                    OnPausePress();
                    break;
                case ActionType.Switch:
                    AttempToSwitchCharacter();
                    break;
                case ActionType.Attack:
                    AttempToUltimateSkill();
                    break;
            }
        }
    }
}

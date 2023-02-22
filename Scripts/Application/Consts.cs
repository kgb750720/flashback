using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Consts
{
    //目录路径
    //public static readonly string Dir = Application.dataPath + "/Resources";
    
    //存档
    //public const string SaveDir = ;
    
    //Dialog事件
    public const string E_TalkCompleted = "E_TalkCompleted";
    
    //移动端按键事件
    public const string M_Button_Down = "M_Button_Down";
    public const string M_Button_Up = "M_Button_Up";

    //传送门事件
    public const string T_D_Start = "T_D_Start";
    public const string T_D_Over = "T_D_Over";

    //Controller事件
    public const string E_StartUp = "E_StartUp"; //
    
    public const string E_EnterScene = "E_EnterScene"; //进入场景
    public const string E_ExitScene = "E_ExitScene"; //退出场景
    public const string E_SaveData = "E_SaveData"; //保存
    public const string E_LoadData = "E_LoadData"; //读入

    //角色事件
    public const string C_SwitchCharacter = "C_SwitchCharacter"; //切换角色
    public const string C_OnSkillEnd = "C_OnSkillEnd"; //角色释放完技能
    public const string C_SkillLaunched = "C_SkillLaunched"; //角色技能发动事件
    public const string C_RewindCharacter = "C_RewindCharacter"; //角色死亡回溯（复活）
    
    //动画事件
    public const string E_StartShoot = "E_StartShoot"; //角色射击

    //技能BUFF事件
    public const string E_BUFF_BulletNum = "ShotGunNum";
    public const string E_BUFF_FireSpeed = "RifleShootSpeed";
    public const string E_BUFF_Accelerate = "Accelerate";

    public const string E_BUFF_TimeSlow = "E_BUFF_TimeSlow";
    public const string E_BUFF_TimeNormal = "E_BUFF_TimeNormal";

    //道具事件
    public const string P_UNLOCK = "P_UNLOCK";  //道具解锁特殊能力
    public const string P_NORMAL = "P_NORMAL";  //拾取道具
    //按键事件

    //场景事件
    public const string S_SAVE = "S_SAVE";  //存档事件

    //Boss死亡事件
    public const string B_BossDeath = "B_BossDeath";

    //资产路径
    public const string A_BulletPrefab = "Prefabs/Bullet/BulletTrailRender";
    public const string A_SkillBulletPrefab = "Prefabs/Bullet/SkillBullet";
    public const string A_ShellPrefab = "Prefabs/Bullet/Shell";
    public const string A_FirePrefab = "Prefabs/Bullet/Effect/Muzzle/";
    public const string A_SkillPrefabPath = "Prefabs/Skill/";
    public const string A_Player = "Prefabs/Player/";
    
    //Dialogue 字段
    public const string XmlDir = "Dialogs/Xml/";
    public const string DialogXmlFileName = "Dialogs";
    public const string DialogueSystemPanel = "DialogueSystemPanel";
    public const string DialogSpriteDir = "Image/Dialog/";
    public const string dia_0 = "dia_0";
    public const string dia_1 = "dia_1";
    public const string dia_2 = "dia_2";
    public const string dia_3 = "dia_3";
    public const string dia_4 = "dia_4";
    public const string dia_5 = "dia_5";
    public const string dia_6 = "dia_6";
    
    //Item字段
    public const string ItemFileName = "Items";
    public const string item_0 = "item_0";
    public const string item_1 = "item_1";
    public const string item_2 = "item_2";
    public const string item_3 = "item_3";
    public const string item_4 = "item_4";
    
    //UI字段
    public const string UIDir = "UI/";
    public const string UISpriteDir = "Image/UI/";
    public const string UIStart = "UIStart";
    public const string UILoading = "UILoading";
    public const string UIPause = "UIPause";
    public const string UISetting = "UISetting";
    public const string UIHelper = "UIHelper";
    public const string UIDead = "UIDead";
    public const string UIHeart = "UIHeart";
    public const string Slot = "Slot";
    public const string UIBattle = "UIBattle";
    public const string UIPhoneBattle = "UIPhoneBattle";
    public const string UILoad = "UILoad";
    public const string UIBackpackPanel = "UIBackpackPanel";
    public const string UIDialogue = "UIDialogue";
    public const string UIHealthBar = "UIHealthBar";
    internal static string UIBossHealthBar = "UIBossHealthBar";

    //场景字段
    public const string StartMenu = "1.StartMenu";
    public const string Main = "2.Main";
}

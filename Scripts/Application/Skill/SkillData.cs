using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SkillSystem
{
    public enum SkillType
    { 
        Skill = 1,
        BulletSkill = 2,
        Buff = 3,
    }
    public enum SkillAttackType
    {
        Single = 1,
        Aoe = 2,
    }

    public enum SeletorType
    {
        None = 1,
        Circular = 2,
        Rectangular = 3,
        FullScreen = 4,
        Single = 5,
    }

    public enum DisappearType
    {
        TimeOver = 1,
        CheckOver = 2,
    }

    public enum BuffType
    { 
        RifleShootSpeed = 0,
        ShotGunNum = 1,
        TimeSlow = 2,
        Shield = 3,
        Hot = 4,
        FullScreenBomb = 5,
        Accelerate = 6
    }

    public enum PrefabType
    {
        ParticleSystem = 0,
        SpriteRender = 1,
        None = 2,
    }

    [Serializable]
    public class BUFF
    {
        public bool isBuff;     //标识是BUFF还是DEBUFF
        public float attributeChange;       //属性的改变量
        public float durationTime;          //Buff的持续时间
        public BuffType buffType;       //buff的类型

        [HideInInspector] public float cDRemain;    //单个Buff的cd剩余时间
    }

    [Serializable]
    public class SkillData
    {
        [Header("技能类型")]
        public SkillType skillType;
        [Header("预制体类型")]
        [Tooltip("当类型为粒子特效时只需要找到对应预制体的名字,其他则需要路径")]
        public PrefabType prefabType;
        [Header("技能属性")]
        public int skillID; //技能ID
        public bool isShareCDSkill;  //技能是否参与共享CD
        public float attackDistance; //技能距离
        public float attackInterval;    //伤害间隔
        public float attackCoe;  //伤害系数
        public float durationTime;  //持续时间
        public float skillCD; //技能冷却时间
        public string name;  //技能名称
        public string prefabName;   //技能预制体名称
        public string prefabPath;   //预制体的路径名称
        public GameObject character;        //技能挂载对象
        public SkillAttackType attackType;      //单体/aoe
        public DisappearType disappearType;     //技能预制体消失方式
        public SeletorType seletorType;     //范围类型

        [HideInInspector] public float cDRemain;  //技能剩余时间
        [HideInInspector] public List<Transform> attackTargets;     //能够攻击到的对象

        [Header("BUFF")]
        public BUFF[] buffs;        //一个技能可能影响的BUFF数组
        [Header("影响内容")]
        public string[] impactType = { "Damage" };      //技能可能影响到的内容、BUFF等等
        [Header("目标Tag")]
        public string[] attackTargetTags = { "Enemy" };  //技能攻击的目标Tag数组
    }
}

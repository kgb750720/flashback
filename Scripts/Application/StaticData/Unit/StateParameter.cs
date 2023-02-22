using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DamageAnimType
{
    Shoot,
    Melee,
    Impact
}

[Serializable]
public class StateParameter
{
    //单位的各种参数
    public float health;    //生命值
    public float moveSpeed = 0.1f;    //移动速度
    public float chaseSpeed = 0.1f;    //追逐速度
    public float idleTime = 5.0f;    //停止时间
    public float spawnPatrolRadius = 3f;    //自动生成巡逻路点的范围
    public List<Transform> patrolPoints = new List<Transform>();    //巡逻点
    public float spawnChaseRadius = 5f;     //自动生成追击路点的范围
    public List<Transform> chasePoints = new List<Transform>();    //追击点
    public bool getHit;    //是否受击

    private float m_damage = -1;    //未处理的伤害值(不可配置)
    public float Damage 
    {
        set
        {
            if (value < 0)
            {
                Debug.LogWarning("输入的伤害" + value + "为负数！");
                return;
            }
            if (m_damage < 0)
                m_damage = 0;
            m_damage += value;
            getHit = true;
        }
        get
        {
            float res = m_damage;
            if(m_damage<0)
            {
                Debug.LogWarning("damage变量值为" + m_damage + "异常值，已将damag置为-1，返回值为0");
                res = 0;
            }
            m_damage = -1;
            return res;
        }
    }
    public DamageAnimType damageType = DamageAnimType.Melee;    //伤害动作类型
    public Animator animator;    //动画
    public Transform target;    //目标
    public float lostTargetTime = 2f;   //丢失目标所需要的时长
    public LayerMask targetLayer;    //目标层
    public Transform attackPoint;    //攻击点
    public float attackAnimaTime = 0;    //攻击动画时间（小于等于0时以动画原生速度为准）
    public float attackCfgCD = 2f;   //可配置的攻击CD时间
    private float m_attackIntervalCD = 0;   //攻击CD剩余时间(不可配置)
    public float AttackIntervalCD 
    {
        get { return m_attackIntervalCD; }
        set
        {
            if (value < 0)
                m_attackIntervalCD = 0;
            else
                m_attackIntervalCD = value;
        }
    }
    public float attackArea = 0.5f;    //攻击范围
    public float fallResetTime = 3f;        //掉落传回
}

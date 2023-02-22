using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SkillSystem
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class RemoteSkillDeployer : SkillDeployer    //飞行道具释放器,包含具体的触发策略
    {
        public float speed = 2F;
        private float startTime;

        private void Awake()
        {
            startTime = Time.time;
        }
        private void OnEnable()     //和对象池的联动，清洗数据
        {
            startTime = Time.time;
        }
        public override void DeploySkill() {}

        private void GetTarget(Transform _enemy)
        {
            skillData.attackTargets = null;
            skillData.attackTargets.Add(_enemy);
        }

        public void OnTriggerEnter2D(Collider2D collision)
        {
            if (skillData.attackTargetTags.Length != 0)
            {
                for (int i = 0; i < skillData.attackTargetTags.Length; ++i)
                {
                    if (collision.tag == skillData.attackTargetTags[i])
                    {
                        //在碰撞后开始执行释放器内容,CalculateTargets对于范围类的要求需要进入Update
                        CalculateTargets();
                        ImpactTargets();
                        break;
                    }
                }
            }
            else
                Debug.LogError("SkillData没有设置可影响Tags");
        }

        private void Update()
        {
            //SkeletonMecanim muzzle = skillData.charecter.GetComponent<SkeletonMecanim>();
            //Vector3 worldPosition = muzzle.skeleton.FindBone("muzzle").GetWorldPosition(muzzle.transform);

            //移动方式要重写，要求更高的拓展性
            transform.Translate(new Vector3(1,0,0) * speed * Time.deltaTime);

            if (speed * (Time.time - startTime) >= skillData.attackDistance)
            {
                Game.Instance.ObjectPool.RecycleObject(gameObject);
            }
        }
    }
}
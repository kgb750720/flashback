using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SkillSystem 
{
    public abstract class SkillDeployer : MonoBehaviour
    {
        private SkillData _skillDate;
        public SkillData skillData
        {
            get
            {
                return _skillDate;
            }
            set
            {
                _skillDate = value;
                InitDeployer();
            }
        }

        private IAttackSeletor seletor;
        private IImpactEffect[] impactEffectArray;

        public void CalculateTargets()
        {
            if (seletor == null) return;

            skillData.attackTargets = seletor.SelectorTarget(skillData, this.transform);    //第二个参数是技能的作用位置,也就是子类所挂载的物体的位置
        }

        public void ImpactTargets()
        {
            if (impactEffectArray == null) return;

            for (int i = 0; i < impactEffectArray.Length; ++i)
            {
                impactEffectArray[i].Execute(this);
            }
        }

        private void InitDeployer()
        {
            //范围选择
            seletor = DeployerConfigFactory.CreateSkillSelector(skillData);
            //效果
            impactEffectArray = DeployerConfigFactory.CreateImpactEffects(skillData);
        }

        public abstract void DeploySkill();     //具体策略可以由子类进行实现,即时策略
    }
}
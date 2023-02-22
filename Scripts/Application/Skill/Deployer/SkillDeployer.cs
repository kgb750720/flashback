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

            skillData.attackTargets = seletor.SelectorTarget(skillData, this.transform);    //�ڶ��������Ǽ��ܵ�����λ��,Ҳ�������������ص������λ��
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
            //��Χѡ��
            seletor = DeployerConfigFactory.CreateSkillSelector(skillData);
            //Ч��
            impactEffectArray = DeployerConfigFactory.CreateImpactEffects(skillData);
        }

        public abstract void DeploySkill();     //������Կ������������ʵ��,��ʱ����
    }
}
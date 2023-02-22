using System;
using UnityEngine;

namespace SkillSystem
{
    public class DeployerConfigFactory : MonoBehaviour
    {
        public static IAttackSeletor CreateSkillSelector(SkillData _skillData)
        {
            string className = string.Format("SkillSystem.{0}SkillSelector", _skillData.seletorType);    //ʹ�������ҵ���Ӧ��ʵ���ಢ����

            return CreateObject<IAttackSeletor>(className);
        }

        public static IImpactEffect[] CreateImpactEffects(SkillData _skillData)
        {
            IImpactEffect[] impacts = new IImpactEffect[_skillData.impactType.Length];

            for (int i = 0; i < _skillData.impactType.Length; ++i)
            {
                string className = string.Format("SkillSystem.{0}Impact", _skillData.impactType[i]);
                impacts[i] = CreateObject<IImpactEffect>(className);    //ÿһ����Ҫ����Ķ���Ӧ��ȥʵ�ֽӿ�
            }

            return impacts;
        }

        //if (type.IsSubclassOf(typeof(MonoBehaviour))) return FindObjectOfType(type) as T;
        private static T CreateObject<T>(string _className) where T : class
        {
            Type type = Type.GetType(_className);
            return Activator.CreateInstance(type) as T;
        }
    }
}

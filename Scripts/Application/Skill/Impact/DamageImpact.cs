using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SkillSystem
{
    public class DamageImpact : IImpactEffect
    {
        private SkillData data;
        private bool isOnce;
        public void Execute(SkillDeployer _deployer)
        {
            if (isOnce) return;

            data = _deployer.skillData;
            isOnce = true;

            switch (data.disappearType)
            {
                case DisappearType.CheckOver:
                    Damage(_deployer);
                    break;
                case DisappearType.TimeOver://针对如同漩涡技能可能添加的伤害的情况
                    _deployer.StartCoroutine(DamageContinue(_deployer));
                    break;
                default:
                    break;
            }
        }
        private void Damage(SkillDeployer _deployer)
        {
            if (data.attackType == SkillAttackType.Single)  //对应只影响单个敌人
            {
                if (data.attackTargets.Count != 1)
                    Debug.LogError("攻击目标对象数量与攻击类型不符");

                if (data.attackTargets[0].gameObject == null)
                {
                    data.attackTargets.RemoveAt(0);
                }
                else
                {
                    DamageTarget(data.attackTargets[0]);
                }
            }
            else if (data.attackType == SkillAttackType.Aoe)
            {
                if (data.attackTargets.Count == 0)
                {
                    Debug.Log("没有敌人");
                    return;
                }
                for (int i = 0; i < data.attackTargets.Count; ++i)
                {
                    if (data.attackTargets[i] == null)
                    {
                        data.attackTargets.RemoveAt(i);
                    }
                    else
                    {
                        DamageTarget(data.attackTargets[i]);
                    }
                }
            }
        }

        private void DamageTarget(Transform _trans)
        {
            if (_trans.gameObject == null) return;

            if (_trans.tag == "Player")
            {
                if (_trans.GetComponent<ShieldDeployer>() != null && _trans.GetComponent<ShieldDeployer>().shield != 0)
                {

                }

                int index = 0;
                if (_trans.rotation.y == 0)
                    index = 1;
                else
                    index = -1;

                _trans.GetComponent<RealCharacterController>().Hurt(index, 1);
            }
            else if (_trans.tag == "Enemy")
            {
                if (_trans.GetComponent<FSM>() != null)
                {
                    _trans.GetComponent<FSM>().parameter.Damage = (data.attackCoe * data.character.GetComponentInParent<PlayerManager>().playerAttack);
                }
            }
            else if (_trans.tag == "CanDestroy")
            {
                if (_trans.GetComponent<InteractiveSystem.CanDestroy>() != null)
                {
                    _trans.GetComponent<InteractiveSystem.CanDestroy>().BeHurt((int)(data.attackCoe * data.character.GetComponentInParent<PlayerManager>().playerAttack));

                }
            }
        }

        private IEnumerator DamageContinue(SkillDeployer _deployer)
        {
            float time = 0F;
            do
            {
                yield return new WaitForSeconds(_deployer.skillData.attackInterval);

                time += _deployer.skillData.attackInterval;
                Damage(_deployer);
            }
            while (time < _deployer.skillData.durationTime);
        }
    }
}

using System;
using System.Collections;
using UnityEngine;

namespace SkillSystem
{
    public class BuffImpact : IImpactEffect
    {
        SkillData data;
        public void Execute(SkillDeployer _deployer)
        {
            data = _deployer.skillData;

            if (data.buffs != null)
            {
                for (int i = 0; i < data.buffs.Length; ++i)
                {
                    BUFF buff = data.buffs[i];

                    SkillBuffArgs sb = new SkillBuffArgs();
                    sb.buffChange = (buff.isBuff == true ? 1 : -1) * buff.attributeChange;
                    sb.buffType = buff.buffType;

                    string buffName = Enum.GetName(typeof(BuffType), buff.buffType);
                    Game.Instance.EventCenter.EventTrigger(buffName, sb);
                    data.character.GetComponentInParent<CDController>().AddBuffCD(buff, SendOppositeEvent);
                }
            }
        }

        private void SendOppositeEvent(BUFF _buff)
        {
            SkillBuffArgs sb = new SkillBuffArgs();
            sb.buffChange = (_buff.isBuff == false ? 1 : -1) * _buff.attributeChange;
            sb.buffType = _buff.buffType;

            string buffName = Enum.GetName(typeof(BuffType), _buff.buffType);

            Game.Instance.EventCenter.EventTrigger(buffName, sb);
        }
    }
}
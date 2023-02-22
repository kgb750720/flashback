using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SkillSystem
{
    public class HotImpact : IImpactEffect
    {
        SkillData data;
        public void Execute(SkillDeployer _deployer)
        {
            data = _deployer.skillData;

            BUFF buff = new BUFF();
            buff.durationTime = data.skillCD;
            buff.buffType = BuffType.Hot;

            MusicManager.Instance.PlaySound("Hot",null,false);
            data.character.GetComponentInParent<CDController>().AddBuffCD(buff,null);
        }
    }
}

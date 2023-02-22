using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SkillSystem
{
    public class ChangeSpeedImpact : IImpactEffect
    {
        SkillData data;
        public void Execute(SkillDeployer _deployer)
        {
            data = _deployer.skillData;
            //data.charecter.GetComponent<PlayerController>().speed += data.ChangeSpeed;

            //Game.Instance.EventCenter.EventTrigger();
        }
    }
}
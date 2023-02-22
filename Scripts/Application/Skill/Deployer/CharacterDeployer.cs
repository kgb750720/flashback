using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SkillSystem
{
    public class CharacterDeployer : SkillDeployer
    {
        public override void DeploySkill()
        {
            ImpactTargets();
        }
    }
}
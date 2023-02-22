using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SkillSystem
{
    public class JustlDeployer : SkillDeployer
    {
        public override void DeploySkill()
        {
            CalculateTargets();
            ImpactTargets();
        }
    }
}

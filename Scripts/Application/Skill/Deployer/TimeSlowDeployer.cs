using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SkillSystem
{
    public class TimeSlowDeployer : SkillDeployer
    {
        public override void DeploySkill()
        {
            ImpactTargets();
        }
    }
}

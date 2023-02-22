using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SkillSystem
{
    public class BuffDeployer : SkillDeployer
    {
        public override void DeploySkill()
        {
            ImpactTargets();
        }
    }
}
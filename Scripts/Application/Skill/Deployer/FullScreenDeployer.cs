using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SkillSystem
{
    public class FullScreenDeployer : SkillDeployer
    {
        public float shackTime = 5F;

        public override void DeploySkill()
        {
            CalculateTargets();
            ImpactTargets();

            DoShark();
            PlayerAudio();
        }

        private void DoShark()
        {
            Game.Instance.CameraManager.DoShake(ShakeType.CameraUltBoomNoise, shackTime);
        }

        private void PlayerAudio()
        {
            MusicManager.Instance.PlaySound("Boom",gameObject,false);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SkillSystem
{
    public class TimeSlowImpact : IImpactEffect
    {
        SkillData data;

        public void Execute(SkillDeployer _deployer)
        {
            data = _deployer.skillData;

            BUFF buff = new BUFF();
            buff.durationTime = data.durationTime;
            buff.buffType = BuffType.TimeSlow;
            
            MusicManager.Instance.PlaySound("SlowTime", null, false);
            TimeManager.Instance.SlowTime(_deployer.skillData.attackCoe,_deployer.transform.parent);
            data.character.GetComponentInParent<CDController>().AddBuffCD(buff, SlowTimeCallBack);
        }

        private void SlowTimeCallBack(BUFF _buff)
        {
            TimeManager.Instance.ContinueTime();
        }
    }
}

namespace SkillSystem
{
    public class ShieldImpact : IImpactEffect
    {
        SkillData data;
        SkillDeployer deployer;

        public void Execute(SkillDeployer _deployer)
        {
            data = _deployer.skillData;
            deployer = _deployer;

            BUFF buff = new BUFF();
            buff.durationTime = data.skillCD;
            buff.buffType = BuffType.Shield;

            data.character.GetComponentInParent<CDController>().AddBuffCD(buff, RecycObjectShield);
            //变更挂载对象的音效,并暂存原有对象,使用回调
            //变更挂载对象的受击特效,并暂存原有对象,使用回调
        }

        private void RecycObjectShield(BUFF buff)
        {
            ObjectPool.Instance.RecycleObject(deployer.gameObject);
        }
    }
}
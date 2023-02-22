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
            //������ض������Ч,���ݴ�ԭ�ж���,ʹ�ûص�
            //������ض�����ܻ���Ч,���ݴ�ԭ�ж���,ʹ�ûص�
        }

        private void RecycObjectShield(BUFF buff)
        {
            ObjectPool.Instance.RecycleObject(deployer.gameObject);
        }
    }
}
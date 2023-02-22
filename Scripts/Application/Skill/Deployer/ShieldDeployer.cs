using EffectSystem;
using UnityEngine;

namespace SkillSystem
{
    public class ShieldDeployer : SkillDeployer
    {
        //public AudioClip shieldContinueAudio;   //���Ƴ�����Ч
        //public AudioClip beHurtAudio;  //�����ܻ�����Ч
        //public AudioClip breakAudio;    //����������Ч
        //public Effect beHurtEffect; //�ܻ���Ч
        public float OffestX;
        public float OffestY;
        public float shield = 1;

        private float shieldPri;

        private void Awake()
        {
            shieldPri = shield;
        }

        public override void DeploySkill()
        {
            Init();
            ImpactTargets();
        }

        public void BeHurt(float _damage)
        {
            shield -= _damage;

            if (shield <= 0)
            {
                //�Ȳ���������Ч,�ڲ����ܻ���Ч
                //PlayBreakAudio();
                ObjectPool.Instance.RecycleObject(gameObject);
            }
            else
            {
                //PlayBeHurtAudio();
            }
        }

        private void Init()
        {
            shield = shieldPri * skillData.attackCoe;
            transform.position = new Vector3(transform.position.x + OffestX, transform.position.y + OffestY, transform.position.z);
            MusicManager.Instance.PlaySound("Shield",gameObject,false);
            //��ȡ���Ƴ�����Ч������
            //��ȡ�����ܻ���Ч
            //��ȡ����������Ч
            //breakEffect = EffectManager.Instance.AddEffect();
            //beHurtEffect = EffectManager.Instance.AddEffect();
        }

        private void OnDisable()
        {
            //shieldContinueAudio = null;
            //beHurtAudio = null;
            //beHurtEffect = null;
        }
    }
}
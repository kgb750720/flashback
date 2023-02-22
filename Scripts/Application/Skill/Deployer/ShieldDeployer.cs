using EffectSystem;
using UnityEngine;

namespace SkillSystem
{
    public class ShieldDeployer : SkillDeployer
    {
        //public AudioClip shieldContinueAudio;   //盾牌持续音效
        //public AudioClip beHurtAudio;  //盾牌受击打音效
        //public AudioClip breakAudio;    //盾牌破碎音效
        //public Effect beHurtEffect; //受击特效
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
                //先播放破碎音效,在播放受击音效
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
            //获取盾牌持续音效并播放
            //获取盾牌受击音效
            //获取盾牌破碎音效
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
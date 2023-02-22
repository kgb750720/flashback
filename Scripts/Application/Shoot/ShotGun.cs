using EffectSystem;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace ShootSystem
{
    public class ShotGun : FireController
    {
        public int bulletNum = 3;
        public float bulletAngle = 22.5F;

        private UnityAction<SkillBuffArgs> action;

        protected override void Start()
        {
            base.Start();

            action = HotFire;
            Game.Instance.EventCenter.AddEventListener(Consts.E_BUFF_BulletNum, action);
        }

        protected override void Fire(Vector2 _direction)
        {
            #region(Muzzle)
            Effect fire = EffectManager.Instance.AddEffect3D(muzzleEffectName, muzzlePos.position, _direction, muzzlePos.gameObject);
            fire.RecycleEffectPercentage(0.9F);
            #endregion

            #region(Bullet)
            int med = bulletNum / 2;

            for (int i = 0; i < bulletNum; i++)
            {
                //获取是否是技能子弹

                Effect bullet = EffectManager.Instance.AddWorldEffect3D(bulletEffectName, muzzlePos.position, _direction);
                bullet.transform.position = new Vector3(muzzlePos.position.x, muzzlePos.position.y, 0F);

                if (bulletNum % 2 == 1)
                {
                    bullet.GetComponent<Bullet>().Init(Quaternion.AngleAxis(bulletAngle * (i - med), Vector3.forward) * _direction, bulletSpeed, maxDistance, attackCoe, gameObject, isBuffTime);
                }
                else
                {
                    bullet.GetComponent<Bullet>().Init(Quaternion.AngleAxis(bulletAngle * (i - med) + bulletAngle / 2, Vector3.forward) * _direction, bulletSpeed, maxDistance, attackCoe, gameObject, isBuffTime);
                }
            }
            #endregion

            LastFireTime = Time.time;
        }

        protected override void SkillFire(Vector2 _direction)
        {
            #region(Muzzle)
            //需要大炮的火焰
            Effect fire = EffectManager.Instance.AddEffect3D(skillMuzzleEffectName, ArtMuzzlePos.position, _direction, ArtMuzzlePos.gameObject);
            #endregion

            #region(Bullet)
            Effect bullet;
            bullet = EffectManager.Instance.AddWorldEffect3D(skillBulletEffectName, ArtMuzzlePos.position, _direction);

            bullet.transform.position = new Vector3(ArtMuzzlePos.position.x, ArtMuzzlePos.position.y, -1F);
            bullet.GetComponentInParent<GravityBullet>().SetSkillData(skillData);
            bullet.GetComponent<Bullet>().Init(_direction, skillBulletSpeed, maxSkillDistance, attackCoe, gameObject);
            bullet.RecycleEffect(5F, true);
            #endregion

            LastFireTime = Time.time;
        }

        private void HotFire(SkillBuffArgs _args)
        {
            if (_args.buffType == SkillSystem.BuffType.ShotGunNum)
            {
                bulletNum += (int)_args.buffChange;
            }
        }

        private void OnDestroy()
        {
            Game.Instance.EventCenter.AddEventListener(Consts.E_BUFF_BulletNum, action);
        }
    }
}

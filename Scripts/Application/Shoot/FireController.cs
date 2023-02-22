using System.Collections;
using EffectSystem;
using UnityEngine;
using UnityEngine.Events;

namespace ShootSystem
{
    public class FireController : MonoBehaviour
    {
        public float bulletSpeed;
        public float skillBulletSpeed;
        public float randomAngle;
        public float maxDistance;
        public float maxSkillDistance;
        public float attackCoe;         //伤害系数
        public SlowType skillSlow;
        public SlowType bulletSlow;
        protected bool isBuffTime = false;
        protected float LastFireTime;
        protected Transform ArtMuzzlePos;   //大炮枪口位置
        protected Transform muzzlePos;  //枪口位置
        protected Transform shellPos;   //通过muzzlePos和direction获取
        protected SkillSystem.SkillData skillData;
        private bool isActive = true;

        #region (特效名字)
        public string muzzleEffectName;
        public string bulletEffectName;
        public string skillMuzzleEffectName;
        public string skillBulletEffectName;
        public string hotFireEffectName;
        public string hotMuzzleEffectName;
        private string tempBulletEffectName;
        private string tempMuzzleEffectName;
        #endregion

        #region(音效名字)
        public string ShootAudio;
        #endregion

        private UnityAction<SkillBuffArgs> actionHotFire;

        protected virtual void Start()
        {
            muzzlePos = transform.Find("Muzzle");
            if (muzzlePos != null)
                muzzlePos.position = new Vector3(muzzlePos.position.x, muzzlePos.position.y, 0F);
            ArtMuzzlePos = transform.Find("ArtMuzzle");
            if (ArtMuzzlePos != null)
                ArtMuzzlePos.position = new Vector3(ArtMuzzlePos.position.x, ArtMuzzlePos.position.y, 0F);

            tempBulletEffectName = bulletEffectName;
            tempMuzzleEffectName = muzzleEffectName;

            actionHotFire = HotFire;
            Game.Instance.EventCenter.AddEventListener(Consts.E_BUFF_FireSpeed, actionHotFire);
        }

        public void SetMuzzlePos(Transform _muzzle)
        {
            muzzlePos = _muzzle;
        }

        public void SetShellPos(Transform _artMuzzle)
        {
            ArtMuzzlePos = _artMuzzle;
        }

        public virtual void DoShoot(Vector2 _direction)
        {
            if (isActive)
            {
                Fire(_direction);
                MusicManager.Instance.PlaySound(ShootAudio, gameObject,false);
            }
        }

        public virtual void DoShoot(Vector2 _direction, SkillSystem.SkillData _skillData)    //参数可以换成int skillID
        {
            skillData = _skillData;
            if (isActive) SkillFire(_direction);
        }

        #region (Action)
        //过热开火时间,供给角色技能调用
        private void HotFire(SkillBuffArgs _args)
        {
            isBuffTime = !isBuffTime;

            if (isBuffTime)
            {
                bulletEffectName = hotFireEffectName;
                muzzleEffectName = hotMuzzleEffectName;
            }
            else
            {
                bulletEffectName = tempBulletEffectName;
                muzzleEffectName = tempMuzzleEffectName;
            }
        }
        #endregion

        protected virtual void Fire(Vector2 _direction)
        {
            #region(Muzzle)
            if (muzzlePos != null)
            {
                Effect fire = EffectManager.Instance.AddEffect3D(muzzleEffectName, muzzlePos.position, _direction, muzzlePos.gameObject);
                if (fire != null) fire.RecycleEffect(0.1F);
            }
            #endregion

            #region(Bullet)
            Effect bullet;
            bullet = EffectManager.Instance.AddWorldEffect3D(bulletEffectName, muzzlePos.position, _direction);
            bullet.transform.position = new Vector3(muzzlePos.position.x, muzzlePos.position.y, 0F);

            float angle = Random.Range(-randomAngle, randomAngle);
            Vector2 Qt = Quaternion.AngleAxis(angle, new Vector3(0,0,1)) * _direction;
            bullet.GetComponent<Bullet>().Init(Qt, bulletSpeed , maxDistance, attackCoe, gameObject , isBuffTime);
            #endregion

            LastFireTime = Time.time;
        }

        protected virtual void SkillFire(Vector2 _direction)
        {
            #region(Muzzle)
            //需要大炮的火焰
            EffectManager.Instance.AddEffect3D(skillMuzzleEffectName, ArtMuzzlePos.position, _direction, ArtMuzzlePos.gameObject).RecycleEffectPercentage(0.9F);
            #endregion

            #region(Bullet)
            Effect bullet;
            bullet = EffectManager.Instance.AddWorldEffect3D(skillBulletEffectName, ArtMuzzlePos.position, _direction);
            bullet.transform.position = new Vector3(ArtMuzzlePos.position.x, ArtMuzzlePos.position.y, 0F);
            bullet.GetComponentInParent<SkillBullet>().SetSkillData(skillData);

            bullet.GetComponent<Bullet>().Init(_direction, skillBulletSpeed, maxSkillDistance, attackCoe, gameObject);

            bullet.RecycleEffect(5F, true);
            #endregion

            LastFireTime = Time.time;
        }

        protected virtual void OnDisable()
        {
            isActive = false;
        }

        protected virtual void OnEnable()
        {
            isActive = true;
        }

        private void OnDestroy()
        {
            Game.Instance.EventCenter.RemoveEventListener(Consts.E_BUFF_FireSpeed, actionHotFire);
        }
    }
}

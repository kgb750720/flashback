using SkillSystem;
using System.Collections;
using UnityEngine;

namespace ShootSystem
{
    public class GravityBullet : Bullet
    {
        public int skillID;
        public float initScale = 0.7F;
        public float afterScale = 1.5F;

        private SkillData skillData;
        private bool Generate = false;

        public void SetSkillData(SkillData _skillData)
        {
            Generate = false;
            skillData = _skillData;
            GetComponent<EffectSystem.Effect>().SetScale(transform, initScale);
        }

        public override void Init(Vector2 _v2, float speed, float _maxDistance, float _coe, GameObject _go = null , bool _isHot = false)
        {
            base.Init(_v2, speed, _maxDistance, _coe, _go);
        }

        protected override void TriggerToDo(Collider2D _collision)
        {
            if (targetTags.Length != 0)
            {
                for (int i = 0; i < targetTags.Length; ++i)
                {
                    if (_collision.tag == targetTags[i])
                    {
                        GetComponent<EffectSystem.Effect>().SetScale(transform, afterScale);
                        break;
                    }
                }
            }
            else
                Debug.LogError("SkillData没有设置可影响Tags");
        }

        protected override void RecycleObject(GameObject _gameObject) 
        {
            isFire = false;     //重力子弹行为停止
            GetComponent<EffectSystem.Effect>().SetScale(transform, afterScale);     //重设大小,可优化项目
            skillData.character.GetComponent<SkillManager>().GenerateSkill(skillData, transform);
            StartCoroutine(IERecycleObject(gameObject, skillData.durationTime));
        }

        protected override void BulletMovement()
        {
            base.BulletMovement();
        }

        protected override void Distance()
        {
            if (maxDistance != 0 && Generate == false 
                && (Mathf.Abs(startPos.x - transform.position.x) >= maxDistance 
                || Mathf.Abs(startPos.y - transform.position.y) >= maxDistance))
            {
                Generate = true;
                RecycleObject(gameObject);
            }
        }

        private IEnumerator IERecycleObject(GameObject _gameObject, float _waitTime)
        {
            CameraManager.Instance.DoShake(hitShakeType, hitShakeTime);
            CameraManager.Instance.GravityBullet(1,1,1);
            Game.Instance.TimeManager.SetBulletTime(attacker.GetComponent<FireController>().skillSlow);
            yield return new WaitForSeconds(_waitTime);
            base.RecycleObject(_gameObject);
        }
    }
}

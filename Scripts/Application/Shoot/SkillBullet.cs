using SkillSystem;
using System.Collections;
using UnityEngine;

namespace ShootSystem
{
    public class SkillBullet : Bullet
    {
        public int skillID;

        private SkillData skillData;
        private bool Generate = false;

        public void SetSkillData(SkillData _skillData)
        {
            Generate = false;
            skillData = _skillData;
        }

        public override void Init(Vector2 _v2, float speed, float _maxDistance, float _coe, GameObject _go = null, bool _isHot = false)
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
                        CameraManager.Instance.DoShake(hitShakeType, hitShakeTime);
                        Game.Instance.TimeManager.SetBulletTime(attacker.GetComponent<FireController>().skillSlow);
                        skillData.character.GetComponent<SkillManager>().GenerateSkill(skillData, transform);
                        break;
                    }
                }
            }
            else
                Debug.LogError("SkillData没有设置可影响Tags");
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

        protected override void RecycleObject(GameObject _gameObject)
        {
             base.RecycleObject(_gameObject);
        }
    }
}

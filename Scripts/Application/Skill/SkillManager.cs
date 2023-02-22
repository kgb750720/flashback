using System.Collections;
using System.Collections.Generic;
using EffectSystem;
using UnityEngine;

namespace SkillSystem
{
    public class SkillManager : MonoBehaviour
    {
        public SkillData[] Skills;

        public Dictionary<int, SkillData> skillDic = new Dictionary<int, SkillData>();
        private UnityEngine.Events.UnityAction<SkillLaunchedArgs> action;
        private CDController CD;

        private void Awake()
        {
            foreach (var skill in Skills)
            {
                InitSkill(skill);
            }
            action = OnSkillLaunched;
        }

        private void Start()
        {
            CD = gameObject.GetComponentInParent<CDController>();
        }

        private void InitSkill(SkillData _data)
        {
            if (_data.prefabName != null)
            {
                _data.character = gameObject;

                if (!skillDic.ContainsKey(_data.skillID))
                {
                    skillDic.Add(_data.skillID, _data);
                }
            }
        }

        public SkillData JudgeSkill(int _id)
        {
            if (skillDic.ContainsKey(_id))
            {
                return skillDic[_id];
            }
            else
            {
                Debug.LogError("找不到技能 ID " + _id);
                return null;
            }
        }

        public bool GenerateBulletSkill(Vector2 _direction, int _id)
        {
            SkillData skillData = JudgeSkill(_id);

            if (skillData != null && skillData.character != null)
            {
                skillData.character.GetComponent<ShootSystem.FireController>().DoShoot(_direction, skillData);
                return true;
            }
            return false;
        }

        public void GenerateSkill(SkillData _data, Transform _transform)
        {
            GameObject skillgo = null;

            switch (_data.prefabType)
            {
                case PrefabType.ParticleSystem:
                    skillgo = EffectManager.Instance.AddWorldEffect2D(_data.prefabName, _transform.position).gameObject;
                    skillgo.GetComponent<Effect>().RecycleEffect(_data.durationTime);
                    break;
                case PrefabType.SpriteRender:
                    skillgo = ObjectPool.Instance.GetObject(_data.prefabPath);
                    skillgo.transform.position = _transform.position;
                    break;
                case PrefabType.None:
                    skillgo = ObjectPool.Instance.GetObject(_data.prefabPath);
                    break;
            }

            if (_data.skillType == SkillType.Buff) skillgo.transform.SetParent(_transform);

            SkillDeployer deployer = skillgo.GetComponent<SkillDeployer>();
            deployer.skillData = _data;
            deployer.DeploySkill();
        }

        public void StartSkillCD(int _id)
        {
            SkillData data = JudgeSkill(_id);
            if (data != null)
                CD.AddNormalSkillCD(_id, data);
        }

        public float CheckSkillCD(int _id)
        {
            return CD.GetNormalCD(_id);
        }

        private void OnSkillLaunched(SkillLaunchedArgs e)
        {
            GenerateBulletSkill(e.director, e.skillId);
        }

        private void OnEnable()
        {
            Game.Instance.EventCenter.AddEventListener(Consts.C_SkillLaunched, action);
        }

        private void OnDisable()
        {
            Game.Instance.EventCenter.RemoveEventListener(Consts.C_SkillLaunched, action);
        }
    }
}
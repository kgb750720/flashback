using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SkillSystem
{
    public class ContinueDeployer : SkillDeployer
    {
        public string ContinueAudio;
        private Dictionary<string, bool> tagBool = new Dictionary<string, bool>();
        private AudioSource ContinueAudioSource;
        public override void DeploySkill()
        {
            CalculateTargets();
            Init();

            MusicManager.Instance.PlaySound(ContinueAudio, gameObject, false , 1 , GetAudio);
        }

        private void Init()
        {
            if (skillData.attackTargets.Count != 0)
            {
                for (int i = 0; i < skillData.attackTargets.Count;++i)
                {
                    if (!tagBool.ContainsKey(skillData.attackTargets[i].name)) tagBool.Add(skillData.attackTargets[i].name,true);
                }
            }
        }

        private void GetAudio(AudioSource _audio)
        {
            ContinueAudioSource = _audio;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (skillData.attackTargetTags.Length != 0)
            {
                for (int i = 0; i < skillData.attackTargetTags.Length; ++i)
                {
                    if (collision.tag == skillData.attackTargetTags[i])
                    {
                        if (tagBool.ContainsKey(collision.name))
                        {
                            return;
                        }
                        else
                        {
                            tagBool.Add(collision.name, true);
                            skillData.attackTargets.Add(collision.transform);
                        }
                    }
                }
            }
            else
                Debug.LogError("SkillData没有设置可影响Tags");
        }

        private void OnDisable()//控制的是技能结束的对象的清除力
        {
            if (skillData == null) return;
            if (ContinueAudioSource != null) MusicManager.Instance.StopSound(ContinueAudioSource);

            if (skillData.attackTargets != null && skillData.attackTargets.Count != 0)
            {
                for (int i = 0; i < skillData.attackTargets.Count;++i)
                {
                    if (skillData.attackTargets[i].gameObject != null) skillData.attackTargets[i].GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                    //skillData.attackTargets[i].GetComponent<Rigidbody2D>().isKinematic = true;      //禁止施加力
                }
            }

            tagBool.Clear();
            skillData.attackTargets = null;
        }

        private void FixedUpdate()  //可以直接开始调用,技能产生则开始执行
        {
            ImpactTargets();
        }
    }
}

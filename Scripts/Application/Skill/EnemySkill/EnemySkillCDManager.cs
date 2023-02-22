using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SkillSystem
{
    public class EnemySkillCDManager : MonoBehaviour
    {
        private Dictionary<int, List<KeyValuePair<string, float>>> enemyCDControl = new Dictionary<int, List<KeyValuePair<string, float>>>();
        //字典——<技能ID，键对> 键对——<敌人名字，技能CD剩余时间>

        public void Add(int _id,string _name,float _cd)
        {
            if (enemyCDControl.ContainsKey(_id))
            {
                if (enemyCDControl[_id].Contains(new KeyValuePair<string, float>(_name, _cd)))
                {
                    return;
                }
                else
                {
                    enemyCDControl[_id].Add(new KeyValuePair<string, float>(_name, _cd));
                }
            }
            else
            {
                List <KeyValuePair<string, float>> kv = new List<KeyValuePair<string, float>>();

                kv.Add(new KeyValuePair<string, float> (_name,_cd));
                enemyCDControl.Add(_id, kv);
            }
        }

        private void Remove(int _id)
        {
            if (enemyCDControl.ContainsKey(_id))
            {
                enemyCDControl.Remove(_id);
            }
            else
            {
                return;
            }
        }

        private IEnumerator CoolDownTime()
        {
            yield return new WaitForSeconds(1F);

            foreach (var key in enemyCDControl.Keys)
            {
                for (int i = 0; i < enemyCDControl[key].Count; ++i)
                {
                    if (enemyCDControl[key][i].Value == 0) enemyCDControl[key].RemoveAt(i);

                    enemyCDControl[key][i] = new KeyValuePair<string, float>(enemyCDControl[key][i].Key,enemyCDControl[key][i].Value - 1);
                }

                if (enemyCDControl[key].Count == 0)
                {
                    Remove(key);
                }
            }
        }

        public void Update()
        {
            if (enemyCDControl.Count != 0)
            {
                StartCoroutine(CoolDownTime());
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SkillSystem
{
    public class SectorSkillSelector : IAttackSeletor
    {
        GameObject[] tempArray;

        public List<Transform> SelectorTarget(SkillData _data, Transform _skillPrefab)
        {
            List<Transform> targets = new List<Transform>();

            for (int i = 0; i < _data.attackTargetTags.Length; i++)
            {
                tempArray = GameObject.FindGameObjectsWithTag(_data.attackTargetTags[i]);

                for (int j = 0; j < tempArray.Length; j++)
                {
                    targets.Add(tempArray[j].transform);
                }
            }

            targets = targets.FindAll(T =>
                Vector3.Distance(T.position, _skillPrefab.position) <= _data.attackDistance 
                //&& Vector3.Angle(_skillPrefab.forward, T.position - _skillPrefab.position) <= _data.attackAngle / 2
            );

            if (targets.Count == 0)
            {
                Debug.Log("û�е���");
                return targets;
            }
            else
            {
                for (int i = 0; i < targets.Count; i++)
                {
                    Debug.Log(targets[i].name);
                }
            }
            return targets;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SkillSystem
{
    public class SingleSkillSelector : IAttackSeletor
    {
        public List<Transform> SelectorTarget(SkillData _data, Transform _skillPrefab)
        {
            List<Transform> singleTargets = new List<Transform>();
            Transform[] singleTargetsArray = singleTargets.ToArray();

            return singleTargets;
        }
    }
}

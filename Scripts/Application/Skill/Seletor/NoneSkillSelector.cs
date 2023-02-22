using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SkillSystem
{
    public class NoneSkillSelector : IAttackSeletor
    {
        public List<Transform> SelectorTarget(SkillData _data, Transform _skillPrefab)
        {
            return null;
        }
    }
}

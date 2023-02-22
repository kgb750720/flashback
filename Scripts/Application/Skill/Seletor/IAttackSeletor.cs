using System.Collections.Generic;
using UnityEngine;

namespace SkillSystem
{
    public interface IAttackSeletor
    {
        List<Transform> SelectorTarget(SkillData _data, Transform _skillPrefab);
    }
}
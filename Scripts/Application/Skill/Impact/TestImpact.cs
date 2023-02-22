using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SkillSystem
{
    public class TestImpact : MonoBehaviour , IImpactEffect
    {
        SkillData data;
        bool isBool = false;
        public void Execute(SkillDeployer _deployer)
        {
            data = _deployer.skillData;
            isBool = true;

            Debug.LogError("����ִ������ʵ�ֵĽӿڵķ��� " + isBool);

            StartCoroutine(Te());
        }

        private void Update()
        {
            if (isBool) Debug.LogError(isBool);
        }
        private IEnumerator Te()
        {
            yield return new WaitForSeconds(2F);
            Debug.Log("ţ��");
        }
    }
}
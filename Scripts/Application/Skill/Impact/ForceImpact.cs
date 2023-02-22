using UnityEngine;

namespace SkillSystem
{
    public class ForceImpact : IImpactEffect
    {
        //两个参数可以供给策划调整
        private float maxForce = 300F;
        private float minForce = 50F;

        public void Execute(SkillDeployer _deployer)
        {
            if (_deployer.skillData.attackTargets != null && _deployer.skillData.attackTargets.Count != 0)
            {
                for (int i = 0; i < _deployer.skillData.attackTargets.Count; ++i)
                {
                    if (_deployer.skillData.attackTargets[i] == null || _deployer.skillData.attackTargets[i].GetComponent<Rigidbody2D>() == null)
                    {
                        Debug.LogError("被销毁或不存在组件Rigidbody2D");
                        _deployer.skillData.attackTargets.RemoveAt(i);
                        continue;
                    }

                    Rigidbody2D deployerRig2D = _deployer.skillData.attackTargets[i].GetComponent<Rigidbody2D>();
                    Vector3 targetPos = new Vector3(_deployer.skillData.attackTargets[i].position.x
                        , _deployer.skillData.attackTargets[i].position.y + _deployer.skillData.attackTargets[i].GetComponent<FSM>().CharacterHight / 2,0);

                    Debug.DrawLine(_deployer.transform.position, targetPos, Color.red);
                    Vector2 distance = _deployer.transform.position - targetPos;
                    //float circulate = (maxForce - minForce) * (_deployer.skillData.attackDistance - distance) / _deployer.skillData.attackDistance + minForce;
                    float circulate = (maxForce - minForce) * (distance.magnitude) / _deployer.skillData.attackDistance + minForce;

                    if (distance.magnitude < _deployer.skillData.attackDistance / 8)
                    {
                        //deployerRig2D.velocity = new Vector2(distance.normalized.x,0);
                        deployerRig2D.AddForce(distance.normalized * circulate);
                    }
                    else
                    {
                        deployerRig2D.velocity = Vector2.zero;
                        deployerRig2D.AddForce(distance.normalized * circulate);
                    }

                    if (_deployer.skillData.attackDistance < Mathf.Abs(distance.magnitude))
                    {
                        //删除操作,控制的是技能过程中离开的对象的清除力
                        deployerRig2D.velocity = Vector2.zero;
                        _deployer.skillData.attackTargets.RemoveAt(i);
                        return;
                    }
                }
            }
        }
    }
}

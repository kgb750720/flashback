using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ShootSystem
{
    public class EnemyBullet : Bullet
    {
        private Vector3 targetPos;
        private Vector3 bulletDir;
        protected override void BulletMovement()
        {
            transform.position = transform.position + (bulletDir * finalSpeed * Time.deltaTime * timeSlowCoe);
        }


        public override void Init(Vector2 _v2, float speed, float _maxDistance, float _coe, GameObject _go = null ,bool _isHot = false)
        {
            base.Init(_v2, speed, _maxDistance, _coe, _go);
            Transform paleyrTrans = attacker.GetComponent<FSM>().parameter.target;
            Vector2 maxOffset = Vector2.zero;
            foreach (var item in paleyrTrans.GetComponents<Collider2D>())
            {
                if (Vector2.Distance(Vector2.zero, item.offset) > Vector2.Distance(Vector2.zero, maxOffset))
                    maxOffset = item.offset;
            }

            targetPos = paleyrTrans.position + new Vector3(maxOffset.x,maxOffset.y);
            transform.LookAt(targetPos);
            transform.Rotate(new Vector3(0, -90, 0));
            bulletDir = (targetPos - transform.position).normalized;
        }

        protected override void RaycastTrigger()
        {
            if (Physics2D.Raycast(prePostion, bulletDir, distance, LayerMask.GetMask("InvinciblePlayer")).collider != null)
            {
                //新增层级需要使用运算符|
                //射线检测
                Physics2D.Raycast(prePostion, bulletDir, distance, LayerMask.GetMask("InvinciblePlayer")).collider.GetComponent<RealCharacterController>().CheckScratch();
            }
        }

        protected override void Distance()
        {
            if(attacker)
                base.Distance();
        }
    }
}

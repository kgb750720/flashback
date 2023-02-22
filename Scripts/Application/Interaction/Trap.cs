using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InteractiveSystem
{
    public class Trap : InteractionManager
    {
        public int damage = 1;
        public bool vertical = false;
        public bool horizontal = true;
        public float backTime = 1F;

        protected override void Trigger(Collider2D collision)
        {
            if (collision.tag == "Player")
            {
                DoHurt(collision);

                if (horizontal)
                {
                    //执行传送
                    if (collision.GetComponent<RealCharacterController>().isDead == false)
                        StartCoroutine(WaitForPass(collision, collision.GetComponent<RealCharacterController>().hurtTime));
                }
            }
        }

        //这个受击不需要动画？
        private void DoHurt(Collider2D collision)
        {
            int hurtIndex = 0;
            if (collision.transform.position.x - transform.position.x > 0)
            {
                hurtIndex = -1;
            }
            else if (collision.transform.position.x - transform.position.x < 0)
            {
                hurtIndex = 1;
            }
            else
            {
                int random = Random.Range(0, 2);
                hurtIndex = random == 0 ? 1 : -1;
            }
            collision.gameObject.GetComponent<RealCharacterController>().Hurt(hurtIndex, damage);
        }

        private IEnumerator WaitForPass(Collider2D collision, float _waitTime)
        {
            yield return new WaitForSeconds(_waitTime);
            collision.transform.position = normal.lastInteraction.position;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}
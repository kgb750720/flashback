using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InteractiveSystem
{
    public class LaserTrap : Trap
    {
        //需要执行动画和消失动画的过渡？

        public float activeTime = 5F;
        public float inActiveTime = 5F;
        private BoxCollider2D box2D;

        private void Start()
        {
            box2D = gameObject.GetComponent<BoxCollider2D>();
        }

        private void StartTrap()
        {
            box2D.enabled = true;

            if (GetComponent<SpriteRenderer>() != null)
            {
                GetComponent<SpriteRenderer>().color =
                    new Color(GetComponent<SpriteRenderer>().color.r, GetComponent<SpriteRenderer>().color.g, GetComponent<SpriteRenderer>().color.b, 255);
            }
        }

        private void CancelTrap()
        {
            box2D.enabled = false;

            if (GetComponent<SpriteRenderer>() != null)
            {
                GetComponent<SpriteRenderer>().color =
                    new Color(GetComponent<SpriteRenderer>().color.r, GetComponent<SpriteRenderer>().color.g, GetComponent<SpriteRenderer>().color.b, 0);
            }
        }

        private void OnEnable()
        {
            InvokeRepeating("StartTrap", 0F, activeTime + inActiveTime);
            InvokeRepeating("CancelTrap", activeTime, activeTime + inActiveTime);
        }

        private void OnDisable()
        {
            CancelInvoke();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            CancelInvoke();
        }

        protected override void Trigger(Collider2D collision)
        {
            base.Trigger(collision);
        }
    }
}

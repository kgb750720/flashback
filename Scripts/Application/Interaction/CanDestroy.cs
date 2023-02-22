using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InteractiveSystem
{
    public class CanDestroy : InteractionManager
    {
        //[CustomLabel("����ֵ")]
        public int health;
        //[CustomLabel("�ƻ�����ֵ")]
        public int breakHealth;
        //[CustomLabel("�ƻ�������ʱ��")]
        public float destroyTime;

        private Rigidbody2D r2d;

        private void Start()
        {
            r2d = GetComponent<Rigidbody2D>();

            r2d.isKinematic = true;
            GetComponent<BoxCollider2D>().isTrigger = false;
        }

        public void BeHurt(int _damage)
        {
            health -= _damage;
 
            if (health <= 0)
            {
                StartCoroutine(DestroyWait(destroyTime));
            }
        }

        private IEnumerator DestroyWait(float _waitTime)
        {
            yield return new WaitForSeconds(_waitTime);
            Destroy(gameObject);
        }
    }
}

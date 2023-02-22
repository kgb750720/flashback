using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InteractiveSystem
{
    public class Disappear : InteractionManager
    {
        public enum DisappearType
        { 
            Trigger = 0,
            Time = 1,
        }

        //[CustomLabel("��ʧ����")]
        public DisappearType disappearType = DisappearType.Trigger;
        //[CustomLabel("��ʧʱ��")]
        public float disappearTime = 2F;
        //[CustomLabel("��˸�ٶ�")]
        public float flashSpeed = 1F;

        private SpriteRenderer sr;
        private float startTime = 0F;
        private bool isStart = false;

        protected override void Awake()
        {
            base.Awake();
            sr = GetComponent<SpriteRenderer>();

            if (disappearType == DisappearType.Trigger)
            {
                GetComponent<BoxCollider2D>().isTrigger = true;
            }
            else
            {
                GetComponent<BoxCollider2D>().isTrigger = false;
            }
        }

        protected override void Trigger(Collider2D _collision)
        {
            if (_collision.tag == "Player")
            {
                if (disappearType == DisappearType.Trigger)
                {
                    Destroy(gameObject);
                }
            }
        }

        private void OnCollisionEnter2D(Collision2D _collision)
        {
            if (_collision.collider.tag == "Player" 
                && _collision.contacts[0].normal.y < 0 
                && _collision.contacts[0].normal.x <= 1/2 
                && _collision.contacts[0].normal.x >= -1/2)
            {
                if (disappearType == DisappearType.Time)
                {
                    StartCoroutine(TimeToDisappear());
                }
            }
        }

        private IEnumerator TimeToDisappear()
        {
            isStart = true;
            yield return new WaitForSeconds(disappearTime);
            Destroy(gameObject);
        }

        private new void Update()
        {
            if (isStart)
            {
                startTime += flashSpeed * Time.deltaTime;
                sr.color = new Color(sr.color.r,sr.color.g,sr.color.b, Mathf.Abs(Mathf.Sin(startTime)));
            }
        }
    }
}

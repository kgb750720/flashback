using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InteractiveSystem
{
    public abstract class DialogueObject : InteractionManager
    {
        public float OffestX;
        public float OffestY;
        
        private UIDialogue uIDialogue;

        protected override void Awake()
        {
            base.Awake();
        }

        protected abstract void SetContent();

        protected override void Trigger(Collider2D collision)
        {
            if (collision.tag == "Player")
            {
                UIDynamicArgs uiDynamic = new UIDynamicArgs();
                uiDynamic.father = transform;
                uiDynamic.offset = new Vector2(OffestX, OffestY);
                UIManager.Instance.GetDynamicUI<UIDialogue>(Consts.UIDialogue, GetInstanceID(), uiDynamic, SetUIDialogue);
            }
        }

        protected override void OnTriggerExit2D(Collider2D _collision)
        {
            if (_collision.tag == "Player")
            {
                UIManager.Instance.HideDynamicPanel(GetInstanceID());
            }
        }

        private void SetUIDialogue(UIDialogue _uIDialogue)
        {
            uIDialogue = _uIDialogue;
            Tip = uIDialogue;

            SetContent();

            uIDialogue.transform.position = new Vector3(uIDialogue.transform.position.x + OffestX, 
                uIDialogue.transform.position.y + OffestY, uIDialogue.transform.position.z);
        }

        private void OnDisable()
        {
            UIManager.Instance.HideDynamicPanel(GetInstanceID());
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            UIManager.Instance.HideDynamicPanel(GetInstanceID());
        }
    }
}

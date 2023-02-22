using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace InteractiveSystem
{
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(Rigidbody2D))]

    public abstract class InteractionManager : MonoBehaviour
    {
        protected NormalManager normal;

        protected UnityAction<UIButtonArgs> actionUba;

        protected virtual void Awake()
        {
            normal = NormalManager.GetInstance();
            actionUba = ButtonDown;
            Game.Instance.EventCenter.AddEventListener(Consts.M_Button_Down, actionUba);
        }

        //物品的提示UI
        protected UIDialogue Tip;

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            Trigger(collision);
        }

        protected virtual void OnTriggerExit2D(Collider2D collision) { }

        //每个物体都会响应一次,因为不是单例
        protected virtual void Apply() 
        {
            if (Tip != null && Tip.gameObject.activeInHierarchy == false)
                return;
        }
        protected virtual void Trigger(Collider2D _collision) { }

        private void ButtonDown(UIButtonArgs uba)
        {
            if (uba.currentUI == Consts.UIPhoneBattle)
            {
                if (uba.actionType == ActionType.Interactive)
                {
                    Apply();
                }
            }
        }

        protected virtual void Update()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                //如果UI是打开的或者可交互的
                Apply();
            }
        }

        protected virtual void OnDestroy()
        {
            actionUba = ButtonDown;
            Game.Instance.EventCenter.RemoveEventListener(Consts.M_Button_Down, actionUba);
        }
    }
}

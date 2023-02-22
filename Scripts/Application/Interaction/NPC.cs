using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InteractiveSystem
{
    public class NPC : DialogueObject
    {
        //[CustomLabel("添加的描述信息")]
        public string content = "[F]键进行交互";
        //[CustomLabel("开启的对话名字 xml")]
        public string dialogName = Consts.dia_2;
        //[CustomLabel("对应需要显隐的对象")]
        public GameObject door;

        private bool isTrigger = false;

        private void Start()
        {
            if (door != null) door.SetActive(false);
        }

        protected override void SetContent()
        {
            Tip.SetText(content);
        }

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            base.OnTriggerEnter2D(collision);

            if (collision.tag == "Player")
            {
                isTrigger = true;
                AndroidStart();
            }
        }

        protected override void OnTriggerExit2D(Collider2D _collision)
        {
            base.OnTriggerExit2D(_collision);

            if (_collision.tag == "Player")
            {
                isTrigger = false;
                AndroidOver();
            }
        }

        protected virtual void AndroidStart()
        {
            Game.Instance.EventCenter.EventTrigger(Consts.T_D_Start);
        }

        protected virtual void AndroidOver()
        {
            Game.Instance.EventCenter.EventTrigger(Consts.T_D_Over);
        }

        protected override void Apply()
        {
            if (!isTrigger) return;

            base.Apply();
            StartCoroutine(DialogStartShow());
        }

        protected virtual IEnumerator DialogStartShow()
        {
            Game.Instance.InputManager.canControl = false;
            yield return Game.Instance.CameraManager.FadeIn(1, Color.black);

            DialogArgs e = new DialogArgs();
            e.dialogName = dialogName;
            Game.Instance.UIManager.ShowPanel<DialogueSystemPanel>(Consts.DialogueSystemPanel, e, EUILayer.Dialog);

            if (door != null) door.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}


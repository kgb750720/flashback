using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InteractiveSystem
{
    public class DialogueOnly : DialogueObject
    {
        //[CustomLabel("��ӵĽ�����Ϣ")]
        public string content = "����һ������";

        protected override void SetContent()
        {
            Tip.SetText(content);
        }
    }
}


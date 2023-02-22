using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnDisableMsgBroadcast : MonoBehaviour
{
    public class MsgOnDisable : EventsManagerBaseMsg { }
    public class MsgOnEnable : EventsManagerBaseMsg { }

    CharacterMsgCenter m_msgCenter;


    private void OnDisable()
    {
        m_msgCenter.BroadcastMsg<MsgOnDisable, GameObject>(gameObject);
    }

    private void OnEnable()
    {
        if(!m_msgCenter)
        {
            m_msgCenter = GetComponent<CharacterMsgCenter>();

            if (!m_msgCenter)
                m_msgCenter = GetComponentInParent<CharacterMsgCenter>();

            if (!m_msgCenter)
                m_msgCenter = GetComponentInChildren<CharacterMsgCenter>();

            if (!m_msgCenter)
                Debug.LogWarning(string.Format("{0}挂着的 OnDisableMsgBroadcast 获取不到 CharacterMsgCenter 引用，请确保{1}的Transform关系链上存在 CharacterMsgCenter 脚本!", name, name));
        }
        m_msgCenter.BroadcastMsg<MsgOnEnable, GameObject>(gameObject);
    }
}

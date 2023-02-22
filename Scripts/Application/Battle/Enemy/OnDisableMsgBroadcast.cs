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
                Debug.LogWarning(string.Format("{0}���ŵ� OnDisableMsgBroadcast ��ȡ���� CharacterMsgCenter ���ã���ȷ��{1}��Transform��ϵ���ϴ��� CharacterMsgCenter �ű�!", name, name));
        }
        m_msgCenter.BroadcastMsg<MsgOnEnable, GameObject>(gameObject);
    }
}

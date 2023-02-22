using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class OnTriggerMsgBroadcast : MonoBehaviour
{
    CharacterMsgCenter m_msgCenter;

    public class MsgOnTriggerEnter2D : EventsManagerBaseMsg { }
    public class MsgOnTriggerExit2D : EventsManagerBaseMsg { }

    // Start is called before the first frame update
    void Start()
    {
        m_msgCenter = GetComponent<CharacterMsgCenter>();

        if (!m_msgCenter)
            m_msgCenter = GetComponentInParent<CharacterMsgCenter>();

        if (!m_msgCenter)
            m_msgCenter = GetComponent<CharacterMsgCenter>();

        if (!m_msgCenter)
            Debug.LogWarning(string.Format("{0}挂着的 EnemyArea 获取不到 CharacterMsgCenter 引用，请确保{1}的Transform关系链上存在 CharacterMsgCenter 脚本!", name, name));
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        m_msgCenter.BroadcastMsg<MsgOnTriggerEnter2D,Collider2D>(collider);
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
         m_msgCenter.BroadcastMsg<MsgOnTriggerExit2D, Collider2D>(collider);
    }
}

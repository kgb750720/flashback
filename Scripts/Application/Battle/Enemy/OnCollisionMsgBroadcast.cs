using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnCollisionMsgBroadcast : MonoBehaviour
{
    public class MsgOnCollision2DEnter : EventsManagerBaseMsg { }
    public class MsgOnCollision2DExit : EventsManagerBaseMsg { }

    CharacterMsgCenter m_msgCenter;
    // Start is called before the first frame update
    void Start()
    {
        m_msgCenter = GetComponent<CharacterMsgCenter>();

        if (!m_msgCenter)
            m_msgCenter = GetComponentInParent<CharacterMsgCenter>();

        if (!m_msgCenter)
            m_msgCenter = GetComponentInChildren<CharacterMsgCenter>();

        if(!m_msgCenter)
            Debug.LogWarning(string.Format("{0}挂着的 OnColiderMsgBoardcast 获取不到 CharacterMsgCenter 引用，请确保{1}的Transform关系链上存在 CharacterMsgCenter 脚本!", name, name));
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    private void OnCollisionEnter2D(Collision2D collision)
    {
        m_msgCenter.BroadcastMsg<MsgOnCollision2DEnter, Collision2D>(collision);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        m_msgCenter.BroadcastMsg<MsgOnCollision2DExit, Collision2D>(collision);
    }


}

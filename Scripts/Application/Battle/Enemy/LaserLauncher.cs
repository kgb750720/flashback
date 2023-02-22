using SkillSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterMsgCenter))]
public class LaserLauncher : MonoBehaviour
{
    public float Range = 1;
    public const float LASER_UNIT_SCALE = 0.025f;   //1U单位所代表缩放长度
    private CharacterMsgCenter m_MsgCenter;
    // Start is called before the first frame update
    void Start()
    {
        m_MsgCenter = GetComponent<CharacterMsgCenter>();
        m_MsgCenter.RegisterMsgEvent<OnTriggerMsgBroadcast.MsgOnTriggerEnter2D,Collider2D>((Collider2D collider)=>
        {
            if(collider.tag=="Player")
            {
                ShieldDeployer shell = PlayerManager.Instance.transform.Find("Buff").GetComponentInChildren<ShieldDeployer>();
                RealCharacterController rcc = collider.GetComponent<RealCharacterController>();
                if (shell)
                {
                    shell.BeHurt(1);
                }
                else
                {
                    rcc.Hurt(collider.transform.position.x - collider.transform.position.x > 0 ? 1 : -1, 1);
                }
                
            }
        }
        );
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = new Vector3(transform.localScale.x, Range * LASER_UNIT_SCALE/2, transform.localScale.z);
    }
}

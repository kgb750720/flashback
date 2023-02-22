using SkillSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterMsgCenter))]
public class EnemyMeleeAttack : MonoBehaviour
{
    public float MeleeAttackAreaRadius = 1f;
    protected CharacterMsgCenter _msgCenter;

    public class DoMeleeAttackMsg : EventsManagerBaseMsg { }

    // Start is called before the first frame update
    void Start()
    {
        _msgCenter = GetComponent<CharacterMsgCenter>();
        _msgCenter.RegisterMsgEvent<DoMeleeAttackMsg,Transform>(DoMeleeAttack);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DoMeleeAttack(Transform attackPoint)
    {
        int layer = LayerMask.NameToLayer("Player");
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, MeleeAttackAreaRadius, 1 << layer);
        RealCharacterController rcc = null;
        ShieldDeployer sd = null;
        foreach (var item in hits)
        {
            if (item.tag != "Player")
                continue;
            
            sd = item.GetComponent<ShieldDeployer>();
            if(sd)
                break;
            rcc = item.GetComponentInParent<RealCharacterController>();
        }
        if(sd)
        {
            sd.BeHurt(1);
        }
        else if(rcc)
        {
            rcc.Hurt(attackPoint.position.x < rcc.transform.position.x ? -1 : 1, 1);
        }

    }
}

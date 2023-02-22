using SkillSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyMeleeAttack))]
public class DinergateFSM : FSM
{
    protected override void InitParameter()
    {
        parameter = new DinergateStateParameter();
    }

    protected override void InitAIState()
    {
        //Ìí¼Ó×´Ì¬
        states.Add(StateType.Idle, new State.DinergateIdleState(this));
        states.Add(StateType.Patrol, new State.DinergatePatrolState(this));
        states.Add(StateType.Attack, new State.DinergateAttackState(this));
        states.Add(StateType.Hit, new State.DinergateHitState(this));
        states.Add(StateType.Death, new State.CustomDeathState(this));

        //Ñ¡Ôñ³õÊ¼×´Ì¬
        TransitionState(StateType.Idle);
    }

    protected override void overHitEvent()
    {
        var rcc = PlayerManager.Instance.GetComponentInChildren<RealCharacterController>();
        if (!Game.Instance|| !rcc || parameter.health <= 0 || PlayerManager.Instance.transform.Find("Buff").GetComponentInChildren<ShieldDeployer>())
            return;
        Vector2 selfPos = transform.position;
        Vector2 targetfPos = rcc.transform.position;
        if (targetfPos.x > selfPos.x - CharacterWidthRadius &&
            targetfPos.x < selfPos.x + CharacterWidthRadius &&
            targetfPos.y < selfPos.y + CharacterHight &&
            targetfPos.y + 1/*²¹Õý*/ >= selfPos.y)
            rcc.Hurt(selfPos.x - targetfPos.x > 0 ? 1 : -1, 1);
    }

    protected virtual void fire(string str)
    {
        msgCenter.BroadcastMsg<EnemyMeleeAttack.DoMeleeAttackMsg, Transform>(AttackPoint);
    }
}

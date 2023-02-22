using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace State
{
    public class DinergateAttackState : CustomAttackState
    {
        public DinergateAttackState(FSM manager) : base(manager){}

        //protected float attackCount=0;  //连续处于Attack状态机中产生的额外动画时间计数

        public override void OnEnter()
        {
            base.OnEnter();
            parameter.AttackIntervalCD = parameter.attackCfgCD;
        }

        public override void OnUpdate()
        {
            parameter.AttackIntervalCD -= Time.deltaTime;
            AnimatorStateInfo info = parameter.animator.GetCurrentAnimatorStateInfo(0);
            if (parameter.getHit)
            {
                manager.TransitionState(StateType.Hit);
                return;
            }
            else if(info.normalizedTime >= .95f)
            {
                manager.TransitionState(StateType.Patrol);
            }
        }

        public override void OnExit()
        {
            parameter.target = null;
        }
    }


    public class DinergatePatrolState : CustomPatrolState
    {
        public DinergatePatrolState(FSM manager) : base(manager)
        {
        }

        public override void OnEnter()
        {
            manager.parameter.animator.SetBool("Move", true);
            if(manager.LastStateType==StateType.Idle && parameter.patrolPoints.Count>0)
            {
                patrolPosition++;
                patrolPosition %= parameter.patrolPoints.Count;
            }
        }

        public override void OnUpdate()
        {
            parameter.AttackIntervalCD -= Time.deltaTime;
            if (parameter.patrolPoints.Count == 0)
                return;

            if (parameter.getHit)
            {
                manager.TransitionState(StateType.Hit);
            }
            if (parameter.target && Vector2.Distance(manager.transform.position, parameter.target.transform.position) < parameter.attackArea)
            {
                manager.TransitionState(StateType.Attack);
            }
            if (Vector2.Distance(manager.transform.position, parameter.patrolPoints[patrolPosition].position) < .1f)
            {
                manager.TransitionState(StateType.Idle);
            }
        }

        public override void OnFixedUpdate()
        {
            if (parameter.patrolPoints.Count == 0)
                return;

            manager.FlipTo(parameter.patrolPoints[patrolPosition]);

            AnimatorStateInfo info = parameter.animator.GetCurrentAnimatorStateInfo(0);

            if (info.IsName("move"))
            {
                manager.transform.position = Vector2.MoveTowards(manager.transform.position,
                    parameter.patrolPoints[patrolPosition].position, parameter.moveSpeed * Time.fixedDeltaTime*manager.TimeScale);
            }
        }

        public override void OnExit()
        {
        }
    }

    public class DinergateIdleState : CustomIdleState
    {
        public DinergateIdleState(FSM manager) : base(manager)
        {
        }

        public override void OnUpdate()
        {
            timer += Time.deltaTime;
            parameter.AttackIntervalCD -= Time.deltaTime;
            if (parameter.getHit)
            {
                manager.TransitionState(StateType.Hit);
            }
            else if (timer >= parameter.idleTime)
            {
                manager.TransitionState(StateType.Patrol);
            }
        }
    }

    public class DinergateHitState : CustomHitState
    {
        public DinergateHitState(FSM manager) : base(manager)
        {
        }

        public override void OnUpdate()
        {
            AnimatorStateInfo info = parameter.animator.GetCurrentAnimatorStateInfo(0);
            if (parameter.health <= 0)
            {
                //移除healthBar
                if (manager.HealthBar)
                    UIManager.Instance.HideDynamicPanel(manager.GetInstanceID());
                manager.TransitionState(StateType.Death);
            }
            else if(parameter.getHit)
            {
                manager.TransitionState(StateType.Hit);
            }
            else if (info.normalizedTime >= .95f)
            {
                manager.TransitionState(StateType.Patrol);
            }
        }
    }

}
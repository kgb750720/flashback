using State;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace State
{
    public class GolyatIdleState : CustomIdleState
    {
        public GolyatIdleState(FSM manager) : base(manager) { }

        public override void OnEnter()
        {
            parameter.animator.SetBool("Move", false);
        }

    }

    public class GolyatPatrolState : CustomPatrolState
    {
        public GolyatPatrolState(FSM manager) : base(manager) { }

        public override void OnUpdate()
        {
            parameter.AttackIntervalCD -= Time.deltaTime * manager.TimeScale;
            if (parameter.getHit)
            {
                manager.TransitionState(StateType.Hit);
            }
            else if (parameter.target != null &&parameter.chasePoints.Count>0&&
                parameter.target.position.x >= parameter.chasePoints[0].position.x &&
                parameter.target.position.x <= parameter.chasePoints[1].position.x)
            {
                manager.TransitionState(StateType.React);
            }
            else if (parameter.chasePoints.Count > 0 && Vector2.Distance(manager.transform.position, parameter.patrolPoints[patrolPosition].position) < .5f)
            {
                manager.TransitionState(StateType.Idle);
            }
        }
    }

    public class GolyatHitState : CustomHitState
    {
        public GolyatHitState(FSM manager) : base(manager)
        {
        }
    }

    public class GolyatChaseState : CustomChaseState
    {
        public GolyatChaseState(FSM manager) : base(manager){ }
    }

    public class GolyatAttackState : AttackState
    {
        GolyatStateParameter m_gsp;

        public GolyatAttackState(FSM manager) : base(manager)
        {
            m_gsp = manager.parameter as GolyatStateParameter;
        }
        float m_timeCount = 0f;
        float m_speed;
        public override void OnEnter() 
        {
            //Attack设为bool的原因是为将来策划万一改需求引爆可逆留下修改余地
            parameter.animator.SetBool("Attack", true);
            base.OnEnter();
            m_timeCount = 0f;
        }

        public override void OnUpdate()
        {
            m_timeCount += Time.deltaTime;

            if (parameter.getHit)
            {
                manager.TransitionState(StateType.Hit);
            }
            else if (m_timeCount>=m_gsp.attackAnimaTime) //计时器到时间了自爆
            {
                m_gsp.deadByBoom = true;
                manager.TransitionState(StateType.Death);
                parameter.health = 0;
            }
        }

        public override void OnExit()
        {
            parameter.animator.speed = manager.TimeScale;
        }
    }

    public class GolyatReactState : CustomReactState
    {
        public GolyatReactState(FSM manager) : base(manager){ }

    }

    public class GolyatDeathState : CustomDeathState
    {
        public GolyatDeathState(FSM manager) : base(manager){ }

        public override void OnEnter()
        {
            parameter.animator.SetTrigger("BoomTrigger");
            parameter.health = 0;
            if (manager.HealthBar)
            {
                UIManager.Instance.HideDynamicPanel(manager.GetInstanceID());
            }
            base.OnEnter();
        }

    }

}
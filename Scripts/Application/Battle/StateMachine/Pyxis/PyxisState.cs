using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace State
{
    public class PyxisIdleState : CustomIdleState
    {
        protected new PyxisFSM manager;
        public PyxisIdleState(PyxisFSM manager):base(manager)
        {
            this.manager = manager;
            parameter = manager.parameter;
        }
        public override void OnEnter()
        {
            parameter.animator.SetBool("React", false);
        }

        public override void OnUpdate()
        {
            parameter.AttackIntervalCD -= Time.deltaTime;
            if (parameter.target != null)
            {
                manager.TransitionState(StateType.React);
            }
        }
    }

    public class PyxisReactState : CustomChaseState
    {
        protected new PyxisFSM manager;
        public PyxisReactState(PyxisFSM manager):base(manager)
        {
            this.manager = manager;
            parameter = manager.parameter;
        }

        protected override void onEnterAnimaProcess()
        {
            parameter.animator.SetBool("React", true);
            if(parameter.getHit)
            {
                var temp = parameter.Damage;
            }
        }


        public override void OnUpdate()
        {
            parameter.AttackIntervalCD -= Time.deltaTime;
            if (parameter.getHit)
            {
                manager.TransitionState(StateType.Hit);
            }
            else if (parameter.target == null)
            {
                manager.TransitionState(StateType.Idle);
            }
            else if(parameter.AttackIntervalCD<=0)
                manager.TransitionState(StateType.Attack);
        }
    }

    public class PyxisAttackState : CustomAttackState
    {
        protected new PyxisFSM manager;
        public PyxisAttackState(PyxisFSM manager):base(manager)
        {
            this.manager = manager;
            parameter = manager.parameter;
        }

        public override void OnUpdate()
        {
            AnimatorStateInfo info = parameter.animator.GetCurrentAnimatorStateInfo(0);

            if (parameter.getHit)
            {
                manager.TransitionState(StateType.Hit);
            }
            else if (info.normalizedTime >= .95f)
                manager.TransitionState(StateType.React);
        }
    }

    public class PyxisHitState : CustomHitState
    {
        protected new PyxisFSM manager;
        public PyxisHitState(PyxisFSM manager):base(manager)
        {
            this.manager = manager;
            parameter = manager.parameter;
        }

        public override void OnUpdate()
        {
            parameter.AttackIntervalCD -= Time.deltaTime;
            if (parameter.health <= 0)
            {
                //ÒÆ³ýhealthBar
                if (manager.HealthBar)
                    UIManager.Instance.HideDynamicPanel(manager.GetInstanceID());
                manager.TransitionState(StateType.Death);
            }
            else if (parameter.getHit)
            {
                manager.TransitionState(StateType.Hit);
            }
            else if (parameter.target)
            {
                manager.TransitionState(StateType.React);
            }
            else
                manager.TransitionState(StateType.Idle);
        }
    }

}
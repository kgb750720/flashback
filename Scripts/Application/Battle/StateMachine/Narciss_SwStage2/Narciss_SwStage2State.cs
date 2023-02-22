using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace State
{
    public class Narciss_SwStage2IdleState : CustomIdleState
    {
        protected new Narciss_SwStage2FSM manager;
        public Narciss_SwStage2IdleState(Narciss_SwStage2FSM manager) : base(manager)
        {
            this.manager = manager;
        }

        public override void OnEnter()
        {
        }

        public override void OnUpdate()
        {
            if (parameter.getHit)
            {
                manager.TransitionState(StateType.Hit);
            }
        }
    }


    public class Narciss_SwStage2HitState : CustomHitState
    {
        protected new Narciss_SwStage2FSM manager;
        public Narciss_SwStage2HitState(Narciss_SwStage2FSM manager) : base(manager)
        {
            this.manager = manager;
        }

        protected override void onEnterAnimaProcess()
        {
            
        }

        public override void OnUpdate()
        {
            if (parameter.getHit)
            {
                manager.TransitionState(StateType.Hit);
            }
            else if(parameter.health<=0)
            {
                //ÒÆ³ýhealthBar
                if (manager.HealthBar)
                    UIManager.Instance.HideDynamicPanel(manager.GetInstanceID());
                GameObject.Destroy(manager.gameObject);
            }
            else
            {
                manager.TransitionState(StateType.Idle);
            }
        }
    }
}
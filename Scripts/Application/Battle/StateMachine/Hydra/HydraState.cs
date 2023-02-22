using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace State
{
    public class HydraAttackState : CustomAttackState
    {
        protected bool hasGrenade = true;
        public HydraAttackState(FSM manager) : base(manager)
        {
        }

        public override void OnEnter()
        {
            if (hasGrenade)
                parameter.animator.SetFloat("AttackType", 1);
            else
                parameter.animator.SetFloat("AttackType", 0);
            base.OnEnter();
        }

        public override void OnExit()
        {
            base.OnExit();
            hasGrenade = false;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace State
{
    public class GolyatFactoryIdleState : CustomIdleState
    {
        public GolyatFactoryIdleState(FSM manager) : base(manager) { }

        public override void OnEnter()
        {
            parameter.animator.SetBool("Move", false);
        }

    }

    public class GolyatFactoryPatrolState : CustomPatrolState
    {
        public GolyatFactoryPatrolState(FSM manager) : base(manager) { }

        public override void OnEnter()
        {
            parameter.animator.SetBool("Move", true);
        }
    }

    public class GolyatFactoryHitState : CustomHitState
    {
        public GolyatFactoryHitState(FSM manager) : base(manager)
        {
        }

    }

    public class GolyatFactoryChaseState : CustomChaseState
    {
        public GolyatFactoryChaseState(FSM manager) : base(manager) { }
    }

    public class GolyatFactoryAttackState : CustomAttackState
    {
        GolyatFactoryStateParameter m_gfsp;

        public GolyatFactoryAttackState(FSM manager) : base(manager)
        {
            m_gfsp = manager.parameter as GolyatFactoryStateParameter;
        }

        private WaitForEndOfFrame m_wfeof = new WaitForEndOfFrame();

        public override void OnEnter()
        {
            base.OnEnter();
            if (parameter.health > 0)
            {
                //产生歌利亚
                var go = Game.Instance.AssetsManager.Load<GameObject>("Prefabs/Enemy/Golyat");

                go.transform.position = manager.AttackPoint.position;
                var fsm = go.GetComponent<GolyatFSM>();
                IEnumerator waitEndOfFrameHelper()
                {
                    do
                    {
                        yield return m_wfeof;
                    } while (parameter.chasePoints.Count == 0);
                    fsm.parameter.target = parameter.target;
                }
                fsm.StartCoroutine(waitEndOfFrameHelper());
            }

        }
    }

    public class GolyatFactoryReactState : CustomReactState
    {
        public GolyatFactoryReactState(FSM manager) : base(manager) { }


    }

    public class GolyatFactoryDeathState : CustomDeathState
    {
        public GolyatFactoryDeathState(FSM manager) : base(manager) { }


    }

}
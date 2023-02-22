using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

namespace State
{
    public class CustomAerocraftFallState : IState
    {
        StateParameter parameter;
        CustomAerocraftFSM manager;

        float m_fallExitTimer = -1;
        public CustomAerocraftFallState(CustomAerocraftFSM manager)
        {
            this.manager = manager;
            parameter = manager.parameter;
            m_fallExitTimer = 0;
        }

        public virtual void OnEnter()
        {
            parameter.animator.SetTrigger("Fall");
            manager.SetRigBodyType(RigidbodyType2D.Dynamic);
        }

        public virtual void  OnExit()
        {
            manager.SetRigBodyType(RigidbodyType2D.Kinematic);
            m_fallExitTimer = -1;
        }

        public virtual void OnFixedUpdate()
        {
        }

        public virtual void OnUpdate()
        {
            m_fallExitTimer += Time.deltaTime;
            if (parameter.getHit)
                manager.TransitionState(StateType.Hit);
            //if (Vector2.Distance(manager.GetRigbodyVelocity(),Vector2.zero)<0.1f)
            if (m_fallExitTimer>3f && manager.GetRigbodyVelocity()==Vector2.zero)
                manager.TransitionState(StateType.Chase);
        }
    }

    public class CustomAerocraftHitState : CustomHitState
    {
        protected new CustomAerocraftFSM manager;
        public CustomAerocraftHitState(CustomAerocraftFSM manager) : base(manager)
        {
            this.manager = manager;
        }

        public override void OnUpdate()
        {
            AnimatorStateInfo info = parameter.animator.GetCurrentAnimatorStateInfo(0);

            if (parameter.health <= 0)
            {
                //ÒÆ³ýhealthBar
                if (manager.HealthBar)
                    UIManager.Instance.HideDynamicPanel(manager.GetInstanceID());
                manager.TransitionState(StateType.Death);
            }
            else if(parameter.getHit)
            {
                manager.TransitionState(StateType.Hit);
            }
            else if (!info.IsName("fall") && manager.hitFall)
            {
                manager.TransitionState(StateType.Fall);
            }
            else if (info.normalizedTime > .95f)
                manager.TransitionState(StateType.Chase);
        }
    }

    public class CustomAerocraftChaseState : CustomChaseState
    {
        protected new CustomAerocraftFSM manager;
        public CustomAerocraftChaseState(CustomAerocraftFSM manager) : base(manager)
        {
            this.manager = manager;
        }

        public override void OnFixedUpdate()
        {
            AnimatorStateInfo info = parameter.animator.GetCurrentAnimatorStateInfo(0);
            if (parameter.target && info.IsName("move"))
            {
                Vector2 moveTarget = parameter.target.position;
                moveTarget.y = parameter.patrolPoints[0].position.y;
                manager.transform.position = Vector2.MoveTowards(manager.transform.position,
                    moveTarget, parameter.chaseSpeed * Time.fixedDeltaTime*manager.TimeScale);
            }
        }
    }

    public class CustomAerocraftDeathState : CustomDeathState
    {
        protected new CustomAerocraftFSM manager;
        public CustomAerocraftDeathState(CustomAerocraftFSM manager) : base(manager)
        {
            this.manager = manager;
        }

        public override void OnEnter()
        {
            switchDeathType();
            parameter.animator.SetTrigger("Die");
            manager.SetRigBodyType(RigidbodyType2D.Dynamic);
            if (manager.DieCushionCollider)
                manager.DieCushionCollider.enabled = true;
            manager.BindMsgOnCollision2DEnter((Collision2D collision) =>
            {
                TilemapCollider2D collider2D = collision.collider.GetComponent<TilemapCollider2D>();
                if(collider2D)
                {
                    manager.SetRigBodyType(RigidbodyType2D.Kinematic);
                    foreach (var item in manager.GetComponentsInChildren<Collider2D>())
                    {
                        item.enabled = false;
                    }
                }
            });
        }
    }
}

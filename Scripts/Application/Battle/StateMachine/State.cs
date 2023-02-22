using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace State
{
    public class IdleState : IState
    {
        protected FSM manager;
        protected StateParameter parameter;

        protected float timer;
        public IdleState(FSM manager)
        {
            this.manager = manager;
            this.parameter = manager.parameter;
        }

        protected virtual void onEnterAnimaProcess()
        {
            parameter.animator.Play("Idle");
        }

        public virtual void OnEnter()
        {
            onEnterAnimaProcess();
        }

        public virtual void OnUpdate()
        {
            timer += Time.deltaTime * manager.TimeScale;

            if (parameter.getHit)
            {
                manager.TransitionState(StateType.Hit);
            }
            else if (parameter.target != null && parameter.chasePoints.Count > 0 &&
                parameter.target.position.x >= parameter.chasePoints[0].position.x &&
                parameter.target.position.x <= parameter.chasePoints[1].position.x)
            {
                manager.TransitionState(StateType.React);
            }
            else if (timer >= parameter.idleTime&&parameter.patrolPoints.Count>0)
            {
                manager.TransitionState(StateType.Patrol);
            }
        }

        public virtual void OnExit()
        {
            timer = 0;
        }

        public virtual void OnFixedUpdate()
        {
        }
    }

    //巡逻
    public class PatrolState : IState
    {
        protected FSM manager;
        protected StateParameter parameter;

        protected int patrolPosition;

        protected Rigidbody2D _rigbody;

        protected float fallCount = 0;
        public PatrolState(FSM manager)
        {
            this.manager = manager;
            //Debug.Log(manager.parameter.patrolPoints.Count);
            this.parameter = manager.parameter;
            _rigbody = manager.GetComponent<Rigidbody2D>();
        }

        protected virtual void onEnterAnimaProcess()
        {
            parameter.animator.Play("Walk");
        }

        public virtual void OnEnter()
        {
            onEnterAnimaProcess();
        }

        public virtual void OnUpdate()
        {
            //Debug.Log(parameter.patrolPoints.Count);
            //Debug.Log(patrolPosition);
            
            if (parameter.getHit)
            {
                manager.TransitionState(StateType.Hit);
            }
            else if (parameter.target != null &&
                parameter.target.position.x >= parameter.chasePoints[0].position.x &&
                parameter.target.position.x <= parameter.chasePoints[1].position.x)
            {
                manager.TransitionState(StateType.React);
            }
            else if (Vector2.Distance(manager.transform.position, parameter.patrolPoints[patrolPosition].position) < .5f)
            {
                manager.TransitionState(StateType.Idle);
            }
        }

        public virtual void OnExit()
        {
            patrolPosition++;

            if (patrolPosition >= parameter.patrolPoints.Count)
            {
                patrolPosition = 0;
            }
        }

        public virtual void OnFixedUpdate()
        {
            //移动
            AnimatorStateInfo info = parameter.animator.GetCurrentAnimatorStateInfo(0);
            if (parameter.target && info.IsName("Walk"))
            {
                //转向
                manager.FlipTo(parameter.patrolPoints[patrolPosition]);


                manager.transform.position = Vector2.MoveTowards(manager.transform.position,
                parameter.patrolPoints[patrolPosition].position, parameter.moveSpeed * Time.fixedDeltaTime*manager.TimeScale);
            }
            //传回判定
            if (manager.transform.position.y < parameter.patrolPoints[patrolPosition].position.y)
                fallCount += Time.fixedDeltaTime;
            else
                fallCount = 0;

            if (fallCount >= parameter.fallResetTime)
                manager.transform.position = parameter.patrolPoints[patrolPosition].position;
        }
    }

    //追逐
    public class ChaseState : IState
    {
        protected FSM manager;
        protected StateParameter parameter;

        protected Rigidbody2D _rigbody;

        public ChaseState(FSM manager)
        {
            this.manager = manager;
            this.parameter = manager.parameter;
            _rigbody = manager.GetComponent<Rigidbody2D>();
        }
        protected virtual void onEnterAnimaProcess()
        {
            parameter.animator.Play("Walk");
        }

        public virtual void OnEnter()
        {
            onEnterAnimaProcess();
        }

        public virtual void OnUpdate()
        {

            if (parameter.getHit)
            {
                manager.TransitionState(StateType.Hit);
            }

            if(parameter.health>0)
                manager.FlipTo(parameter.target);

            if (parameter.target == null ||
                manager.transform.position.x < parameter.chasePoints[0].position.x ||
                manager.transform.position.x > parameter.chasePoints[1].position.x)
            {
                manager.TransitionState(StateType.Idle);
            }
            else
            {
                float distance = Vector2.Distance(parameter.target.position, manager.transform.position);
                if (distance <= parameter.attackArea)
                {
                    manager.TransitionState(StateType.Attack);
                }
                else
                    parameter.animator.SetBool("Move", true);
            }
        }

        public virtual void OnExit()
        {

        }

        public virtual void OnFixedUpdate()
        {
            if (parameter.target)
            {
                float distance = Vector2.Distance(parameter.target.position, manager.transform.position);
                //if (Physics2D.OverlapCircle(parameter.attackPoint.position, parameter.attackArea, parameter.targetLayer)) //每帧都进行物理检测，性能损耗有点大
                AnimatorStateInfo info = parameter.animator.GetCurrentAnimatorStateInfo(0);
                if (distance > parameter.attackArea)
                {
                    manager.transform.position = Vector2.MoveTowards(manager.transform.position,
                        parameter.target.position, parameter.chaseSpeed * Time.fixedDeltaTime*manager.TimeScale);
                }

            }
        }
    }

    public class ReactState : IState
    {
        protected FSM manager;
        protected StateParameter parameter;

        private AnimatorStateInfo info;
        public ReactState(FSM manager)
        {
            this.manager = manager;
            this.parameter = manager.parameter;
        }

        protected virtual void onEnterAnimaProcess()
        {
            parameter.animator.Play("React");
        }

        public virtual void OnEnter()
        {
            onEnterAnimaProcess();
        }

        public virtual void OnUpdate()
        {
            info = parameter.animator.GetCurrentAnimatorStateInfo(0);

            if (parameter.getHit)
            {
                manager.TransitionState(StateType.Hit);
            }
            else if (info.normalizedTime >= .95f)
            {
                manager.TransitionState(StateType.Chase);
            }
        }

        public virtual void OnExit()
        {

        }

        public virtual void OnFixedUpdate()
        {
        }
    }

    public class AttackState : IState
    {
        protected FSM manager;
        protected StateParameter parameter;

        protected float _configScale = 1;

        private AnimatorStateInfo info;
        public AttackState(FSM manager)
        {
            this.manager = manager;
            this.parameter = manager.parameter;
        }

        protected virtual void onEnterAnimaProcess()
        {
            parameter.animator.Play("Attack");
        }

        public virtual void OnEnter()
        {
            onEnterAnimaProcess();
            if (parameter.attackAnimaTime > 0)
            {
                _configScale = parameter.animator.GetCurrentAnimatorStateInfo(0).length / parameter.attackAnimaTime;
                parameter.animator.speed *= _configScale;
            }
            //todo
            //调用manager的攻击接口，攻击接口的具体实现由每种怪物的攻击行为决定（近战、远程）
        }

        public virtual void OnUpdate()
        {
            info = parameter.animator.GetCurrentAnimatorStateInfo(0);

            if (parameter.getHit)
            {
                manager.TransitionState(StateType.Hit);
            }
            else if (info.normalizedTime >= .95f)    //攻击动画时间片进度大于等于0.95时转追击
            {
                manager.TransitionState(StateType.Chase);
            }
        }

        public virtual void OnExit()
        {
            parameter.animator.speed /= _configScale;
        }

        public virtual void OnFixedUpdate()
        {
        }
    }

    public class HitState : IState
    {
        protected FSM manager;
        protected StateParameter parameter;


        private AnimatorStateInfo info;
        public HitState(FSM manager)
        {
            this.manager = manager;
            this.parameter = manager.parameter;
        }
        public virtual void OnEnter()
        {
            onEnterAnimaProcess();
            parameter.health--;
        }

        public virtual void OnUpdate()
        {
            info = parameter.animator.GetCurrentAnimatorStateInfo(0);

            if (parameter.health <= 0)
            {
                manager.TransitionState(StateType.Death);
            }
            else if(parameter.getHit)
            {
                manager.TransitionState(StateType.Hit);
            }
            else if (info.normalizedTime >= .95f)
            {
                Collider2D player = Physics2D.OverlapCircle(manager.transform.position, parameter.attackArea, 1 << LayerMask.NameToLayer("Player"));
                if(player)
                    parameter.target = player.transform; 

                manager.TransitionState(StateType.Chase);
            }
        }

        public virtual void OnExit()
        {
            parameter.getHit = false;
        }

        public virtual void OnFixedUpdate()
        {
        }

        protected virtual void onEnterAnimaProcess()
        {
            parameter.animator.Play("Hit");
        }
    }

    public class DeathState : IState
    {
        protected FSM manager;
        protected StateParameter parameter;


        public DeathState(FSM manager)
        {
            this.manager = manager;
            this.parameter = manager.parameter;
        }

        protected virtual void onEnterAnimaProcess()
        {
            parameter.animator.Play("Dead");
        }

        public virtual void OnEnter()
        {
            onEnterAnimaProcess();
            foreach (var item in manager.GetComponents<Rigidbody2D>())
            {
                item.bodyType = RigidbodyType2D.Kinematic;
            }
            foreach (var item in manager.GetComponents<Collider2D>())
            {
                item.enabled = false;
            } 
        }

        public virtual void OnUpdate()
        {
            AnimatorStateInfo info = parameter.animator.GetCurrentAnimatorStateInfo(0);
        }


        public virtual void OnFixedUpdate()
        {
        }

        public virtual void OnExit()
        {
        }
    }
}
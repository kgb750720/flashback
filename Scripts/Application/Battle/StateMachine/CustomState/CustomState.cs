using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace State
{
    public class CustomIdleState : IdleState
    {
        public CustomIdleState(FSM manager) : base(manager) 
        {
        }

        protected override void onEnterAnimaProcess()
        {
            parameter.animator.SetBool("Move", false);
        }

        public override void OnUpdate()
        {
            parameter.AttackIntervalCD -= Time.deltaTime * manager.TimeScale;
            base.OnUpdate();
        }

    }

    public class CustomPatrolState : PatrolState
    {
        public CustomPatrolState(FSM manager) : base(manager)
        {
        }

        protected override void onEnterAnimaProcess()
        {
            parameter.animator.SetBool("Move", true);
        }

        public override void OnUpdate()
        {
            parameter.AttackIntervalCD -= Time.deltaTime * manager.TimeScale;
            base.OnUpdate();
        }

        public override void OnFixedUpdate()
        {

            if(parameter.patrolPoints.Count==0)
            {
                Debug.LogError(manager.name + " 未绑定巡逻路点！position：" + manager.transform.position);
                return;
            }

            //移动
            AnimatorStateInfo info = parameter.animator.GetCurrentAnimatorStateInfo(0);
            if (info.IsName("move")&&_rigbody.velocity==Vector2.zero)
            {
                //转向
                manager.FlipTo(parameter.patrolPoints[patrolPosition]);

                manager.transform.position = Vector2.MoveTowards(manager.transform.position,
                parameter.patrolPoints[patrolPosition].position, parameter.moveSpeed * Time.fixedDeltaTime*manager.TimeScale);
            }
            //传回判定
            if (manager.transform.position.y < parameter.patrolPoints[patrolPosition].position.y)
                fallCount += Time.fixedDeltaTime * manager.TimeScale;
            else
                fallCount = 0;

            if (fallCount >= parameter.fallResetTime)
                manager.transform.position = parameter.patrolPoints[patrolPosition].position;
        }

    }

    public class CustomHitState : HitState
    {
        protected float _maxHp;
        public CustomHitState(FSM manager) : base(manager)
        {
            _maxHp = parameter.health;
        }

        protected override void onEnterAnimaProcess()
        {
            parameter.animator.SetTrigger("Hurt");
        }

        public override void OnEnter()
        {
            parameter.health -= parameter.Damage;
            if (parameter.health < 0)
                parameter.health = 0;
            onEnterAnimaProcess();

            //血条更新
            if (!manager.HealthBar)
            {
                UIDynamicArgs uiDynamic = new UIDynamicArgs();
                uiDynamic.father = manager.transform;
                uiDynamic.offset = new Vector2(0, manager.CharacterHight + 0.5f);
                var hpbar = UIManager.Instance.GetDynamicUI<UIHealthBar>(Consts.UIHealthBar, manager.GetInstanceID(), uiDynamic, (UIHealthBar hpBar) =>
                {
                    hpBar.transform.position = new Vector3(hpBar.transform.position.x + uiDynamic.offset.x,
                    hpBar.transform.position.y + uiDynamic.offset.y, hpBar.transform.position.z);
                });
                manager.HealthBar = hpbar;
            }
            manager.HealthBar.UpdateHealth(parameter.health > 0 ? parameter.health / _maxHp : -1);
        }

        public override void OnUpdate()
        {
            parameter.AttackIntervalCD -= Time.deltaTime * manager.TimeScale;
            var info = parameter.animator.GetCurrentAnimatorStateInfo(0);
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
            //else if (info.normalizedTime >= .95f)
            //{
            //    Collider2D player = Physics2D.OverlapCircle(manager.transform.position, parameter.attackArea, 1 << LayerMask.NameToLayer("Player"));
            //    if (player)
            //        parameter.target = player.transform;

            //    manager.TransitionState(StateType.Chase);
            //}
            Collider2D player = Physics2D.OverlapCircle(manager.transform.position, parameter.attackArea, 1 << LayerMask.NameToLayer("Player"));
            if (player)
            {
                parameter.target = player.transform;
                manager.TransitionState(StateType.Chase);
            }

        }

    }

    public class CustomChaseState : ChaseState
    {
        public CustomChaseState(FSM manager) : base(manager) 
        {
        }
        protected override void onEnterAnimaProcess()
        {
        }

        public override void OnUpdate()
        {
            parameter.AttackIntervalCD -= Time.deltaTime * manager.TimeScale;

            if (parameter.getHit)
            {
                manager.TransitionState(StateType.Hit);
            }
            else if (parameter.target == null ||
                parameter.target.position.x < parameter.chasePoints[0].position.x ||
                parameter.target.position.x > parameter.chasePoints[1].position.x)
            {
                manager.TransitionState(StateType.Idle);
            }
            else if (parameter.target)
            {
                if (parameter.health > 0)
                    manager.FlipTo(parameter.target);
                float distance = Vector2.Distance(parameter.target.position, manager.transform.position);
                //if (Physics2D.OverlapCircle(parameter.attackPoint.position, parameter.attackArea, parameter.targetLayer)) //每帧都进行物理检测，性能损耗有点大
                if (distance <= parameter.attackArea && parameter.AttackIntervalCD <= 0)
                {
                    parameter.animator.SetBool("Move", false);
                    manager.TransitionState(StateType.Attack);
                }
                else if (distance > parameter.attackArea)
                {
                    parameter.animator.SetBool("Move", true);
                }
            }

        }

        public override void OnFixedUpdate()
        {
            AnimatorStateInfo info = parameter.animator.GetCurrentAnimatorStateInfo(0);
            if (parameter.target && info.IsName("move")&&_rigbody.velocity==Vector2.zero)
            {
                manager.transform.position = Vector2.MoveTowards(manager.transform.position,
                    parameter.target.position, parameter.chaseSpeed * Time.fixedDeltaTime*manager.TimeScale);
            }
        }
    }

    public class CustomAttackState : AttackState
    {

        public CustomAttackState(FSM manager) : base(manager)
        {
        }
        public override void OnEnter()
        {
            if (parameter.AttackIntervalCD <= 0)
            {
                onEnterAnimaProcess();
                if (parameter.attackAnimaTime > 0)
                {
                    _configScale = parameter.animator.GetCurrentAnimatorStateInfo(0).length / parameter.attackAnimaTime;
                    parameter.animator.speed *= _configScale;
                }
            }
        }


        public override void OnExit()
        {
            base.OnExit();
            parameter.AttackIntervalCD = parameter.attackCfgCD;
        }

        protected override void onEnterAnimaProcess()
        {
            parameter.animator.SetTrigger("Attack");
        }
    }

    public class CustomReactState : ReactState
    {
        public CustomReactState(FSM manager) : base(manager) { }

        protected override void onEnterAnimaProcess()
        {
            parameter.animator.SetTrigger("React");
        }

        public override void OnUpdate()
        {
            parameter.AttackIntervalCD -= Time.deltaTime * manager.TimeScale;
            base.OnUpdate();
        }
    }

    public class CustomDeathState : DeathState
    {
        public CustomDeathState(FSM manager) : base(manager) { }

        public override void OnEnter()
        {
            switchDeathType();
            onEnterAnimaProcess();
            foreach (var item in manager.GetComponents<Rigidbody2D>())
            {
                item.velocity = Vector3.zero;
                item.bodyType = RigidbodyType2D.Kinematic;
            }
            foreach (var item in manager.GetComponents<Collider2D>())
            {
                item.enabled = false;
            }
        }

        protected override void onEnterAnimaProcess()
        {
            parameter.animator.SetTrigger("Die");
        }

        protected void switchDeathType()
        {
            switch (parameter.damageType)
            {
                case DamageAnimType.Shoot:
                    parameter.animator.SetFloat("DeathType", 0);
                    break;
                case DamageAnimType.Melee:
                    parameter.animator.SetFloat("DeathType", 1);
                    break;
                case DamageAnimType.Impact:
                    parameter.animator.SetFloat("DeathType", 2);
                    break;
                default:
                    break;
            }
        }
    }
}

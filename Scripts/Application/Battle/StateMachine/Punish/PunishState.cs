using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace State
{
    public class PunishChaseState : CustomChaseState
    {
        public PunishChaseState(PunishFSM manager) : base(manager)
        {
        }

        public override void OnUpdate()
        {
            if (parameter.AttackIntervalCD > 0)
                parameter.AttackIntervalCD -= Time.deltaTime;

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
                AnimatorStateInfo info = parameter.animator.GetCurrentAnimatorStateInfo(0);
                if (!info.IsName("attack") && !info.IsName("sp_to") && info.IsName("sp_back"))
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
            if (parameter.target)
            {
                manager.FlipTo(parameter.target);
                if (info.IsName("move"))
                {
                    manager.transform.position = Vector2.MoveTowards(manager.transform.position,
                        parameter.target.position, parameter.chaseSpeed * Time.deltaTime*manager.TimeScale);
                }
            }
        }
    }

    public class PunishHitState : CustomHitState
    {
        public PunishHitState(PunishFSM manager) : base(manager)
        {
        }

        public override void OnEnter()
        {
            float damage= parameter.Damage;
            var info = parameter.animator.GetCurrentAnimatorStateInfo(0);
            bool shell = (info.IsName("move") && parameter.target && 
                ((manager.transform.position.x >= parameter.target.position.x && manager.transform.localEulerAngles == Vector3.zero)||
                (manager.transform.position.x < parameter.target.position.x && manager.transform.localEulerAngles != Vector3.zero)));
            parameter.health -= shell ? damage / 2 : damage;
            if (parameter.health < 0)
                parameter.health = 0;
            if (!shell)
                onEnterAnimaProcess();

            //血条更新
            if (!manager.HealthBar)
            {
                UIDynamicArgs uiDynamic = new UIDynamicArgs();
                uiDynamic.father = manager.transform;
                uiDynamic.offset = new Vector2(0, manager.CharacterHight + 0.5f);
                var hpBar= UIManager.Instance.GetDynamicUI<UIHealthBar>(Consts.UIHealthBar, manager.GetInstanceID(), uiDynamic, (UIHealthBar hpBar) =>
                {
                    hpBar.transform.position = new Vector3(hpBar.transform.position.x + uiDynamic.offset.x,
                    hpBar.transform.position.y + uiDynamic.offset.y, hpBar.transform.position.z);
                });
                manager.HealthBar = hpBar;
            }
            manager.HealthBar.UpdateHealth(parameter.health > 0 ? parameter.health / _maxHp : -1);

        }
    }
}
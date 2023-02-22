using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace State
{
    public class NarcissIdleState : CustomIdleState
    {
        protected new NarcissFSM manager;
        protected new NarcissStateParameter parameter;
        public NarcissIdleState(NarcissFSM manager) : base(manager)
        {
            this.manager = manager;
            this.parameter = manager.parameter as NarcissStateParameter;
        }

        public override void OnUpdate()
        {
            parameter.AttackIntervalCD -= Time.deltaTime;
            timer += Time.deltaTime;

            if (parameter.getHit)
            {
                manager.TransitionState(StateType.Hit);
            }
            else if (parameter.target)
                manager.TransitionState(StateType.Chase);
        }


    }

    public class NarcissChaseState : CustomChaseState
    {
        protected new NarcissFSM manager;
        protected new NarcissStateParameter parameter;

        private float m_fallTimer = -1f;
        public NarcissChaseState(NarcissFSM manager) : base(manager)
        {
            this.manager = manager;
            this.parameter = manager.parameter as NarcissStateParameter;
        }


        public override void OnUpdate()
        {
            parameter.AttackIntervalCD -= Time.deltaTime;

            if (parameter.getHit)
            {
                manager.TransitionState(StateType.Hit);
            }
            else if (!parameter.target)
                manager.TransitionState(StateType.Idle);
            else
            {
                manager.FlipTo(parameter.target);

                switch (parameter.bossCurrStage)
                {
                    case BossStage.Stage1:
                    case BossStage.Stage3:

                        if (parameter.target &&
                            parameter.AttackIntervalCD <= 0
                            )
                        {
                            manager.TransitionState(StateType.Attack);
                        }
                        break;
                    case BossStage.Stage2:
                        break;
                    default:
                        break;
                }
            }
        }

        public override void OnFixedUpdate()
        {
            //base.OnFixedUpdate();
            if (parameter.chasePoints[0].position.y > manager.transform.position.y)
                m_fallTimer = 0;
            if (m_fallTimer >= 0)
                m_fallTimer += Time.fixedDeltaTime*manager.TimeScale;
            if(m_fallTimer>=parameter.fallResetTime)
            {
                m_fallTimer = -1;
                manager.transform.position = parameter.chasePoints[0].position;
            }
        }
    }

    public class NarcissJumpState : IState
    {
        protected NarcissFSM manager;
        protected NarcissStateParameter parameter;
        public NarcissJumpState(NarcissFSM manager)
        {
            this.manager = manager;
            this.parameter = manager.parameter as NarcissStateParameter;
        }
        public void OnEnter()
        {
            parameter.animator.SetTrigger("Teleport");
        }

        public void OnExit()
        {
        }

        public void OnFixedUpdate()
        {
        }

        public void OnUpdate()
        {
            var info = parameter.animator.GetCurrentAnimatorStateInfo(0);
            if (info.IsName("teleport") &&info.normalizedTime > 0.95f)
            {
                switch (parameter.bossCurrStage)
                {
                    case BossStage.Stage1:
                        //manager.MoveToResetAIPoint(manager.CurrActivePlayer.position);
                        //manager.MoveToFartherChasePoint(manager.CurrActivePlayer.position);
                        manager.MoveToResetAIPoint(manager.CurrActivePlayer.position + new Vector3(0, 0.3f));
                        manager.MoveToFartherChasePoint(manager.CurrActivePlayer.position + new Vector3(0, 0.3f));
                        manager.TransitionState(StateType.Chase);
                        break;
                    case BossStage.Stage2:
                        break;
                    case BossStage.Stage3:
                        manager.MoveToResetAIPoint(manager.CurrActivePlayer.position + new Vector3(0, 0.3f));
                        manager.MoveToFartherChasePoint(manager.CurrActivePlayer.position + new Vector3(0, 0.3f));
                        manager.TransitionState(StateType.Chase);
                        break;
                    default:
                        break;
                }
            }
        }
    }

    public class NarcissHitState : CustomHitState
    {
        protected new NarcissFSM manager;
        protected new NarcissStateParameter parameter;

        protected StateType _lastNotHitType;
        public NarcissHitState(NarcissFSM manager) : base(manager)
        {
            this.manager = manager;
            this.parameter = manager.parameter as NarcissStateParameter;
        }

        public override void OnEnter()
        {
            parameter.health -= parameter.Damage;
            if (parameter.health < 0)
                parameter.health = 0;
            onEnterAnimaProcess();

            //血条更新
            if (!manager.BossHealthBar)
            {
                UIDynamicArgs uiDynamic = new UIDynamicArgs();
                uiDynamic.father = manager.transform;
                uiDynamic.offset = new Vector2(0, manager.CharacterHight + 0.5f);
                var hpbar = UIManager.Instance.GetDynamicUI<UIBossHealthBar>(Consts.UIBossHealthBar, manager.GetInstanceID(), uiDynamic, (UIBossHealthBar hpBar) =>
                {
                    hpBar.transform.position = new Vector3(hpBar.transform.position.x + uiDynamic.offset.x,
                    hpBar.transform.position.y + uiDynamic.offset.y, hpBar.transform.position.z);
                });
                manager.BossHealthBar = hpbar;
            }
            manager.BossHealthBar.UpdateHealth(parameter.health > 0 ? parameter.health / _maxHp : -1);



            switch (parameter.bossCurrStage)
            {
                case BossStage.Stage1:
                    if (parameter.health / _maxHp <= 0.66f)
                    {
                        manager.TransitionStage(BossStage.Stage2);
                    }
                    break;
                case BossStage.Stage2:
                    break;
                case BossStage.Stage3:
                    break;
                default:
                    break;
            }
            if (manager.LastStateType != StateType.Hit)
                _lastNotHitType = manager.LastStateType;
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
            else if (parameter.getHit)
            {
                manager.TransitionState(StateType.Hit);
            }
            else if (manager.LastStateType == StateType.React && !parameter.animator.GetBool("Fly"))
            {
                manager.TransitionState(StateType.Chase);
            }
            else
                manager.TransitionState(_lastNotHitType);
        }
    }

    public class NarcissAttackState : CustomAttackState
    {
        protected new NarcissFSM manager;
        protected new NarcissStateParameter parameter;

        int m_meleeAttackCount = 0;
        int m_laserAttackCount = 0;
        public NarcissAttackState(NarcissFSM manager) : base(manager)
        {
            this.manager = manager;
            this.parameter = manager.parameter as NarcissStateParameter;
            m_meleeAttackCount = manager.Stage3MeleeAttackPrepareCount;
            m_laserAttackCount = manager.Stage3LaserAttackPrepareCount;
        }

        protected override void onEnterAnimaProcess()
        {
            if (m_laserAttackCount != -1)
                parameter.animator.SetTrigger("Attack");
        }

        public override void OnEnter()
        {
            switch (parameter.bossCurrStage)
            {
                case BossStage.Stage1:
                    parameter.animator.SetFloat("AttackType", 0);
                    break;
                case BossStage.Stage2:
                    break;
                case BossStage.Stage3:
                    if (m_meleeAttackCount == 0)
                    {
                        m_meleeAttackCount = -1;
                        parameter.animator.SetFloat("AttackType", 2);
                    }
                    else if (m_laserAttackCount == 0)
                    {
                        m_laserAttackCount = -1;
                        manager.TransitionState(StateType.React);
                    }
                    else
                    {
                        parameter.animator.SetFloat("AttackType", 0);
                    }
                    break;
                default:
                    break;
            }
            base.OnEnter();
        }

        public override void OnUpdate()
        {
            var info = parameter.animator.GetCurrentAnimatorStateInfo(0);

            if (parameter.getHit&& m_meleeAttackCount >0 )
            {
                manager.TransitionState(StateType.Hit);
            }
            else if (info.normalizedTime >= .95f)    //攻击动画时间片进度大于等于0.95时转追击
            {
                manager.TransitionState(StateType.Chase);
            }
        }

        public override void OnExit()
        {
            base.OnExit();


            if (m_laserAttackCount > 0)
                m_laserAttackCount--;
            if (m_meleeAttackCount > 0)
                m_meleeAttackCount--;

            if (m_meleeAttackCount < 0)
            {
                m_meleeAttackCount = manager.Stage3MeleeAttackPrepareCount;
                parameter.animator.SetFloat("AttackType", 0);
            }
            if (m_laserAttackCount < 0)
                m_laserAttackCount = manager.Stage3LaserAttackPrepareCount;

        }
    }

    public class NarcissReactState : IState
    {

        NarcissFSM manager;
        NarcissStateParameter parameter;
        public NarcissReactState(NarcissFSM manager)
        {
            this.manager = manager;
            this.parameter = manager.parameter as NarcissStateParameter;
        }

        public void OnEnter()
        {
            if(!parameter.animator.GetBool("Fly"))
                manager.Stage3LaserShoot();
            parameter.animator.SetBool("Fly", true);
        }

        public void OnExit()
        {
        }

        public void OnFixedUpdate()
        {
        }

        public void OnUpdate()
        {

            if(parameter.getHit)
            {
                  manager.TransitionState(StateType.Hit);
            }

        }
    }

}

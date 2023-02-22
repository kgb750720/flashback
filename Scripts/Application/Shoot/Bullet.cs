using System.Collections.Generic;
using EffectSystem;
using UnityEngine;
using UnityEngine.Events;

namespace ShootSystem
{
    public class Bullet : MonoBehaviour
    {
        //�ӵ�ֻ�������ӵ�������,�˺�Tag����Ч
        public float hitShakeTime = 1F;
        public ShakeType hitShakeType;
        public string hitEnemyAudio;
        public string hitWallAudio;
        public string impactEffectName;
        public string HotImpactEffectName;
        public string[] targetTags = { };  //���ܹ�����Ŀ��Tag����
        [HideInInspector] public GameObject attacker;

        protected bool isFire = false;
        protected bool isSlowTime = false;
        protected float timeSlowCoe = 1F;
        protected float startTime;
        protected float maxDistance;
        protected float finalSpeed;
        protected float distance;      //λ��
        protected Vector3 startPos;
        protected Vector3 prePostion;         //��¼λ��

        private float offest;     //���ƫ��
        private float coe;
        private Vector2 originDirection;    //ԭ����
        private UnityAction<SlowTimeArgs> actionSlowTime;
        private UnityAction actionNormalTime;

        private void Awake()
        {
            actionSlowTime = SlowTime;
            actionNormalTime = NormalTime;
            Game.Instance.EventCenter.AddEventListener(Consts.E_BUFF_TimeSlow, actionSlowTime);
            Game.Instance.EventCenter.AddEventListener(Consts.E_BUFF_TimeNormal, actionNormalTime);
        }

        public virtual void Init(Vector2 _v2 ,float speed ,float _maxDistance ,float _coe ,GameObject _go = null , bool _isHot = false)
        {
            if (speed == 0F || _maxDistance == 0F || impactEffectName == null) Debug.LogError("û�������ٶȻ�����Զ�����Impact��Ч��");

            coe = _coe;
            isFire = true;
            maxDistance = _maxDistance;
            startPos = transform.position;

            if (_isHot) impactEffectName = HotImpactEffectName;

            if (_go != null)
            {
                attacker = _go;
                finalSpeed = speed;

                if (Mathf.Abs(_v2.x) < 0.5F)
                {
                    offest = _v2.x;

                    if (_v2.y > 0)
                    {
                        originDirection = Vector2.up;
                        transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, transform.rotation.y, 90F));//��ת��ײ��
                    }
                    else 
                    {
                        originDirection = Vector2.down;
                        transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, transform.rotation.y, -90F));
                    } 
                }
                else
                {
                    offest = _v2.y;

                    if (_v2.x > 0)
                    {
                        originDirection = Vector2.right;
                        transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, transform.rotation.y, 0));
                    }
                    else
                    {
                        originDirection = Vector2.left;
                        transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, transform.rotation.y, 180F));
                    }
                }
            } 
        }

        private int JudgeDirectionForHurt(Vector2 _direction)
        {
            if (_direction == Vector2.right)
            {
                return -1;
            }
            else if (_direction == Vector2.left)
            {
                return 1;
            }
            else
            {
                int random = Random.Range(0, 2);
                return random == 0 ? -1 : 1;
            }
        }

        #region (��ײ)
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (targetTags.Length == 0)
            {
                Debug.LogError("û�и��ӵ�������ײ�����ǩ");
                return;
            }
            for (int i = 0; i < targetTags.Length; ++i)
            {
                if (collision.tag == targetTags[i])
                {
                    TriggerToDo(collision);
                    RecycleObject(gameObject);
                    break;
                }
            }
        }

        protected virtual void OnCollisionEnter2D(Collision2D collision)
        {
            OnTriggerEnter2D(collision.collider);
        }

        protected virtual void TriggerToDo(Collider2D _collision)
        {
            //�ӵ��Ĳ�ͬ����ЧҪͨ�����ʵ��,����ʹ��Switch
            //��������ǽ�ɫ��������жϽ�ɫǰ�Ƿ��ж�
            //������Ըĳ�ʹ���¼��ַ�

            if (_collision.tag == "Player")
            {
                if (_collision.GetComponent<SkillSystem.ShieldDeployer>() != null && _collision.GetComponent<SkillSystem.ShieldDeployer>().shield != 0)
                {
                    _collision.GetComponent<SkillSystem.ShieldDeployer>().BeHurt(1);
                }
                else
                {
                    _collision.GetComponent<RealCharacterController>().Hurt(JudgeDirectionForHurt(originDirection), 1);
                }
            }
            else if (_collision.tag == "Enemy")
            {
                //ǹ��Ҫ���Լ��Ĺ���ϵ��
                if (_collision.GetComponent<FSM>() != null)
                {
                    _collision.GetComponent<FSM>().parameter.Damage = (attacker.GetComponentInParent<PlayerManager>().playerAttack * coe);

                    if (hitEnemyAudio != null) MusicManager.Instance.PlaySound(hitEnemyAudio, null, false);
                    Game.Instance.CameraManager.DoShake(hitShakeType, hitShakeTime);
                    Game.Instance.TimeManager.SetBulletTime(attacker.GetComponent<FireController>().bulletSlow);
                }
            }
            else if (_collision.tag == "CanDestroy")
            {
                _collision.GetComponent<InteractiveSystem.CanDestroy>().BeHurt((int)(attacker.GetComponentInParent<PlayerManager>().playerAttack * coe));
            }
            else if (_collision.tag == "Wall")
            {
                MusicManager.Instance.PlaySound(hitWallAudio, null, false);
            }
            EffectManager.Instance.AddWorldEffect3D(impactEffectName, transform.position, -1 * originDirection);
        }

        protected virtual void RaycastTrigger()
        {
            if (Physics2D.Raycast(prePostion, new Vector3(1, offest, 0), distance, LayerMask.GetMask("Enemy") | LayerMask.GetMask("CollisionEneny")).collider != null)
            {
                //�����㼶��Ҫʹ�������|
                //���߼��
                OnTriggerEnter2D(Physics2D.Raycast(prePostion, new Vector3(1, offest, 0), distance, LayerMask.GetMask("Enemy") | LayerMask.GetMask("CollisionEneny")).collider);
            }
        }

        //���ղ���
        protected virtual void RecycleObject(GameObject _gameObject)
        {
            ObjectPool.Instance.RecycleObject(gameObject);
        }
        #endregion

        #region (Action)
        private void SlowTime(SlowTimeArgs st)
        {
            timeSlowCoe = st.magnification;
            isSlowTime = true;
        }

        private void NormalTime()
        {
            timeSlowCoe = 1;
            isSlowTime = false;
        }
        #endregion

        #region (Update)
        protected virtual void BulletMovement()
        {
            transform.Translate(new Vector3(1, offest, 0) * finalSpeed * timeSlowCoe * Time.deltaTime);
        }

        protected virtual void Distance()
        {
            if (maxDistance != 0 && (Mathf.Abs(attacker.transform.position.x - transform.position.x) >= maxDistance
                || Mathf.Abs(attacker.transform.position.y - transform.position.y) >= maxDistance))
            {
                RecycleObject(gameObject);
            }
            else if (maxDistance == 0)
            {
                Debug.LogError("û�и��ӵ�����������");
            }
        }
        #endregion

        private void OnEnable() //��ϴ������
        {
            isFire = false;
            maxDistance = 0;
        }

        private void OnDisable()
        {
            isFire = false;
        }

        private void OnDestroy()
        {
            Game.Instance.EventCenter.RemoveEventListener(Consts.E_BUFF_TimeSlow, actionSlowTime);
            Game.Instance.EventCenter.RemoveEventListener(Consts.E_BUFF_TimeNormal, actionNormalTime);
        }

        private void Update()
        {
            if (isFire)
            {
                prePostion = transform.position;
                BulletMovement();
                distance = (prePostion - transform.position).magnitude;

                RaycastTrigger();
                Distance();
            }
        }
    }
}

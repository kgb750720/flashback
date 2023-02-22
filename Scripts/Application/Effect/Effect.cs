using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace EffectSystem
{
    public class Effect : MonoBehaviour
    {
        private bool forever;
        private float playTime;
        private float recycleTime;
        private ParticleSystem ps;
        private EffectData data;
        private Transform parent;

        private UnityAction<SlowTimeArgs> actionSlowTime;
        private UnityAction actionNormalTime;

        private void Start()
        {
            actionSlowTime = SlowTime;
            actionNormalTime = NormalTime;
            Game.Instance.EventCenter.AddEventListener(Consts.E_BUFF_TimeSlow, actionSlowTime);
            Game.Instance.EventCenter.AddEventListener(Consts.E_BUFF_TimeNormal, actionNormalTime);
        }

        public void Init(EffectData _data)
        {
            data = _data;
            forever = false;
            ps = transform.GetComponent<ParticleSystem>();
            if (ps == null) 
                ps = transform.GetComponentInChildren<ParticleSystem>();
            recycleTime = ps.main.duration;

            SetActive(false);
        }

        public void Play(float _scale = 0F)
        {
            SetActive(true);
            playTime = 0;
            if (parent != null) transform.SetParent(parent);

            ps.Play();

            if (_scale != 0) SetScale(transform,_scale);
        }

        public void SetScale(Transform _trans, float _scale)
        {
            for (int i = 0; i < _trans.childCount; ++i)
                SetScale(_trans.GetChild(i),_scale);

            _trans.localScale = new Vector3(_scale, _scale, _scale);
        }

        public void SetParent(Transform _parent)
        {
            parent = _parent;
        }

        //forever代表回收不会由粒子的生命周期决定
        public void RecycleEffect(float _reTime, bool _forever = false)
        {
            recycleTime = _reTime;
            if (_forever) forever = _forever;
        }

        public void RecycleEffectPercentage(float _reTime)
        {
            if (_reTime <= 0 || _reTime > 1)
            {
                Debug.LogError("设置的参数不为百分比参数,请重新设计");
            }
            recycleTime = ps.main.duration * _reTime;
        }

        private void SetActive(bool _active)
        {
            if (transform.childCount != 0)
            {
                for (int i = 0; i < transform.childCount; ++i)
                {
                    transform.GetChild(i).gameObject.SetActive(_active);
                }
            }
            gameObject.SetActive(_active);
        }

        private Transform FindChild(Transform _trans,string _targetName)
        {
            Transform target = _trans.Find(_targetName);

            if (target != null)
            {
                return target;
            }

            for (int i = 0; i < _trans.childCount; ++i)
            {
                target = FindChild(_trans.GetChild(i), _targetName);
                if (target != null) return target;
            }

            return null;
        }

        #region (Action)
        private void SlowTime(SlowTimeArgs st)
        {
            SetSimulate(st.magnification * Time.deltaTime);
        }

        private void NormalTime()
        {
            SetSimulate(Time.deltaTime);
        }

        private void SetSimulate(float _deltaTime)
        {
            ps.Simulate(_deltaTime,true,false);
        }
        #endregion

        private void OnDisable()
        {
            playTime = 0;
            parent = null;
            forever = false;
        }

        private void Update()
        {
            playTime += Time.deltaTime;

            if (!forever && playTime > recycleTime)
            {
                ObjectPool.Instance.RecycleObject(gameObject);
            }
        }

        private void OnDestroy()
        {
            actionSlowTime = SlowTime;
            actionNormalTime = NormalTime;
            Game.Instance.EventCenter.RemoveEventListener(Consts.E_BUFF_TimeSlow, actionSlowTime);
            Game.Instance.EventCenter.RemoveEventListener(Consts.E_BUFF_TimeNormal, actionNormalTime);
        }
    }
}


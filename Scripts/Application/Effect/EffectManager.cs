using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EffectSystem
{
    public class EffectManager : SingletonMono<EffectManager>
    {
        //在后面会将统一的数据放在Static的数据类中,该类会取消单例,只保留方法

        public EffectData[] effects;
        //public List<EffectData> effects;

        public Dictionary<string, List<Effect>> effectPool = new Dictionary<string, List<Effect>>();
        //字典<特效名字，某种特效的数量>

        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            //初始化effect的所有资源
            if (effects != null && effectPool.Count == 0)
            {
                for (int i = 0; i < effects.Length; ++i)
                {
                    EffectData data = effects[i];
                    if (data.maxInScene == 0) data.maxInScene = 1;
                    //当没有设置缓存数量,就看作是1,也就是退化成对象池逻辑

                    for (int j = 0; j < data.maxInScene; ++j)
                    {
                        CreateEffect(data);
                    }
                }
            }
        }

        //添加特效到世界坐标系
        public Effect AddWorldEffect3D(string _name, Vector3 _position, Vector2 _direction)
        {
            Effect effect = GetEffect(_name);
            if (effect == null) return null;

            effect.transform.position = _position;
            effect.transform.rotation = Quaternion.Euler(GetRotation(effect, _direction));
            effect.Play();
            return effect;
        }

        public Effect AddWorldEffect2D(string _name, Vector3 _position)
        {
            Effect effect = GetEffect(_name);
            if (effect == null) return null;

            effect.transform.position = _position;
            effect.Play();
            return effect;
        }

        //与上述方法不同的是这里的参数V3是相对于父类的偏移量
        public Effect AddEffect3D(string _name, Vector3 _position, Vector2 _direction, GameObject _obj)
        {
            if (_obj == null)
            {
                return AddWorldEffect3D(_name, _position, _direction);
            }

            Effect effect = GetEffect(_name);
            if (effect == null) return null;

            effect.SetParent(_obj.transform);
            effect.transform.position = _position;
            effect.transform.rotation = Quaternion.Euler(GetRotation(effect, _direction));
            effect.Play();

            return effect;
        }

        public Effect AddEffect2D(string _name, Vector3 _position, GameObject _obj)
        {
            if (_obj == null)
            {
                return AddWorldEffect2D(_name, _position);
            }
            Effect effect = GetEffect(_name);
            if (effect == null) return null;

            effect.SetParent(_obj.transform);
            effect.transform.position = _position;
            effect.Play();

            return effect;
        }

        private Effect GetEffect(string _name)
        {
            if (!effectPool.ContainsKey(_name))
            {
                return null;
            }

            List<Effect> pool = effectPool[_name];

            Effect ef = null;

            for (int i = 0; i < pool.Count; ++i)
            {
                Effect temp = pool[i];

                if (temp.gameObject.activeSelf) continue;

                ef = temp;
                break;
            }

            if (ef == null)
            {
                for (int j = 0; j < effects.Length; ++j)
                {
                    if (_name == effects[j].effectName && effects[j] != null)
                    {
                        ef = CreateEffect(effects[j]);
                        break;
                    }
                }
            }
            return ef;
        }

        //添加特效到缓存池
        private Effect CreateEffect(EffectData _data)
        {
            GameObject obj = ObjectPool.Instance.GetObject(_data.effectPath);
            if (obj == null)
            {
                Debug.LogError("找不到特效路径" + _data.effectPath);
                return null;
            }

            Effect effect = obj.GetComponent<Effect>();

            if (effect == null) Debug.LogError(_data.effectName + " 没有添加必要的组件Effect");

            effect.Init(_data);

            if (!effectPool.ContainsKey(_data.effectName))
            {
                effectPool.Add(_data.effectName, new List<Effect>());
            }

            effectPool[_data.effectName].Add(effect);

            return effect;
        }

        private Vector3 GetRotation(Effect _effect, Vector2 _direction)
        {
            float z = _effect.transform.rotation.z;
            Vector3 result = Vector3.zero;

            if (_direction == Vector2.right)
            {
                result = new Vector3(0, 90, z);
            }
            else if (_direction == Vector2.left)
            {
                result = new Vector3(0, -90, z);
            }
            else if (_direction == Vector2.up)
            {
                result = new Vector3(-90, 90, z);
            }
            else if (_direction == Vector2.down)
            {
                result = new Vector3(90, -90, z);
            }
            else
            {
                return Vector3.zero;
            }
            return result;
        }

        private void OnDestroy()
        {
            effectPool.Clear();
        }
    }
}

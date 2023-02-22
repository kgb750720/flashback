using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SkillSystem
{
    public interface IBUFFInfo { }

    public class BUFFInfo<T> : IBUFFInfo
    {
        public UnityAction<T> actions;
        public BUFFInfo(UnityAction<T> _action)
        {
            actions += _action;
        }
    }

    public class CDController : MonoBehaviour
    {
        //SkillManager AddComponent

        //还要有持续时间比如盾

        //仅仅管理处于CD状态的BUFF和技能
        private float shareCD = 0F;
        private System.Action actionShareCD;

        //配合字典进行索引,需注意同增同减
        private List<BUFF> buffsArray = new List<BUFF>();
        private List<SkillData> skillsArray = new List<SkillData>();

        private Dictionary<BuffType, System.Action<BUFF>> cdCallBack = new Dictionary<BuffType, System.Action<BUFF>>();
        //字典——<BUFF类型，回调>
        private Dictionary<BuffType, BUFF> cDDic = new Dictionary<BuffType, BUFF>();
        //字典——<BUFF类型,BUFF对象>
        private Dictionary<int, System.Action> normalCallBack = new Dictionary<int, System.Action>();
        //字典——<技能ID,回调>
        private Dictionary<int, SkillData> normalDic = new Dictionary<int, SkillData>();
        //字典——<技能ID,SkillData对象>

        #region(BUFF CD)
        public void AddBuffCD(BUFF _buff, System.Action<BUFF> _action)
        {
            if (cDDic.ContainsKey(_buff.buffType))
            {
                //添加CD时间;
                AddBuffContinueTime(_buff, _buff.durationTime);
            }
            else
            {
                _buff.cDRemain = _buff.durationTime;        //初始化cd剩余时间为持续时间

                buffsArray.Add(_buff);
                cDDic.Add(_buff.buffType, _buff);
            }

            if (_action == null) return;

            if (!cdCallBack.ContainsKey(_buff.buffType))
            {
                cdCallBack.Add(_buff.buffType, _action);
            }
            else if (cdCallBack.ContainsKey(_buff.buffType))
            {
                cdCallBack[_buff.buffType] += _action;
            }
        }

        public BUFF GetBuff(BuffType _buffType)
        {
            return cDDic.ContainsKey(_buffType) == true ? cDDic[_buffType] : null;
        }

        public float GetBuffCDPre(BuffType _buffType)
        {
            if (cDDic.ContainsKey(_buffType))
            {
                return cDDic[_buffType].cDRemain / cDDic[_buffType].durationTime;
            }
            else return 0F;
        }

        public void AddBuffContinueTime(BUFF _buff, float _continueTime)
        {
            if (GetBuff(_buff.buffType) != null) GetBuff(_buff.buffType).cDRemain += _continueTime;
            else Debug.LogError("想要延长Buff时间的Buff对象不存在");
        }

        private void BuffCoolTimeDown()
        {
            for (int i = 0; i < buffsArray.Count; ++i)
            {
                buffsArray[i].cDRemain -= Time.deltaTime;

                if (buffsArray[i].cDRemain <= 0)
                {
                    Remove(buffsArray[i].buffType);
                }
            }
        }

        private void Remove(BuffType _buffType)
        {
            if (cdCallBack.ContainsKey(_buffType))
            {
                cdCallBack[_buffType](GetBuff(_buffType));
                cdCallBack.Remove(_buffType);
            }

            if (cDDic.ContainsKey(_buffType))
            {
                buffsArray.Remove(GetBuff(_buffType));
                cDDic.Remove(_buffType);
            }
        }
        #endregion

        #region(Normal CD)
        public void AddNormalSkillCD(int _id, SkillData _skillData, System.Action _action = null)
        {
            if (normalDic.ContainsKey(_id))
            {
                Debug.LogError("技能尚未冷却完毕,不可以开始倒计时");
                return;
            }
            else
            {
                _skillData.cDRemain = _skillData.skillCD;
                skillsArray.Add(_skillData);
                normalDic.Add(_id, _skillData);
            }

            if (_action == null) return;

            if (!normalCallBack.ContainsKey(_id))
            {
                normalCallBack.Add(_id, _action);
            }
            else if (normalCallBack.ContainsKey(_id))
            {
                normalCallBack[_id] += _action;
            }
        }

        public SkillData GetSkillData(int _id)
        {
            return normalDic.ContainsKey(_id) == true ? normalDic[_id] : null;
        }

        public float GetNormalCD(int _id)
        {
            return normalDic.ContainsKey(_id) == true ? normalDic[_id].cDRemain : 0F;
        }

        public float GetNormalCDPer(int _id)
        {
            if (normalDic.ContainsKey(_id))
            {
                return normalDic[_id].cDRemain / normalDic[_id].skillCD;
            }
            else return 0F;
        }

        private void NormalCoolTimeDown()
        {
            for (int i = 0; i < skillsArray.Count; ++i)
            {
                skillsArray[i].cDRemain -= Time.deltaTime;

                if (skillsArray[i].cDRemain <= 0)
                {
                    NormalRemove(skillsArray[i].skillID);
                }
            }
        }

        private void NormalRemove(int _id)
        {
            if (normalDic.ContainsKey(_id))
            {
                if (normalCallBack.ContainsKey(_id))
                {
                    normalCallBack[_id]();
                    normalCallBack.Remove(_id);
                }

                skillsArray.Remove(GetSkillData(_id));
                normalDic.Remove(_id);
            }
        }
        #endregion

        #region(Share CD)
        public void StartShareCD(float _CDTime, System.Action _action = null)
        {
            shareCD = _CDTime;
            if (_action != null) actionShareCD = _action;
        }

        public float GetShareCD()
        {
            return shareCD;
        }
        #endregion

        private void Update()
        {
            if (cDDic.Count != 0)
            {
                BuffCoolTimeDown();
            }

            if (normalDic.Count != 0)
            {
                NormalCoolTimeDown();
            }

            if (shareCD > 0)
            {
                shareCD -= Time.deltaTime;

                if (shareCD <= 0)
                {
                    shareCD = 0;
                }
            }
        }
    }
}

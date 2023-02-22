using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


/// <summary>
/// EventsManager 类专用的消息封装接口
/// </summary>
public class EventsManagerBaseMsg { }

namespace CharaterMsg
{
    public  class MsgOnEventManagerDestryGO : EventsManagerBaseMsg { }


    public static class EventsManager<Msg> where Msg: EventsManagerBaseMsg
    {
        static Dictionary<GameObject, UnityEvent> m_gameObjectEvents = new Dictionary<GameObject, UnityEvent>();

        static UnityEvent m_Events = new UnityEvent();

        public static void AddListener(UnityAction call)
        {
            m_Events.AddListener(call);
        }

        public static void RemoveListener(UnityAction call)
        {
            m_Events.RemoveListener(call);
        }


        public static void Invoke(GameObject go)
        {
            if (go && m_gameObjectEvents.ContainsKey(go))
                m_gameObjectEvents[go]?.Invoke();
        }

        public static void AddListener(GameObject go, UnityAction call)
        {
            if (!m_gameObjectEvents.ContainsKey(go))
                m_gameObjectEvents.Add(go, new UnityEvent());
            m_gameObjectEvents[go].AddListener(call);
        }

        

        public static void RemoveListener(GameObject go, UnityAction call)
        {
            if (m_gameObjectEvents.ContainsKey(go))
            {
                m_gameObjectEvents[go].RemoveListener(call);
            }
        }

        

        public static void RemoveGameObject(GameObject go)
        {
            if (m_gameObjectEvents.ContainsKey(go))
                m_gameObjectEvents.Remove(go);
        }

        /// <summary>
        /// 物体事件集内事件个数，-1为未创建事件集
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        public static int GetEventsCount(GameObject go)
        {
            if (!m_gameObjectEvents.ContainsKey(go))
                return -1;
            return m_gameObjectEvents[go].GetPersistentEventCount();
        }

    }

    public static class EventsManager<Msg,ArgType> where Msg : EventsManagerBaseMsg
    {
        static Dictionary<GameObject, UnityEvent<ArgType>> m_gameObjectEvents = new Dictionary<GameObject, UnityEvent<ArgType>>();

        static UnityEvent<ArgType> m_Events = new UnityEvent<ArgType>();

        public static void AddListener(UnityAction<ArgType> call)
        {
            m_Events.AddListener(call);
        }

        public static void RemoveListener(UnityAction<ArgType> call)
        {
            m_Events.RemoveListener(call);
        }

        public static void Invoke(GameObject go, ArgType arg)
        {
            if (go && m_gameObjectEvents.ContainsKey(go))
                m_gameObjectEvents[go]?.Invoke(arg);
        }

        public static void AddListener(GameObject go, UnityAction<ArgType> call)
        {
            if (!m_gameObjectEvents.ContainsKey(go))
                m_gameObjectEvents.Add(go, new UnityEvent<ArgType>());
            m_gameObjectEvents[go].AddListener(call);
        }

        public static void RemoveListener(GameObject go, UnityAction<ArgType> call)
        {
            if (m_gameObjectEvents.ContainsKey(go))
            {
                m_gameObjectEvents[go].RemoveListener(call);
            }
        }

        public static void RemoveGameObject(GameObject go)
        {
            if (m_gameObjectEvents.ContainsKey(go))
                m_gameObjectEvents.Remove(go);
        }

        /// <summary>
        /// 物体事件集内事件个数，-1为未创建事件集
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        public static int GetEventsCount(GameObject go)
        {
            if (!m_gameObjectEvents.ContainsKey(go))
                return -1;
            return m_gameObjectEvents[go].GetPersistentEventCount();
        }
    }

    public static class EventsManager<Msg, ArgType0, ArgType1> where Msg : EventsManagerBaseMsg
    {
        static Dictionary<GameObject, UnityEvent<ArgType0, ArgType1>> m_gameObjectEvents = new Dictionary<GameObject, UnityEvent<ArgType0, ArgType1>>();

        static UnityEvent<Msg, ArgType0, ArgType1> m_Events = new UnityEvent<Msg, ArgType0, ArgType1>();

        public static void AddListener(UnityAction<Msg, ArgType0, ArgType1> call)
        {
            m_Events.AddListener(call);
        }

        public static void RemoveListener(UnityAction<Msg, ArgType0, ArgType1> call)
        {
            m_Events.RemoveListener(call);
        }

        public static void Invoke(GameObject go, ArgType0 arg0, ArgType1 arg1)
        {
            if (go && m_gameObjectEvents.ContainsKey(go))
                m_gameObjectEvents[go]?.Invoke(arg0, arg1);
        }

        public static void AddListener(GameObject go, UnityAction<ArgType0, ArgType1> call)
        {
            if (!m_gameObjectEvents.ContainsKey(go))
                m_gameObjectEvents.Add(go, new UnityEvent<ArgType0, ArgType1>());
            m_gameObjectEvents[go].AddListener(call);
        }

        public static void RemoveListener(GameObject go, UnityAction<ArgType0, ArgType1> call)
        {
            if (m_gameObjectEvents.ContainsKey(go))
            {
                m_gameObjectEvents[go].RemoveListener(call);
            }
        }

        public static void RemoveGameObject(GameObject go)
        {
            if (m_gameObjectEvents.ContainsKey(go))
                m_gameObjectEvents.Remove(go);
        }

        /// <summary>
        /// 物体事件集内事件个数，-1为未创建事件集
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        public static int GetEventsCount(GameObject go)
        {
            if (!m_gameObjectEvents.ContainsKey(go))
                return -1;
            return m_gameObjectEvents[go].GetPersistentEventCount();
        }
    }

    public static class EventsManager<Msg,ArgType0, ArgType1, ArgType2> where Msg : EventsManagerBaseMsg
    {
        static Dictionary<GameObject, UnityEvent<ArgType0, ArgType1, ArgType2>> m_gameObjectEvents = new Dictionary<GameObject, UnityEvent<ArgType0, ArgType1, ArgType2>>();

        static UnityEvent<Msg, ArgType0, ArgType1, ArgType2> m_Events = new UnityEvent<Msg, ArgType0, ArgType1, ArgType2>();

        public static void AddListener(UnityAction<Msg, ArgType0, ArgType1, ArgType2> call)
        {
            m_Events.AddListener(call);
        }

        public static void RemoveListener(UnityAction<Msg, ArgType0, ArgType1, ArgType2> call)
        {
            m_Events.RemoveListener(call);
        }

        public static void Invoke(GameObject go, ArgType0 arg0, ArgType1 arg1, ArgType2 arg2)
        {
            if (go && m_gameObjectEvents.ContainsKey(go))
                m_gameObjectEvents[go]?.Invoke(arg0, arg1, arg2);
        }

        public static void AddListener(GameObject go, UnityAction<ArgType0, ArgType1, ArgType2> call)
        {
            if (!m_gameObjectEvents.ContainsKey(go))
                m_gameObjectEvents.Add(go, new UnityEvent<ArgType0, ArgType1, ArgType2>());
            m_gameObjectEvents[go].AddListener(call);
        }

        public static void RemoveListener(GameObject go, UnityAction<ArgType0, ArgType1, ArgType2> call)
        {
            if (m_gameObjectEvents.ContainsKey(go))
            {
                m_gameObjectEvents[go].RemoveListener(call);
            }
        }

        public static void RemoveGameObject(GameObject go)
        {
            if (m_gameObjectEvents.ContainsKey(go))
                m_gameObjectEvents.Remove(go);
        }

        /// <summary>
        /// 物体事件集内事件个数，-1为未创建事件集
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        public static int GetEventsCount(GameObject go)
        {
            if (!m_gameObjectEvents.ContainsKey(go))
                return -1;
            return m_gameObjectEvents[go].GetPersistentEventCount();
        }
    }

    public static class EventsManager<Msg, ArgType0, ArgType1, ArgType2, ArgType3> where Msg : EventsManagerBaseMsg
    {
        static Dictionary<GameObject, UnityEvent<ArgType0, ArgType1, ArgType2, ArgType3>> m_gameObjectEvents = new Dictionary<GameObject, UnityEvent<ArgType0, ArgType1, ArgType2, ArgType3>>();


        static UnityEvent<ArgType0, ArgType1, ArgType2, ArgType3> m_Events = new UnityEvent<ArgType0, ArgType1, ArgType2, ArgType3>();

        public static void AddListener(UnityAction<ArgType0, ArgType1, ArgType2, ArgType3> call)
        {
            m_Events.AddListener(call);
        }

        public static void RemoveListener(UnityAction< ArgType0, ArgType1, ArgType2, ArgType3> call)
        {
            m_Events.RemoveListener(call);
        }

        public static void Invoke(GameObject go,   ArgType0 arg0, ArgType1 arg1, ArgType2 arg2, ArgType3 arg3)
        {
            if (go && m_gameObjectEvents.ContainsKey(go))
                m_gameObjectEvents[go]?.Invoke(arg0, arg1, arg2, arg3);
        }

        public static void AddListener(GameObject go,   UnityAction<ArgType0, ArgType1, ArgType2, ArgType3> call)
        {
            if (!m_gameObjectEvents.ContainsKey(go))
                m_gameObjectEvents.Add(go, new UnityEvent<ArgType0, ArgType1, ArgType2, ArgType3>());
            m_gameObjectEvents[go].AddListener(call);
        }

        public static void RemoveListener(GameObject go, UnityAction<ArgType0, ArgType1, ArgType2, ArgType3> call)
        {
            if (m_gameObjectEvents.ContainsKey(go))
            {
                m_gameObjectEvents[go].RemoveListener(call);
            }
        }

        public static void RemoveGameObject(GameObject go)
        {
            if (m_gameObjectEvents.ContainsKey(go))
                m_gameObjectEvents.Remove(go);
        }

        /// <summary>
        /// 物体事件集内事件个数，-1为未创建事件集
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        public static int GetEventsCount(GameObject go)
        {
            if (!m_gameObjectEvents.ContainsKey(go))
                return -1;
            return m_gameObjectEvents[go].GetPersistentEventCount();
        }
    }
}
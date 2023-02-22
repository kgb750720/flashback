using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IEventInfo
{
    
}

public class EventInfo<T> : IEventInfo
{
    public UnityAction<T> actions;

    public EventInfo(UnityAction<T> action)
    {
        actions += action;
    }
}

public class EventInfo : IEventInfo
{
    public UnityAction actions;

    public EventInfo(UnityAction action)
    {
        actions += action;
    }
}

public class EventCenter : SingletonMono<EventCenter>
{
    //key -- 事件的名称
    //value -- 需要触发的函数
    private Dictionary<string, IEventInfo> eventDic = new Dictionary<string, IEventInfo>();

    /// <summary>
    /// 添加事件监听
    /// </summary>
    /// <param name="name"></param>
    /// <param name="action"></param>
    public void AddEventListener<T>(string name, UnityAction<T> action)
    {
        //判定有没有对应事件监听
        if (eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo<T>).actions += action;
        }
        else
        {
            eventDic.Add(name, new EventInfo<T>(action));
        }
    }
    
    public void AddEventListener(string name, UnityAction action)
    {
        
        //判定有没有对应事件监听
        if (eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo).actions += action;
        }
        else
        {
            eventDic.Add(name, new EventInfo(action));
        }
    }

    public void RemoveEventListener<T>(string name, UnityAction<T> action)
    {
        if (eventDic.ContainsKey(name)) (eventDic[name] as EventInfo<T>).actions -= action;
    }
    
    public void RemoveEventListener(string name, UnityAction action)
    {
        if (eventDic.ContainsKey(name)) (eventDic[name] as EventInfo).actions -= action;
    }
    
    /// <summary>
    /// 事件触发
    /// </summary>
    /// <param name="name"></param>
    public void EventTrigger<T>(string name, T info)
    {
        if (eventDic.ContainsKey(name))
        {
            if ((eventDic[name] as EventInfo<T>).actions != null)
            {
                (eventDic[name] as EventInfo<T>).actions.Invoke(info);
            }
        }
    }
    
    public void EventTrigger(string name)
    {
        if (eventDic.ContainsKey(name))
        {
            if((eventDic[name] as EventInfo).actions!=null)
                (eventDic[name] as EventInfo).actions.Invoke();
        }
    }
    
    /// <summary>
    /// 清空事件中心，跳场景用
    /// </summary>
    public void Clear()
    {
        eventDic.Clear();
    }
}

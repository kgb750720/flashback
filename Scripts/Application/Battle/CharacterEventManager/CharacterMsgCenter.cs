using CharaterMsg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;





public class CharacterMsgCenter : MonoBehaviour
{
    /// <summary>
    /// OnDestory销毁消息封装
    /// </summary>
    private class MsgOnDestroy : EventsManagerBaseMsg { }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        EventsManager<MsgOnDestroy>.Invoke(gameObject);
        EventsManager<MsgOnDestroy>.RemoveGameObject(gameObject);
    }


    /// <summary>
    /// 注册事件
    /// </summary>
    /// <param name="call"></param>
    public void RegisterMsgEvent<Msg>(UnityAction call) where Msg:EventsManagerBaseMsg
    {
        EventsManager<Msg>.AddListener(gameObject, call);

        //检查是否已经有了同种消息,若有同种消息不必重复绑定销毁
        if (EventsManager<Msg>.GetEventsCount(gameObject) < 0)
        {
            //将回收过程添加到 OnDestroy() 事件中以实现对象销毁时事件同步回收
            EventsManager<MsgOnDestroy>.AddListener(gameObject, () =>
            {
                EventsManager<Msg>.RemoveGameObject(gameObject);
            });
        }
    }

    /// <summary>
    /// 移除事件
    /// </summary>
    /// <param name="call"></param>
    public void RemoveMsgEvent<Msg>(UnityAction call) where Msg: EventsManagerBaseMsg
    {
        //外部无法调用 OnDestroy() 事件，OnDestroy() 由系统管理移除
        EventsManager<Msg>.RemoveListener(gameObject, call);
    }

    /// <summary>
    /// 执行事件
    /// </summary>
    public void BroadcastMsg<Msg>() where Msg : EventsManagerBaseMsg
    {
        EventsManager<Msg>.Invoke(gameObject);
    }

    public void RegisterMsgEvent<Msg, ArgType>(UnityAction<ArgType> call) where Msg : EventsManagerBaseMsg
    {
        EventsManager<Msg, ArgType>.AddListener(gameObject, call);

        //检查是否已经有了同种消息,若有同种消息不必重复绑定销毁
        if (EventsManager<Msg, ArgType>.GetEventsCount(gameObject) < 0)
        {
            //将回收过程添加到 OnDestroy() 事件中以实现对象销毁时事件同步回收
            EventsManager<MsgOnDestroy>.AddListener(gameObject, () =>
            {
                EventsManager<Msg, ArgType>.RemoveGameObject(gameObject);
            });
        }
    }

    public void RemoveMsgEvent<Msg, ArgType>(UnityAction<ArgType> call) where Msg : EventsManagerBaseMsg
    {
        EventsManager<Msg, ArgType>.RemoveListener(gameObject, call);
    }

    public void BroadcastMsg<Msg, ArgType>(ArgType arg) where Msg : EventsManagerBaseMsg
    {
        EventsManager<Msg, ArgType>.Invoke(gameObject, arg);
    }

    public void RegisterMsgEvent<Msg,ArgType0, ArgType1>(UnityAction<ArgType0, ArgType1> call) where Msg : EventsManagerBaseMsg
    {
        EventsManager<Msg,ArgType0, ArgType1>.AddListener(gameObject, call);

        //检查是否已经有了同种消息,若有同种消息不必重复绑定销毁
        if (EventsManager<Msg, ArgType0, ArgType1>.GetEventsCount(gameObject) < 0)
        {
            //将回收过程添加到 OnDestroy() 事件中以实现对象销毁时事件同步回收
            EventsManager<MsgOnDestroy>.AddListener(gameObject, () =>
            {
                EventsManager<Msg, ArgType0, ArgType1>.RemoveGameObject(gameObject);
            });
        }
    }

    public void RemoveMsgEvent<Msg,ArgType0, ArgType1>(EventsManagerBaseMsg msgCenterEvent, UnityAction<ArgType0, ArgType1> call) where Msg : EventsManagerBaseMsg
    {
        EventsManager<Msg, ArgType0, ArgType1>.RemoveListener(gameObject, call);
    }

    public void BroadcastMsg<Msg, ArgType0, ArgType1>(EventsManagerBaseMsg msgCenterEvent, ArgType0 arg0, ArgType1 arg1) where Msg : EventsManagerBaseMsg
    {
        EventsManager<Msg, ArgType0, ArgType1>.Invoke(gameObject, arg0, arg1);
    }


    public void RegisterMsgEvent<Msg, ArgType0, ArgType1, ArgType2>(EventsManagerBaseMsg msgCenterEvent, UnityAction<ArgType0, ArgType1, ArgType2> call) where Msg : EventsManagerBaseMsg
    {
        EventsManager<Msg, ArgType0, ArgType1, ArgType2>.AddListener(gameObject, call);

        //检查是否已经有了同种消息,若有同种消息不必重复绑定销毁
        if (EventsManager<Msg, ArgType0, ArgType1, ArgType2>.GetEventsCount(gameObject) < 0)
        {
            //将回收过程添加到 OnDestroy() 事件中以实现对象销毁时事件同步回收
            EventsManager<MsgOnDestroy>.AddListener(gameObject, () =>
            {
                EventsManager<Msg, ArgType0, ArgType1, ArgType2>.RemoveGameObject(gameObject);
            });
        }
    }
    public void RemoveMsgEvent<Msg, ArgType0, ArgType1, ArgType2>(UnityAction<ArgType0, ArgType1, ArgType2> call) where Msg : EventsManagerBaseMsg
    {
        EventsManager<Msg,ArgType0, ArgType1, ArgType2>.RemoveListener(gameObject, call);
    }

    public void BroadcastMsg<Msg, ArgType0, ArgType1, ArgType2>(EventsManagerBaseMsg msgCenterEvent, ArgType0 arg0, ArgType1 arg1, ArgType2 arg2) where Msg : EventsManagerBaseMsg
    {
        EventsManager<Msg, ArgType0, ArgType1, ArgType2>.Invoke(gameObject, arg0, arg1, arg2);
    }


    public void RegisterMsgEvent<Msg, ArgType0, ArgType1, ArgType2, ArgType3>(UnityAction<ArgType0, ArgType1, ArgType2, ArgType3> call) where Msg : EventsManagerBaseMsg
    {
        EventsManager<Msg, ArgType0, ArgType1, ArgType2, ArgType3>.AddListener(gameObject, call);


        //检查是否已经有了同种消息,若有同种消息不必重复绑定销毁
        if (EventsManager<Msg, ArgType0, ArgType1, ArgType2, ArgType3>.GetEventsCount(gameObject) < 0)
        {
            //将回收过程添加到 OnDestroy() 事件中以实现对象销毁时事件同步回收
            EventsManager<MsgOnDestroy>.AddListener(gameObject, () =>
            {
                EventsManager<Msg, ArgType0, ArgType1, ArgType2, ArgType3>.RemoveGameObject(gameObject);
            });
        }
    }

    public void RemoveMsgEvent<Msg, ArgType0, ArgType1, ArgType2, ArgType3>(UnityAction<ArgType0, ArgType1, ArgType2, ArgType3> call) where Msg : EventsManagerBaseMsg
    {
        EventsManager<Msg, ArgType0, ArgType1, ArgType2, ArgType3>.RemoveListener(gameObject,call);
    }

    public void BroadcastMsg<Msg, ArgType0, ArgType1, ArgType2, ArgType3>(EventsManagerBaseMsg msgCenterEvent, ArgType0 arg0, ArgType1 arg1, ArgType2 arg2, ArgType3 arg3) where Msg : EventsManagerBaseMsg
    {
        EventsManager<Msg, ArgType0, ArgType1, ArgType2, ArgType3>.Invoke(gameObject,arg0, arg1, arg2,arg3);
    }

}

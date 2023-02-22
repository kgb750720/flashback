using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MVC
{
    //存储MVC
    public static Dictionary<string, Model> Models = new Dictionary<string, Model>();  //名字--模型
    public static Dictionary<string, View> Views = new Dictionary<string, View>();    //名字--视图
    public static Dictionary<string, Type> Controllers = new Dictionary<string, Type>();    //事件名字--控制器类型
    
    //注册
    public static void RegisterModel(Model model)
    {
        Models[model.Name] = model;
    }

    public static void RegisterView(View view)
    {
        //防止重复注册
        if (Views.ContainsKey(view.Name))
            Views.Remove(view.Name);

        //注册关心的事件
        view.RegisterEvents();
        
        Views[view.Name] = view;
    }

    public static void RegisterController(string eventName, Type controllerType)
    {
        Controllers[eventName] = controllerType;
    }
    
    //获取
    public static Model GetModel<T>() where T : Model
    {
        foreach (Model model in Models.Values)
        {
            if (model!= null && model is Model)
            {
                return model;
            }
        }

        return null;
    }
    
    public static View GetView<T>() where T : View
    {
        foreach (View view in Views.Values)
        {
            if (view!= null && view is View)
            {
                return view;
            }
        }

        return null;
    }
    
    //发送事件
    public static void SendEvent(string eventName, object data = null)
    {
        //控制器响应事件
        if (Controllers.ContainsKey(eventName))
        {
            Type t = Controllers[eventName];
            Controller c = Activator.CreateInstance(t) as Controller;
            //控制器执行
            c.Execute(data);
        }
        
        //视图响应事件
        foreach (View view in Views.Values)
        {
            if (view.AttentionEvents.Contains(eventName))
            {
                //视图响应事件
                view.HandleEvent(eventName, data);
            }
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Singleton<T> where T : new()
{
    private static T instance;

    public static T GetInstance()
    {
        if(instance == null)
            instance = new T();
        return instance;
    }
}

public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get { return instance; }
    }

    protected virtual void Awake()
    {
        instance = this as T;
    }
}



using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//每一类的层级关系
public class SubPool
{
    //每一类对象所挂载的物体节点（一个类别抽屉）
    public GameObject subPool;
    //每一类的子对象池
    public List<GameObject> poolList;

    //初始化
    public SubPool(GameObject obj, GameObject poolObj, bool isRecycle = true)
    {
        //给抽屉创建一个父对象，作为pool子物体
        subPool = new GameObject(obj.name);
        subPool.transform.parent = poolObj.transform;
        
        poolList = new List<GameObject>(){};
        if(isRecycle) RecycleObject(obj);
        else obj.transform.parent = subPool.transform;
    }
    
    public SubPool(GameObject obj, GameObject poolObj, Vector3 pos, bool isRecycle = true)
    {
        //给抽屉创建一个父对象，作为pool子物体
        subPool = new GameObject(obj.name);
        subPool.transform.parent = poolObj.transform;
        
        poolList = new List<GameObject>(){};
        if(isRecycle) RecycleObject(obj);
        else
        {
            obj.transform.position = pos;
            obj.transform.parent = subPool.transform;
        }
    }

    //往抽屉里放东西
    public void RecycleObject(GameObject obj)
    {
        obj.SetActive(false);
        poolList.Add(obj);
        obj.transform.SetParent(subPool.transform);
    }
    
    //从抽屉里取对象
    public GameObject GetObject(Transform father = null)
    {
        GameObject obj = null;
        obj = poolList[0];
        poolList.RemoveAt(0);
        obj.transform.parent = father;
        obj.SetActive(true);

        return obj;
    }
    
    public GameObject GetObject(Vector3 pos, Transform father = null)
    {
        GameObject obj = null;
        obj = poolList[0];
        poolList.RemoveAt(0);
        obj.transform.position = pos;
        obj.transform.parent = father;
        obj.SetActive(true);

        return obj;
    }
}

//对象池
public class ObjectPool : SingletonMono<ObjectPool>
{
    private Dictionary<string, SubPool> pool = new Dictionary<string, SubPool>();

    private GameObject poolObj = null;

    /*public void GetObjectAsync(string name, UnityAction<GameObject> callback=null)
    {
        if (pool.ContainsKey(name) && pool[name].poolList.Count > 0)
        {
            if(callback != null)
                callback(pool[name].GetObject());
        }
        else
        {
            //通过异步加载资源
            Game.Instance.AssetsManager.LoadAsync<GameObject>(name, (o) =>
            {
                o.name = name;
                if(callback != null)
                    callback(o);
            });
        }
    }*/
    
    public GameObject GetObject(string name, Vector3 pos, Transform father = null)
    {
        if (poolObj == null)
            poolObj = new GameObject("Pool");
        
        if (pool.ContainsKey(name) && pool[name].poolList.Count > 0)
        {
            return pool[name].GetObject(pos, father);
        }
        else
        {
            //通过同步加载资源
            GameObject go = Game.Instance.AssetsManager.Load<GameObject>(name);
            go.name = name;
            
            if (father == null)
            {
                if (pool.ContainsKey(go.name))
                {
                    go.transform.position = pos;
                    go.transform.parent = pool[go.name].subPool.transform;
                }
                else
                {
                    pool.Add(go.name, new SubPool(go, poolObj, pos, false));
                }
            }
            else
            {
                go.transform.position = pos;
                go.transform.parent = father;
            }

            return go;
        }
    }
    
    public GameObject GetObject(string name, Transform father = null)
    {
        if (poolObj == null)
        {
            poolObj = new GameObject("Pool");
            DontDestroyOnLoad(poolObj);
        }
            
        
        if (pool.ContainsKey(name) && pool[name].poolList.Count > 0)
        {
            return pool[name].GetObject(father);
        }
        else
        {
            //通过同步加载资源
            GameObject go = Game.Instance.AssetsManager.Load<GameObject>(name);
            go.name = name;
            
            if (father == null)
            {
                if (pool.ContainsKey(go.name))
                {
                    go.transform.parent = pool[go.name].subPool.transform;
                }
                else
                {
                    pool.Add(go.name, new SubPool(go, poolObj, false));
                }
            }
            else
            {
                go.transform.SetParent(father);
            }

            return go;
        }
    }


    public void RecycleObject(GameObject obj)
    {
        if (poolObj == null)
        {
            poolObj = new GameObject("Pool");
            DontDestroyOnLoad(poolObj);
        }

        //回收，暂时隐藏可见性
        if (pool.ContainsKey(obj.name))
        {
            pool[obj.name].RecycleObject(obj);
        }
        else
        {
            pool.Add(obj.name, new SubPool(obj, poolObj));
            pool[obj.name].RecycleObject(obj);
        }
    }

    //用于切场景时的清空
    public void Clear()
    {
        pool.Clear();
        Destroy(poolObj);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InteractiveSystem
{
    public class ItemManager : Singleton<ItemManager>
    {
        public Dictionary<int, Item> itemDic = new Dictionary<int, Item>();

        public void Add(int _id, Item _item)
        {
            if (!itemDic.ContainsKey(_id))
            {
                itemDic.Add(_id, _item);
            }
            else
                return;
        }

        public bool CheckItem(int _id)
        {
            return itemDic.ContainsKey(_id);
        }

        public Item GetItem(int _id)
        {
            if (itemDic.ContainsKey(_id))
                return itemDic[_id];
            else
                Debug.LogError("对应id的物品没有找到 " + _id);
            return null;
        }
    }
}

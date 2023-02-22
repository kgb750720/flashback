using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InteractiveSystem
{
    public class Item : MonoBehaviour
    {
        public ItemData data;

        private void Awake()
        {
            if (data == null)
            {
                Debug.LogError("����û�������� " + transform.name);
                return;
            }
            ItemManager.GetInstance().Add(data.itemID,this);
        }
    }
}

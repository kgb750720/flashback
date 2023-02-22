using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InteractiveSystem
{
    [RequireComponent(typeof(Item))]
    public class Prop : DialogueObject ,ISaveable
    {
        //[CustomLabel("道具的动画时间")]
        public float AnimationTime = 1F;
        //[CustomLabel("动画Shader所在的SpriteRenderer")]
        public SpriteRenderer doorSpriteRenderer;

        private Item item;
        private bool isTrigger = false;
        private bool isOpenning = false;
        private float start = 0;


        protected override void Awake()
        {
            base.Awake();
            item = GetComponent<Item>();
        }

        private void Start()
        {
            SaveableRegister();
        }

        protected override void Trigger(Collider2D _collision)
        {
            base.Trigger(_collision);

            if (_collision.tag == "Player")
            {
                isTrigger = true;
                Game.Instance.EventCenter.EventTrigger(Consts.T_D_Start);
            }
        }

        protected override void OnTriggerExit2D(Collider2D _collision)
        {
            base.OnTriggerExit2D(_collision);

            if (_collision.tag == "Player")
            {
                isTrigger = false;
                Game.Instance.EventCenter.EventTrigger(Consts.T_D_Over);
            }
        }
        
        protected override void Apply()
        {
            if (!isTrigger) return; 

            Debug.LogError("正确拾取了东西 : "  + item.data.itemName);

            PropPickUp ppu = new PropPickUp();
            ppu.itemData = item.data;

            switch (item.data.itemType)
            {
                case ItemType.Unlock:
                    Game.Instance.EventCenter.EventTrigger(Consts.P_UNLOCK, ppu);       //分发解锁能力事件
                    PickUpUnLock();
                    break;
                case ItemType.Normal:
                    //Game.Instance.EventCenter.EventTrigger(Consts.P_NORMAL, ppu);       //分发拾取道具事件
                    PickUpNormal();
                    break;
            }
        }

        public void SetSpriteRendererFill(GameObject obj, float fillAmount, float fillType = 1, float fillFlip = 0)
        {
            if (doorSpriteRenderer == null) return;

            Material mat = doorSpriteRenderer.material;

            fillType = Mathf.Clamp01(fillType);
            fillAmount = Mathf.Clamp01(fillAmount);
            mat.SetFloat("_FillType", fillType);
            mat.SetFloat("_FillAmount", fillAmount);
            mat.SetFloat("_FillFlip", fillFlip);
        }

        protected override void SetContent()
        {
            //if (item.data.itemType == ItemType.Normal)
            //{
            //    Tip.SetText(StaticData.Instance.itemDatas[item.data.itemID].itemName);
            //}
            //else if (item.data.itemType == ItemType.Unlock)
            //{
            //    Tip.SetText(item.data.itemDescribe);
            //}
            Tip.SetText(item.data.itemDescribe);
        }

        private void PickUpUnLock()
        {
            isOpenning = true;
            GetComponent<BoxCollider2D>().enabled = false;
        }

        private void PickUpNormal()
        {
            Debug.LogError("获得了心智碎片" + item.data.itemID);

            if (item.data.itemID != -1)
            {
                SaveSystem.Instance.currentSave.getedGoods |= 1 << item.data.itemID;
            }
            ///////////////////////////////
            //todo:
            UIManager.Instance.ShowPanel<UIItemTip>("UIItemTip",null,EUILayer.Top,null,false,(UIItemTip tipUI) => 
            {
                tipUI.Titlle.text = "获得道具";
                tipUI.Text.text = StaticData.Instance.itemDatas[item.data.itemID].itemName;
                Sprite sprite = Resources.Load<Sprite>("Image/UI/" + StaticData.Instance.itemDatas[item.data.itemID].itemSprite);
                tipUI.IconImage.sprite = Sprite.Instantiate(sprite);
                tipUI.BeginPos = new Vector2(1000, 1000);
                tipUI.targetPos = new Vector2(1000, 800);
            });
            Destroy(gameObject);
        }

        protected override void Update()
        {
            base.Update();

            if (isOpenning)
            {
                start += Time.deltaTime;

                float temp = Mathf.Lerp(0, 1, start / AnimationTime);
                SetSpriteRendererFill(gameObject, temp);

                if (temp >= 1)
                {
                    isOpenning = false;
                }
            }
        }

        public void SaveableRegister()
        {
            if (item.data.itemType == ItemType.Normal)
            {
                Game.Instance.SaveSystem.RegisterSaveable(this);
            }
        }

        public void SaveData() { }

        public void LoadData()
        {
            if (item.data.itemType == ItemType.Normal)
            {
                int index = SaveSystem.Instance.currentSave.getedGoods;

                for (int i = 0; i <= item.data.itemID - 1; ++i)
                {
                    index >>= 1;
                }

                if ((index & 1) == 1)
                    gameObject.SetActive(false);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            SaveSystem.Instance.RemoveSaveable(this);
        }
    }
}

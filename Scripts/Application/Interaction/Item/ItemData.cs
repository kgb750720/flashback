namespace InteractiveSystem
{
    public enum ItemType
    {
        //[CustomLabel("解锁二段跳爬墙等类型")]
        Unlock = 0,
        //[CustomLabel("普通道具,重复不叠加")]
        Normal = 1,
    }

    [System.Serializable]
    public class ItemData
    {
        //[CustomLabel("物品的种类")]
        public ItemType itemType;

        //[CustomLabel("物品的ID")]
        public int itemID = -1;
        //[CustomLabel("物品的图片名")]
        public string itemSprite;
        //[CustomLabel("物品的名字")]
        public string itemName;
        //[CustomLabel("物品的文字描述")]
        public string itemDescribe;
    }

}
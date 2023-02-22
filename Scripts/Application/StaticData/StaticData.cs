using System.Collections;
using System.Collections.Generic;
using InteractiveSystem;
using UnityEngine;

public class StaticData : SingletonMono<StaticData>
{
    //物品数据
    public List<ItemData> itemDatas;

    //场景数据
    public Dictionary<string, string> sceneHash;

    //场景音乐哈希
    public Dictionary<string, string> sceneMusicHash;
    
    //场景进场对话哈希
    public Dictionary<string, string> sceneDialogHash;
    public Dictionary<string, string> dialogSceneHash;
    
    //角色初始动画速度
    public Dictionary<string, float> bertheSpeed;
    public Dictionary<string, float> ittaSpeed;
    
    
    //初始存档数据
    public SaveData initialData = new SaveData()
    {
        playerMaxHp = 5,
        level = "3.Level_001",
        dialog = -1,
        SavePointX = -2.428f,
        SavePointY = 7.9829f,
        SavePointZ = 0.1749697f,
        getedSkill = false,
        getedGoods = 0,
        characString = "berthe",
        DifficultyMode = 0
    };

    protected override void Awake()
    {
        base.Awake();
        itemDatas = new List<ItemData>()
        {
            new ItemData()
            {
                itemType = ItemType.Normal,
                itemID = 0,
                itemName = XmlSystemHelper.GetItemFromXml(Consts.ItemFileName, Consts.item_0, "name"),
                itemSprite = XmlSystemHelper.GetItemFromXml(Consts.ItemFileName, Consts.item_0, "sprite"),
                itemDescribe = XmlSystemHelper.GetItemFromXml(Consts.ItemFileName, Consts.item_0)
            },
            new ItemData()
            {
                itemType = ItemType.Normal,
                itemID = 1,
                itemName = XmlSystemHelper.GetItemFromXml(Consts.ItemFileName, Consts.item_1, "name"),
                itemSprite = XmlSystemHelper.GetItemFromXml(Consts.ItemFileName, Consts.item_1, "sprite"),
                itemDescribe = XmlSystemHelper.GetItemFromXml(Consts.ItemFileName, Consts.item_1)
            },
            new ItemData()
            {
                itemType = ItemType.Normal,
                itemID = 2,
                itemName = XmlSystemHelper.GetItemFromXml(Consts.ItemFileName, Consts.item_2, "name"),
                itemSprite = XmlSystemHelper.GetItemFromXml(Consts.ItemFileName, Consts.item_2, "sprite"),
                itemDescribe = XmlSystemHelper.GetItemFromXml(Consts.ItemFileName, Consts.item_2)
            },
            new ItemData()
            {
                itemType = ItemType.Normal,
                itemID = 3,
                itemName = XmlSystemHelper.GetItemFromXml(Consts.ItemFileName, Consts.item_3, "name"),
                itemSprite = XmlSystemHelper.GetItemFromXml(Consts.ItemFileName, Consts.item_3, "sprite"),
                itemDescribe = XmlSystemHelper.GetItemFromXml(Consts.ItemFileName, Consts.item_3)
            },
            new ItemData()
            {
                itemType = ItemType.Normal,
                itemID = 4,
                itemName = XmlSystemHelper.GetItemFromXml(Consts.ItemFileName, Consts.item_4, "name"),
                itemSprite = XmlSystemHelper.GetItemFromXml(Consts.ItemFileName, Consts.item_4, "sprite"),
                itemDescribe = XmlSystemHelper.GetItemFromXml(Consts.ItemFileName, Consts.item_4)
            }
        };
        
        sceneHash = new Dictionary<string, string>()
        {
            {"3.Level_101", "序：0-0-0"},
            {"3.Level_102", "序：0-0-1"},
            {"3.Level_201", "破：0-1-0"},
            {"3.Level_202", "破：0-1-1"},
            {"3.Level_203", "破：1-0-0"},
            {"3.Level_204", "破：1-0-1"},
            {"3.Level_205", "破：1-1-0"},
            {"3.Level_300", "终：1-1-1"},
            {"3.Level_301", "终：1-1-1"}
        };

        sceneMusicHash = new Dictionary<string, string>()
        {
            {"1.StartMenu", "BG_MainMenu"},
            {"3.Level_001", "Scene_001"},
            {"3.Level_101", "Scene_101"},
            {"3.Level_102", "Scene_102"},
            {"3.Level_201", "Scene_201"},
            {"3.Level_202", "Scene_202"},
            {"3.Level_203", "Scene_203"},
            {"3.Level_204", "Scene_204"},
            {"3.Level_205", "Scene_205"},
            {"3.Level_300", ""},
            {"3.Level_301", "Scene_301"}
        };
        
        sceneDialogHash = new Dictionary<string, string>()
        {
            {"3.Level_001", "dia_0"},
            {"3.Level_101", "dia_2"},
            {"3.Level_201", "dia_3"}
        };
        
        dialogSceneHash = new Dictionary<string, string>()
        {
            {"dia_2", "3.Level_101"},
            {"dia_3", "3.Level_201"}
        };
        
        /*bertheSpeed = new Dictionary<string, float>()
        {
            {"move", 1.1f},
            {},
            {},
        };*/
        
        /*ittaSpeed = new Dictionary<string, float>()
        {
            {"", },
            {},
            {},
        };*/
    }
}

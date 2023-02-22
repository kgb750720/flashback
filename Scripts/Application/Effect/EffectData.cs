using System;

namespace EffectSystem
{
    public enum DisappearType
    { 
        Destroy = 0,
        SetActive = 1,
    }

    [Serializable]
    public class EffectData
    {
        public int maxInScene;      //特效存在与场景的最大数量,可以通过实际测试来获取
        public string effectPath;   //特效的路径(完整)
        public string effectName;   //特效名字
    }
}
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
        public int maxInScene;      //��Ч�����볡�����������,����ͨ��ʵ�ʲ�������ȡ
        public string effectPath;   //��Ч��·��(����)
        public string effectName;   //��Ч����
    }
}
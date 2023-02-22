using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIUltimateSkillPanel : MonoBehaviour
{
    private Image[] images;
    private List<Sprite> sprites = new List<Sprite>();
    
    // Start is called before the first frame update
    void Awake()
    {
        sprites.Add(Game.Instance.AssetsManager.Load<Sprite>(Consts.UISpriteDir + "Number_0"));
        sprites.Add(Game.Instance.AssetsManager.Load<Sprite>(Consts.UISpriteDir + "Number_1"));
        sprites.Add(Game.Instance.AssetsManager.Load<Sprite>(Consts.UISpriteDir + "Number_X"));
        images = GetComponentsInChildren<Image>();
        foreach (var image in images)
        {
            image.sprite = sprites[2];
        }
    }

    public void SetNumber(int skillList, int cacheSkill)
    {
        if (cacheSkill < 3) images[0].sprite = sprites[2];
        else images[0].sprite = sprites[(skillList >> 2) & 1];
        if (cacheSkill < 2) images[1].sprite = sprites[2];
        else images[1].sprite = sprites[(skillList >> 1) & 1];
        if (cacheSkill < 1) images[2].sprite = sprites[2];
        else images[2].sprite = sprites[(skillList) & 1];
    }
}

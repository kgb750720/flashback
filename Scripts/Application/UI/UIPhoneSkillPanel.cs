using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPhoneSkillPanel : MonoBehaviour
{
    private Image skill;
    private Image skillMask;
    private Image ultimateSkill;
    private Image ultimateSkillMask;
    private Image image0;
    private Image image;
    private Button skillButton;
    private UIUltimateSkillPanel uiUltimateSkillPanel;

    private Dictionary<string, Sprite> skillSprites;

    private Coroutine coroutine;
    
    // Start is called before the first frame update
    void Awake()
    {
        skill = this.transform.Find("UISkill").GetComponent<Image>();
        skillMask = this.transform.Find("UISkillMask").GetComponent<Image>();
        ultimateSkill = this.transform.Find("UIUltimateSkill").GetComponent<Image>();
        ultimateSkillMask = this.transform.Find("UIUltimateSkillMask").GetComponent<Image>();
        uiUltimateSkillPanel = this.transform.Find("UIUltimateSkillPanel").GetComponent<UIUltimateSkillPanel>();
        skillSprites = new Dictionary<string, Sprite>();
        image0 = this.transform.Find("Image0").GetComponent<Image>();
        image = this.transform.Find("Image").GetComponent<Image>();
        
        skillSprites.Add("Skill0", Game.Instance.AssetsManager.Load<Sprite>(Consts.UISpriteDir + "Skill0"));
        skillSprites.Add("Skill1", Game.Instance.AssetsManager.Load<Sprite>(Consts.UISpriteDir + "Skill1"));
        skillSprites.Add("Null", Game.Instance.AssetsManager.Load<Sprite>(Consts.UISpriteDir + "BuffBG"));

        skillSprites.Add("BuffActive_0", Game.Instance.AssetsManager.Load<Sprite>(Consts.UISpriteDir + "BuffActive_0"));
        skillSprites.Add("BuffActive_1", Game.Instance.AssetsManager.Load<Sprite>(Consts.UISpriteDir + "BuffActive_1"));
        skillSprites.Add("BuffActive_2", Game.Instance.AssetsManager.Load<Sprite>(Consts.UISpriteDir + "BuffActive_2"));
        skillSprites.Add("BuffActive_3", Game.Instance.AssetsManager.Load<Sprite>(Consts.UISpriteDir + "BuffActive_3"));
        image.gameObject.SetActive(false);
        image0.gameObject.SetActive(false);
    }

    public void SetNumber(int skillList, int cacheSkill)
    {
        if(coroutine != null)
            StopCoroutine(coroutine);
        coroutine = StartCoroutine(Spark());
        uiUltimateSkillPanel.SetNumber(skillList, cacheSkill);
        if (cacheSkill < 3)
        {
            ultimateSkill.sprite = skillSprites["Null"];
        }
        else
        {
            //Todo：根据技能列表读不同的技能图标
            int skillIndex = 0;

            if ((skillList & 1) == 1) skillIndex++;
            if ((skillList & 2) == 2) skillIndex++;
            if ((skillList & 4) == 4) skillIndex++;
            
            switch (skillIndex)
            {
                case 0:
                    ultimateSkill.sprite = skillSprites["BuffActive_0"];
                    break;
                case 1:
                    ultimateSkill.sprite = skillSprites["BuffActive_1"];
                    break;
                case 2:
                    ultimateSkill.sprite = skillSprites["BuffActive_2"];
                    break;
                case 3:
                    ultimateSkill.sprite = skillSprites["BuffActive_3"];
                    break;
            }
        }
    }

    private IEnumerator Spark()
    {
        image.gameObject.SetActive(true);
        image0.gameObject.SetActive(true);
        
        yield return new WaitForSecondsRealtime(0.5f);
        
        image.gameObject.SetActive(false);
        image0.gameObject.SetActive(false);
    }

    public void SetSkill(string skillName)
    {
        skill.sprite = skillSprites[skillName];
    }

    public void SetSkillCd(float skillCD)
    {
        skill.fillAmount = 1-skillCD;
    }

    public void SetUltimateSkill(string ultimateSkillName)
    {
        ultimateSkill.sprite = skillSprites[ultimateSkillName];
    }

    public void SetUltimateSkillCd(float UltimateSkillCd)
    {
        ultimateSkillMask.fillAmount = UltimateSkillCd;
    }
}

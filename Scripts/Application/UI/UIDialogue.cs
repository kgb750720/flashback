using TMPro;
using UnityEngine.UI;

public class UIDialogue : UIDynamic
{
    private Image dialogueImage;
    private TMP_Text dialogueText;

    protected override void Awake()
    {
        base.Awake();
        dialogueImage = transform.Find("dialogueImage").GetComponent<Image>();
        dialogueText = transform.Find("dialogueImage/dialogueText").GetComponent<TMP_Text>();
    }

    public void SetText(string _content)
    {
        dialogueText.text = _content;
    }
}

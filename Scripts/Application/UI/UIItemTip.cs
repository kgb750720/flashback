using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIItemTip : BasePanel
{
    public Image IconImage;
    public Image BGImge;
    public TMP_Text Titlle;
    public TMP_Text Text;


    public Vector2 BeginPos;
    public Vector2 targetPos;

    public float moveSpeed = 30f;
    public float stayTime = 5f;
    //[Range(0.01f,1)]
    public float fadeOutSpeed = 0.5f;


    private float m_timeCount = -1;
    private string m_panelName;
    private Transform m_moveTrans;
    private Color m_originIconColor;
    private Color m_originBGColor;
    private Color m_originTitleColor;
    private Color m_originTextColor;


    public override void ShowPanel(UIArgs args)
    {
        ShowPanel(args as UIItemTipArgs);
    }

    public virtual void ShowPanel<T>(T args) where T:UIItemTipArgs
    {
        if (args != null && args.Title != null)
            this.Titlle.text = args.Title;
        if (args != null && args.Text != null)
            this.Text.text = args.Text;
        if (args != null && args.Icon)
            this.IconImage.sprite = args.Icon;
        if (args != null && args.Background)
            this.BGImge.sprite = args.Background;
        if (args != null && args.BeginPos != null)
            this.BeginPos = args.BeginPos;
        if (args != null && args.TargetPos != null)
            this.targetPos = args.TargetPos;
        if (args != null && args.moveSpeed >= 0)
            this.moveSpeed = args.moveSpeed;
        if (args != null && args.stayTime >= 0)
            this.stayTime = args.stayTime;
        if (args != null && args.fadeOutSpeed >= 0)
            this.fadeOutSpeed = args.fadeOutSpeed;

        StopAllCoroutines();
        base.HidePanel();
        m_moveTrans.position = BeginPos;
        IconImage.color = m_originIconColor;
        BGImge.color = m_originBGColor;
        Text.color = m_originTextColor;
        Titlle.color = m_originTitleColor;
        base.ShowPanel(args);
        StartCoroutine(_moveCoroutineHelper());
    }

    private void Awake()
    {
        m_moveTrans = transform.Find("UIItemTipPanel");
        string[] strs = name.Split('/');
        m_panelName = strs[strs.Length - 1];
        int subIdx = m_panelName.IndexOf('(');
        m_panelName = m_panelName.Substring(0, subIdx);
        m_originIconColor = IconImage.color;
        m_originBGColor = BGImge.color;
        m_originTextColor = Text.color;
        m_originTitleColor = Titlle.color;
    }

    private void Start()
    {
    }

    private void Update()
    {
        if (m_timeCount >= 0)
            m_timeCount += Time.deltaTime;

        if(m_timeCount>=stayTime)
        {
            StartCoroutine(_fadeOutCoroutineHelper());
            m_timeCount = -1;
        }

    }

    WaitForFixedUpdate m_wffu = new WaitForFixedUpdate();

    IEnumerator _moveCoroutineHelper()
    {
        while (Vector2.Distance(m_moveTrans.position,targetPos)>0.1f)
        {
            m_moveTrans.position = Vector3.Lerp(m_moveTrans.position, targetPos, moveSpeed * Time.fixedDeltaTime);
            yield return m_wffu;
        }
        m_timeCount = 0;
    }

    WaitForEndOfFrame m_wtfeof = new WaitForEndOfFrame();

    IEnumerator _fadeOutCoroutineHelper()
    {

        bool alphaNotZore;
        do
        {
            yield return m_wtfeof;
            alphaNotZore = false;
            if (IconImage.color.a>0.1f)
            {
                alphaNotZore = true;
                IconImage.color = new Color(IconImage.color.r,
                    IconImage.color.g,
                    IconImage.color.b, 
                    Mathf.Lerp(IconImage.color.a, 0, fadeOutSpeed * Time.deltaTime));
            }

            if (BGImge.color.a > 0.1f)
            {
                alphaNotZore = true;
                BGImge.color = new Color(BGImge.color.r,
                    BGImge.color.g,
                    BGImge.color.b,
                    Mathf.Lerp(BGImge.color.a, 0, fadeOutSpeed * Time.deltaTime));
            }

            if (Titlle.color.a > 0.1f)
            {
                alphaNotZore = true;
                Titlle.color = new Color(Titlle.color.r,
                    Titlle.color.g,
                    Titlle.color.b,
                    Mathf.Lerp(Titlle.color.a, 0, fadeOutSpeed * Time.deltaTime));
            }

            if (Text.color.a > 0.1f)
            {
                alphaNotZore = true;
                Text.color = new Color(Text.color.r,
                    Text.color.g,
                    Text.color.b,
                    Mathf.Lerp(Text.color.a, 0, fadeOutSpeed * Time.deltaTime));
            }

        } while (alphaNotZore);
        Game.Instance.UIManager.HidePanel(m_panelName);
    }
}

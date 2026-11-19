using System;
using UnityEngine;
using UnityEngine.UI;
public class QuestPanel : BasePanel
{
    private RawImage rawImageOne;
    private RawImage rawImageTwo;
    private RawImage rawImageThree;
    private Button QuestBtnOne;
    private Button QuestBtnTwo;
    private Button QuestBtnThree;
    private Button QuestBtnFour;
    private Button QuestBtnFive;
    private Button QuestBtnSix;
    private Text QuestNameOne;
    private Text QuestNameTwo;
    private Text QuestNameThree;
    private Text QuestNameFour;
    private Text QuestNameFive;
    private Text QuestNameSix;

    private GameObject QuestRedPointOne;
    private GameObject QuestRedPointTwo;
    private GameObject QuestRedPointThree;
    private GameObject QuestRedPointFour;
    private GameObject QuestRedPointFive;
    private GameObject QuestRedPointSix;

    private string questRedPointPathOne;
    private string questRedPointPathTwo;
    private string questRedPointPathThree;
    private string questRedPointPathFour;
    private string questRedPointPathFive;
    private string questRedPointPathSix;

    public override bool HideWhenPaused
    {
        get
        {
            return false;
        }
    }

    private void Awake()
    {
        rawImageOne = transform.Find("QuestBg/Text/One").GetComponent<RawImage>();
        rawImageTwo = transform.Find("QuestBg/Text/Two").GetComponent<RawImage>();
        rawImageThree = transform.Find("QuestBg/Text/Three").GetComponent<RawImage>();
        QuestNameOne = transform.Find("QuestList/QuestBgOne/Name").GetComponent<Text>();
        QuestNameTwo = transform.Find("QuestList/QuestBgTwo/Name").GetComponent<Text>();
        QuestNameThree = transform.Find("QuestList/QuestBgThree/Name").GetComponent<Text>();
        QuestNameFour = transform.Find("QuestList/QuestBgFour/Name").GetComponent<Text>();
        QuestNameFive = transform.Find("QuestList/QuestBgFive/Name").GetComponent<Text>();
        QuestNameSix = transform.Find("QuestList/QuestBgSix/Name").GetComponent<Text>();

        QuestRedPointOne = transform.Find("QuestList/QuestBgOne/RedPointIcon").gameObject;
        QuestRedPointTwo = transform.Find("QuestList/QuestBgTwo/RedPointIcon").gameObject;
        QuestRedPointThree = transform.Find("QuestList/QuestBgThree/RedPointIcon").gameObject;
        QuestRedPointFour = transform.Find("QuestList/QuestBgFour/RedPointIcon").gameObject;
        QuestRedPointFive = transform.Find("QuestList/QuestBgFive/RedPointIcon").gameObject;
        QuestRedPointSix = transform.Find("QuestList/QuestBgSix/RedPointIcon").gameObject;

        QuestBtnOne = transform.Find("QuestList/QuestBgOne").GetComponent<Button>();
        QuestBtnOne.onClick.AddListener(OnClickQuestBtnOne);

        QuestBtnTwo = transform.Find("QuestList/QuestBgTwo").GetComponent<Button>();
        QuestBtnTwo.onClick.AddListener(OnClickQuestBtnTwo);

        QuestBtnThree = transform.Find("QuestList/QuestBgThree").GetComponent<Button>();
        QuestBtnThree.onClick.AddListener(OnClickQuestBtnThree);

        QuestBtnFour = transform.Find("QuestList/QuestBgFour").GetComponent<Button>();
        QuestBtnFour.onClick.AddListener(OnClickQuestBtnFour);

        QuestBtnFive = transform.Find("QuestList/QuestBgFive").GetComponent<Button>();
        QuestBtnFive.onClick.AddListener(OnClickQuestBtnFive);

        QuestBtnSix = transform.Find("QuestList/QuestBgSix").GetComponent<Button>();
        QuestBtnSix.onClick.AddListener(OnClickQuestBtnSix);

        QuestRedPointOne.SetActive(false);
        QuestRedPointTwo.SetActive(false);
        QuestRedPointThree.SetActive(false);
        QuestRedPointFour.SetActive(false);
        QuestRedPointFive.SetActive(false);
        QuestRedPointSix.SetActive(false);

    }

    public override void OnEnter()
    {
        base.OnEnter();

        RefreshQuestList();
    }
    public override void OnResume()
    {
        base.OnResume();
        RefreshQuestList();
    }

    public override void OnExit()
    {
        base.OnExit();
        UnbindQuestRedPoint(ref questRedPointPathOne, RefreshQuestRedPointOne, QuestRedPointOne);
        UnbindQuestRedPoint(ref questRedPointPathTwo, RefreshQuestRedPointTwo, QuestRedPointTwo);
        UnbindQuestRedPoint(ref questRedPointPathThree, RefreshQuestRedPointThree, QuestRedPointThree);
        UnbindQuestRedPoint(ref questRedPointPathFour, RefreshQuestRedPointFour, QuestRedPointFour);
        UnbindQuestRedPoint(ref questRedPointPathFive, RefreshQuestRedPointFive, QuestRedPointFive);
        UnbindQuestRedPoint(ref questRedPointPathSix, RefreshQuestRedPointSix, QuestRedPointSix);
    }

    void Update()
    {
        RawImageUVUpdate(rawImageOne, 0.05f);
        RawImageUVUpdate(rawImageTwo, 0.05f);
        RawImageUVUpdate(rawImageThree, 0.05f);
    }
    public void OnClickQuestBtnSix()
    {
        QuestPanelController.Instance.OnClickQuestBtnSix();
    }

    public void OnClickQuestBtnFive()
    {
        QuestPanelController.Instance.OnClickQuestBtnFive();
    }

    public void OnClickQuestBtnFour()
    {
        QuestPanelController.Instance.OnClickQuestBtnFour();
    }

    public void OnClickQuestBtnThree()
    {
        QuestPanelController.Instance.OnClickQuestBtnThree();
    }

    public void OnClickQuestBtnTwo()
    {
        QuestPanelController.Instance.OnClickQuestBtnTwo();
    }

    public void OnClickQuestBtnOne()
    {
        QuestPanelController.Instance.OnClickQuestBtnOne();
    }

    private void RawImageUVUpdate(RawImage rawImage, float speed)
    {
        Rect rect = rawImage.uvRect;

        rect.x += speed * Time.deltaTime;

        if (rect.x >= 1f)
        {
            rect.x -= 1f;
        }

        rawImage.uvRect = rect;
    }
    private void RefreshQuestList()
    {
        RefreshQuestButton(QuestBtnOne, QuestNameOne, QuestRedPointOne, QuestType.Move.ToString(), ref questRedPointPathOne, RefreshQuestRedPointOne);
        RefreshQuestButton(QuestBtnTwo, QuestNameTwo, QuestRedPointTwo, QuestType.CharactorDevelopment.ToString(), ref questRedPointPathTwo, RefreshQuestRedPointTwo);
        RefreshQuestButton(QuestBtnThree, QuestNameThree, QuestRedPointThree, QuestType.Daily.ToString(), ref questRedPointPathThree, RefreshQuestRedPointThree);
        RefreshQuestButton(QuestBtnFour, QuestNameFour, QuestRedPointFour, QuestType.Fight.ToString(), ref questRedPointPathFour, RefreshQuestRedPointFour);
        RefreshQuestButton(QuestBtnFive, QuestNameFive, QuestRedPointFive, QuestType.Spend.ToString(), ref questRedPointPathFive, RefreshQuestRedPointFive);
        RefreshQuestButton(QuestBtnSix, QuestNameSix, QuestRedPointSix, QuestType.WeaponDevelopment.ToString(), ref questRedPointPathSix, RefreshQuestRedPointSix);
    }
    private void RefreshQuestButton(Button btn, Text questName, GameObject redPointIcon, string questType, ref string redPointPath, Action<bool> callback)
    {
        UnbindQuestRedPoint(ref redPointPath, callback, redPointIcon);

        QuestInfo quest = QuestModel.Instance.GetCurrentQuestByType(questType);
        if (quest == null)
        {
            btn.gameObject.SetActive(false);
            return;
        }
        btn.gameObject.SetActive(true);
        questName.text = quest.Name;

        redPointPath = RedPointPath.GetQuestPath(quest.Id);
        RedPointManager.Instance.AddListener(redPointPath, callback);

        return;
    }

    private void UnbindQuestRedPoint(ref string redPointPath, Action<bool> callback, GameObject redPointIcon)
    {
        if (!string.IsNullOrEmpty(redPointPath))
        {
            RedPointManager.Instance.RemoveListener(redPointPath, callback);
        }
        redPointPath = null;
        if (redPointIcon != null)
        {
            redPointIcon.SetActive(false);
        }
    }

    private void RefreshQuestRedPointOne(bool show)
    {
        QuestRedPointOne.SetActive(show);
    }

    private void RefreshQuestRedPointTwo(bool show)
    {
        QuestRedPointTwo.SetActive(show);
    }

    private void RefreshQuestRedPointThree(bool show)
    {
        QuestRedPointThree.SetActive(show);
    }

    private void RefreshQuestRedPointFour(bool show)
    {
        QuestRedPointFour.SetActive(show);
    }

    private void RefreshQuestRedPointFive(bool show)
    {
        QuestRedPointFive.SetActive(show);
    }

    private void RefreshQuestRedPointSix(bool show)
    {
        QuestRedPointSix.SetActive(show);
    }

}

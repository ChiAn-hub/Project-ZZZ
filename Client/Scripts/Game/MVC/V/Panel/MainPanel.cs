using System;
using UnityEngine;
using UnityEngine.UI;

public class MainPanel : BasePanel
{
    private Text playerNmaeText;
    private Text playerExpText;
    private Text playerLevelText;
    private Text playerSpText;
    private Text playerGoldText;
    private Text playerMoneyText;

    private Button AchieveBtn;
    private Button CharactorBtn;
    private Button BagBtn;

    private Button EmplyBtn;
    private Button MoreBtn;
    private Button QuestBtn;
    private Button ShopBtn;
    private Button SystemBtn;
    private Button NotityBtn;
    private Button EmailBtn;

    private GameObject bagRedPointIcon;
    private GameObject questRedPointIcon;

    private void Awake()
    {
        playerNmaeText = transform.Find("Top/Player/PlayerName").GetComponent<Text>();
        playerExpText = transform.Find("Top/Player/PlayerExp").GetComponent<Text>();
        playerLevelText = transform.Find("Top/Player/Level").GetComponent<Text>();
        playerSpText = transform.Find("Top/Player_Sp/Num").GetComponent<Text>();
        playerGoldText = transform.Find("Top/Player_Gold/Num").GetComponent<Text>();
        playerMoneyText = transform.Find("Top/Player_Money/Num").GetComponent<Text>();
        bagRedPointIcon = transform.Find("Down/AllIcon/BagBtn/RedPointIcon").gameObject;
        questRedPointIcon = transform.Find("Down/AllIcon/QuestBtn/RedPointIcon").gameObject;

        AchieveBtn = transform.Find("Down/AllIcon/AchieveBtn").GetComponent<Button>();
        AchieveBtn.onClick.AddListener(OnClickAchieveBtn);

        CharactorBtn = transform.Find("Down/AllIcon/CharactorBtn").GetComponent<Button>();
        CharactorBtn.onClick.AddListener(OnClickCharactorBtn);

        BagBtn = transform.Find("Down/AllIcon/BagBtn").GetComponent<Button>();
        BagBtn.onClick.AddListener(OnClickBagBtn);

        QuestBtn = transform.Find("Down/AllIcon/QuestBtn").GetComponent<Button>();
        QuestBtn.onClick.AddListener(OnClickQuestBtn);

        EmplyBtn = transform.Find("Down/AllIcon/EmplyBtn").GetComponent<Button>();
        EmplyBtn.onClick.AddListener(OnClickEmplyBtn);

        MoreBtn = transform.Find("Down/AllIcon/MoreBtn").GetComponent<Button>();
        MoreBtn.onClick.AddListener(OnClickMoreBtn);

        ShopBtn = transform.Find("Down/AllIcon/ShopBtn").GetComponent<Button>();
        ShopBtn.onClick.AddListener(OnClickShopBtn);

        SystemBtn = transform.Find("Down/AllIcon/SystemBtn").GetComponent<Button>();
        SystemBtn.onClick.AddListener(OnClickSystemBtn);

        NotityBtn = transform.Find("Down/AllIcon/NotityBtn").GetComponent<Button>();
        NotityBtn.onClick.AddListener(OnClickNotityBtn);


        EmailBtn = transform.Find("Down/AllIcon/EmailBtn").GetComponent<Button>();
        EmailBtn.onClick.AddListener(OnClickEmailBtn);

        bagRedPointIcon.SetActive(false);
        questRedPointIcon.SetActive(false);
    }

    public override void OnEnter()
    {
        base.OnEnter();
        PlayerModel.Instance.GoldChanged -= RefreshGold;//防止重复添加
        PlayerModel.Instance.GoldChanged += RefreshGold;
        RefreshGold(PlayerModel.Instance.PlayerGold);//更新金币显示

        PlayerModel.Instance.ExpChanged -= RefreshExp;
        PlayerModel.Instance.ExpChanged += RefreshExp;
        RefreshExp(PlayerModel.Instance.PlayerExp);//更新经验显示

        PlayerModel.Instance.LevelChanged -= RefreshLevel;
        PlayerModel.Instance.LevelChanged += RefreshLevel;
        RefreshLevel(PlayerModel.Instance.PlayerLevel);//更新等级显示

        PlayerModel.Instance.NameChanged -= RefreshName;
        PlayerModel.Instance.NameChanged += RefreshName;
        RefreshName(PlayerModel.Instance.PlayerName);//更新名字显示

        PlayerModel.Instance.SpChanged -= RefreshSp;
        PlayerModel.Instance.SpChanged += RefreshSp;
        RefreshSp(PlayerModel.Instance.PlayerSp);//更新体力显示

        PlayerModel.Instance.MoneyChanged -= RefreshMoney;
        PlayerModel.Instance.MoneyChanged += RefreshMoney;
        RefreshMoney(PlayerModel.Instance.PlayerMoney);//更新钱钱显示

        RedPointManager.Instance.RemoveListener(RedPointPath.Bag, RefreshBagRedPoint);
        RedPointManager.Instance.AddListener(RedPointPath.Bag, RefreshBagRedPoint);

        RedPointManager.Instance.RemoveListener(RedPointPath.Quest, RefreshQuestRedPoint);
        RedPointManager.Instance.AddListener(RedPointPath.Quest, RefreshQuestRedPoint);

        PlayerInfoController.Instance.RequestPlayerInfo();
        BagPanelController.Instance.RequestBag();

        OpenQuestPanelController.Instance.RequestQuestList((success) =>//进入该页面时请求一次任务列表
        {
            if (!success)
            {
                Debug.LogError("进入主界面时获取任务列表失败");
            }
        });
    }

    public override void OnResume()
    {
        base.OnResume();
        PlayerInfoController.Instance.RequestPlayerInfo();
    }

    public override void OnExit()
    {
        base.OnExit();
        PlayerModel.Instance.GoldChanged -= RefreshGold;
        PlayerModel.Instance.ExpChanged -= RefreshExp;
        PlayerModel.Instance.LevelChanged -= RefreshLevel;
        PlayerModel.Instance.NameChanged -= RefreshName;
        PlayerModel.Instance.SpChanged -= RefreshSp;
        PlayerModel.Instance.MoneyChanged -= RefreshMoney;
        RedPointManager.Instance.RemoveListener(RedPointPath.Bag, RefreshBagRedPoint);
        RedPointManager.Instance.RemoveListener(RedPointPath.Quest, RefreshQuestRedPoint);
    }

    private void RefreshBagRedPoint(bool show)
    {
        if (bagRedPointIcon == null)
        {
            return;
        }
        bagRedPointIcon.SetActive(show);
    }

    private void RefreshQuestRedPoint(bool show)
    {
        if (questRedPointIcon == null)
        {
            return;
        }
        questRedPointIcon.SetActive(show);
    }

    private void RefreshGold(long gold)
    {
        playerGoldText.text = gold.ToString();
    }

    private void RefreshSp(long sp)
    {
        playerSpText.text = sp.ToString();
    }

    private void RefreshLevel(int level)
    {
        playerLevelText.text = level.ToString();
    }

    private void RefreshName(string name)
    {
        playerNmaeText.text = name;
    }

    private void RefreshExp(long exp)
    {
        playerExpText.text = exp.ToString();
    }

    private void RefreshMoney(long money)
    {
        playerMoneyText.text = money.ToString();
    }

    public void OnClickEmailBtn()
    {
        MainPanelController.Instance.OnClickEmailBtn();
    }

    public void OnClickNotityBtn()
    {
        MainPanelController.Instance.OnClickNotityBtn();
    }

    public void OnClickSystemBtn()
    {
        MainPanelController.Instance.OnClickSystemBtn();
    }

    public void OnClickShopBtn()
    {
        MainPanelController.Instance.OnClickShopBtn();
    }

    public void OnClickMoreBtn()
    {
        MainPanelController.Instance.OnClickMoreBtn();
    }

    public void OnClickEmplyBtn()
    {
        MainPanelController.Instance.OnClickEmplyBtn();
    }

    public void OnClickQuestBtn()
    {
        MainPanelController.Instance.OnClickQuestBtn();
    }

    public void OnClickBagBtn()
    {
        MainPanelController.Instance.OnClickBagBtn();
    }

    public void OnClickCharactorBtn()
    {
        MainPanelController.Instance.OnClickCharactorBtn();
    }

    public void OnClickAchieveBtn()
    {
        MainPanelController.Instance.OnClickAchieveBtn();
    }
}

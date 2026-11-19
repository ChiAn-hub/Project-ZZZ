using UnityEngine;
using UnityEngine.UI;

public class CityPanel : BasePanel
{
    private Button SwitchBtn;
    private Button CharactorBtn;
    private Button NoteBtn;
    private Button EmplyBtn;
    private Button MoreBtn;
    private Button ShopBtn;
    private Button TVBtn;
    private Image PlayerHpImage;
    private Text PlayerHpText;

    private GameObject deadBg;
    private Button reviveBtn;

    private bool isReviving;//防止重复点击重生

    private void Awake()
    {

        SwitchBtn = transform.Find("Top/SwitchBg/SwitchBtn").GetComponent<Button>();
        SwitchBtn.onClick.AddListener(OnClickSwitchBtn);


        CharactorBtn = transform.Find("AllBtn/CharactorBtn").GetComponent<Button>();
        CharactorBtn.onClick.AddListener(OnClickCharactorBtn);

        NoteBtn = transform.Find("AllBtn/NoteBtn").GetComponent<Button>();
        NoteBtn.onClick.AddListener(OnClickNoteBtn);


        EmplyBtn = transform.Find("AllBtn/EmplyBtn").GetComponent<Button>();
        EmplyBtn.onClick.AddListener(OnClickEmplyBtn);


        MoreBtn = transform.Find("AllBtn/MoreBtn").GetComponent<Button>();
        MoreBtn.onClick.AddListener(OnClickMoreBtn);


        ShopBtn = transform.Find("AllBtn/ShopBtn").GetComponent<Button>();
        ShopBtn.onClick.AddListener(OnClickShopBtn);

        TVBtn = transform.Find("AllBtn/TVBtn").GetComponent<Button>();
        TVBtn.onClick.AddListener(OnClickTVBtn);

        reviveBtn = transform.Find("DeadBg/Btn").GetComponent<Button>();
        reviveBtn.onClick.AddListener(OnClickReviveBtn);

        deadBg = transform.Find("DeadBg").gameObject;
        PlayerHpImage = transform.Find("Hp/HpBg/PlayerHp").GetComponent<Image>();
        PlayerHpText = transform.Find("Hp/HpBg/PlayerHpText").GetComponent<Text>();

        deadBg.SetActive(false);

    }

    public override void OnEnter()
    {
        base.OnEnter();

        isReviving = false;
        reviveBtn.interactable = true;

        EllenDataModel.Instance.HpChanged -= RefreshPlayerHp;
        EllenDataModel.Instance.HpChanged += RefreshPlayerHp;

        //不能只依赖事件，打开面板时必须主动刷新一次
        RefreshPlayerHp(EllenDataModel.Instance.CurHp, EllenDataModel.Instance.MaxHp);

        OpenQuestPanelController.Instance.RequestQuestList((success) =>
        {
            if (!success)
            {
                Debug.LogError("获取任务列表失败");
            }
        });

        AudioClip bgMusic = Resources.Load<AudioClip>("Audio/BG");

        if (MusicManage.Instance != null)
        {
            MusicManage.Instance.MusicChange(bgMusic);
        }
    }
    public override void OnPause()
    {
        base.OnPause();
        CityPanelController.Instance.SetInputControl(false);
    }
    public override void OnResume()
    {
        base.OnResume();
        RefreshPlayerHp(EllenDataModel.Instance.CurHp, EllenDataModel.Instance.MaxHp);
        CityPanelController.Instance.SetInputControl(true);
    }
    public override void OnExit()
    {
        base.OnExit();
        EllenDataModel.Instance.HpChanged -= RefreshPlayerHp;
        CityPanelController.Instance.SetInputControl(false);
    }
    public void OnClickTVBtn()
    {
        CityPanelController.Instance.OnClickTVBtn();
    }

    public void OnClickShopBtn()
    {
        CityPanelController.Instance.OnClickShopBtn();
    }

    public void OnClickMoreBtn()
    {
        CityPanelController.Instance.OnClickMoreBtn();
    }

    public void OnClickEmplyBtn()
    {
        CityPanelController.Instance.OnClickEmplyBtn();
    }

    public void OnClickNoteBtn()
    {
        CityPanelController.Instance.OnClickNoteBtn();
    }

    public void OnClickCharactorBtn()
    {
        CityPanelController.Instance.OnClickCharactorBtn();
    }

    public void OnClickSwitchBtn()
    {
        CityPanelController.Instance.OnClickSwitchBtn();
    }

    private void OnClickReviveBtn()
    {
        if (isReviving)
        {
            return;
        }

        isReviving = true;
        reviveBtn.interactable = false;

        CityPanelController.Instance.Revive((success) =>
        {
            if (!success)
            {
                isReviving = false;
                reviveBtn.interactable = true;

                Debug.LogError("角色重生失败");
                return;
            }

            PlayerInput.SetCanControl(true);

            //使用现有加载界面重新进入MainScene
            UIManager.Instance.PushPanel(UIPanelType.LoadingPanel);
        });
    }

    private void RefreshPlayerHp(int curHp, int maxHp)//刷新角色血条
    {
        if (PlayerHpImage == null || PlayerHpText == null || deadBg == null)
        {
            return;
        }

        if (maxHp <= 0)
        {
            PlayerHpImage.fillAmount = 0f;
            PlayerHpText.text = "0/0";

            deadBg.SetActive(false);

            return;
        }

        PlayerHpImage.fillAmount = (float)curHp / maxHp;
        PlayerHpText.text = curHp + "/" + maxHp;

        if (curHp <= 0)
        {
            deadBg.SetActive(true);
            deadBg.transform.SetAsLastSibling();

            PlayerInput.SetCanControl(false);
        }
        else
        {
            deadBg.SetActive(false);
        }
    }
}

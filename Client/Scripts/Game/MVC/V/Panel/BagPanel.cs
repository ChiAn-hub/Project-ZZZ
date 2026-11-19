using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;

public class BagPanel : BasePanel
{
    private const int DefaultSlotCount = 80;
    private GameObject SlotPrefab;
    private GameObject emptyDetail;
    private InputField addInputField;
    private bool isExpandRequesting;
    private Transform BagBg;
    private Vector3 StartPos;
    private Button addBtn;
    private Button WeaponBtn;
    private Button BangBooBtn;
    private Button ExpBtn;
    private Button OtherBtn;
    private Button RemoveBtn;
    private Transform slotParent;
    private DetailItemPanel detailItemPanel;
    private Text playerSpText;
    private Text playerGoldText;
    private Text playerMoneyText;
    private Dropdown sort;
    private List<BagSlot> slots = new List<BagSlot>();//用来存储格子的数量
    private void Awake()
    {
        BagBg = transform.Find("BagBG");
        StartPos = BagBg.position;
        sort = transform.Find("Sort").GetComponent<Dropdown>();
        SlotPrefab = Resources.Load<GameObject>("Prefabs/UIPrefabs/Slot");
        slotParent = transform.Find("AllSlot/Scroll View/Viewport/Content");
        emptyDetail = transform.Find("DetailPanel/PanelBg/Ematy").gameObject;
        detailItemPanel = transform.Find("DetailPanel/DetailItemPanel").GetComponent<DetailItemPanel>();
        playerSpText = transform.Find("Player/Player_Sp/Num").GetComponent<Text>();
        playerGoldText = transform.Find("Player/Player_Gold/Num").GetComponent<Text>();
        playerMoneyText = transform.Find("Player/Player_Money/Num").GetComponent<Text>();

        WeaponBtn = transform.Find("Switch/SwitchBg/WeaponBtn").GetComponent<Button>();
        BangBooBtn = transform.Find("Switch/SwitchBg/BangBooBtn").GetComponent<Button>();
        ExpBtn = transform.Find("Switch/SwitchBg/ExpBtn").GetComponent<Button>();
        OtherBtn = transform.Find("Switch/SwitchBg/OtherBtn").GetComponent<Button>();
        RemoveBtn = transform.Find("RemoveBtn").GetComponent<Button>();
        addBtn = transform.Find("AddSlotBg/AddBtn").GetComponent<Button>();
        addInputField = transform.Find("AddSlotBg/InputField").GetComponent<InputField>();

        addBtn.onClick.AddListener(OnClickAddBtn);
        WeaponBtn.onClick.AddListener(OnClickWeaponBtn);
        BangBooBtn.onClick.AddListener(OnClickBangBooBtn);
        ExpBtn.onClick.AddListener(OnClickExpBtn);
        OtherBtn.onClick.AddListener(OnClickOtherBtn);
        RemoveBtn.onClick.AddListener(OnClickRemoveBtn);
        sort.onValueChanged.AddListener(OnValueChanged);

        addInputField.gameObject.SetActive(false);

        EnsureSlotCount(DefaultSlotCount);//创建默认的格子
        ShowEmptyDetail();
    }

    void Update()
    {
        BGAnimation();
    }

    public override void OnEnter()
    {
        base.OnEnter();

        addInputField.text = "";
        addInputField.gameObject.SetActive(false);

        BagModel.Instance.BagChanged -= RefreshBag;
        BagModel.Instance.BagChanged += RefreshBag;

        BagPanelController.Instance.ItemSelected -= ShowItemDetail;
        BagPanelController.Instance.ItemSelected += ShowItemDetail;

        PlayerModel.Instance.SpChanged -= RefreshSp;
        PlayerModel.Instance.SpChanged += RefreshSp;

        PlayerModel.Instance.GoldChanged -= RefreshGold;
        PlayerModel.Instance.GoldChanged += RefreshGold;

        PlayerModel.Instance.MoneyChanged -= RefreshMoney;
        PlayerModel.Instance.MoneyChanged += RefreshMoney;

        RefreshSp(PlayerModel.Instance.PlayerSp);
        RefreshGold(PlayerModel.Instance.PlayerGold);
        RefreshMoney(PlayerModel.Instance.PlayerMoney);

        BagPanelController.Instance.ResetFilter();

        ShowEmptyDetail();
        RefreshBag();

        BagPanelController.Instance.RequestBag();
        PlayerInfoController.Instance.RequestPlayerInfo();
    }
    public override void OnExit()
    {
        base.OnExit();
        BagModel.Instance.BagChanged -= RefreshBag;
        BagPanelController.Instance.ItemSelected -= ShowItemDetail;
        PlayerModel.Instance.SpChanged -= RefreshSp;
        PlayerModel.Instance.GoldChanged -= RefreshGold;
        PlayerModel.Instance.MoneyChanged -= RefreshMoney;

        BagPanelController.Instance.ExitRecycleMode();
    }
    private void ShowItemDetail(ItemInfo itemInfo, int count)
    {
        emptyDetail.SetActive(false);
        detailItemPanel.ShowItem(itemInfo, count);
    }

    private void ShowEmptyDetail()//显示详细面板
    {
        detailItemPanel.HideItem();
        emptyDetail.SetActive(true);
    }

    public void BGAnimation()//背景动画
    {
        BagBg.Translate(-BagBg.right * Time.deltaTime * 100);
        if (BagBg.position.x < -1916.0f)
        {
            BagBg.position = StartPos;
        }
    }
    private void RefreshBag()
    {
        EnsureSlotCount(BagModel.Instance.Capacity);
        List<BagItemData> items = BagPanelController.Instance.GetDisplayItems();//获取到对应的道具
        for (int i = 0; i < slots.Count; i++)
        {
            if (i < items.Count)//在创建的所有格子中,只对对应数量的道具进行更改
            {
                BagItemData itemData = items[i];
                ItemInfo itemInfo = ItemModel.Instance.GetItemInfo(itemData.ItemId);//获取该道具的信息
                if (itemInfo != null)
                {
                    slots[i].SetData(itemInfo, itemData.Count, itemData.EquippedCharacterId);//更新UI
                }
                else
                {
                    slots[i].Clear();
                    Debug.LogError("没有找到物品配置" + itemData.ItemId);
                }
            }
            else
            {
                slots[i].Clear();
            }
        }
    }

    private void EnsureSlotCount(int targetCount)
    {
        if (targetCount <= slots.Count)
        {
            return;
        }
        while (slots.Count < targetCount)
        {
            GameObject go = GameObject.Instantiate(SlotPrefab, slotParent);
            BagSlot slot = go.GetComponent<BagSlot>();

            slots.Add(slot);
        }
    }

    private void RefreshSp(long sp)
    {
        playerSpText.text = sp.ToString();
    }

    private void RefreshGold(long gold)
    {
        playerGoldText.text = gold.ToString();
    }

    private void RefreshMoney(long money)
    {
        playerMoneyText.text = money.ToString();
    }
    public void OnClickWeaponBtn()
    {
        BagPanelController.Instance.OnClickWeaponBtn();
        RefreshBag();
        ShowEmptyDetail();
    }
    public void OnClickBangBooBtn()
    {
        BagPanelController.Instance.OnClickBangBooBtn();
        RefreshBag();
        ShowEmptyDetail();
    }
    public void OnClickExpBtn()
    {
        BagPanelController.Instance.OnClickExpBtn();
        RefreshBag();
        ShowEmptyDetail();
    }
    public void OnClickOtherBtn()
    {
        BagPanelController.Instance.OnClickOtherBtn();
        RefreshBag();
        ShowEmptyDetail();
    }
    public void OnClickRemoveBtn()
    {
        BagPanelController.Instance.OnClickRemoveBtn();

        RefreshBag();
        ShowEmptyDetail();
    }
    private void OnClickAddBtn()
    {
        if (isExpandRequesting)
        {
            return;
        }

        if (!addInputField.gameObject.activeSelf)
        {
            addInputField.text = "";
            addInputField.gameObject.SetActive(true);
            addInputField.ActivateInputField();
            return;
        }

        int addCount;
        if (!int.TryParse(addInputField.text, out addCount))
        {
            return;
        }
        isExpandRequesting = true;
        BagPanelController.Instance.RequestExpandBag(addCount, (success) =>
        {
            isExpandRequesting = false;
            if (!success)
            {
                Debug.LogError("背包扩容失败");
                return;
            }
            addInputField.text = "";
            addInputField.gameObject.SetActive(false);
        });
    }

    private void OnValueChanged(int value)
    {
        BagPanelController.Instance.ChangeSortType(value);
        RefreshBag();
        ShowEmptyDetail();
    }

}

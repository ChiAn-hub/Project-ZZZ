using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
public class PlayerPanel : BasePanel
{
    private GameObject characterPage;
    private GameObject equipmentPage;
    private GameObject allSlotPanel;
    private GameObject player;//角色显示图片

    private Button baseBtn;
    private Button equipmentBtn;

    private Text characterNameText;
    private Text levelText;
    private Text hpText;
    private Text attackText;
    private Text impactText;
    private Text defenseText;
    private Text criticalText;
    private Text criticalDamageText;
    private GameObject slotPrefab;
    private Transform weaponSlotParent;

    private RawImage rawImageOne;
    private RawImage rawImageTwo;
    private RawImage rawImageThree;

    private Image equipmentIcon;
    private Button equipmentIconBtn;
    private Button unequipBtn;
    private Button equipmentListCloseBtn;

    private Sprite emptyEquipmentSprite;
    private Color emptyEquipmentColor;

    private bool isEquipmentRequesting;

    private const int ExpItemOneId = 2001;
    private const int ExpItemTwoId = 2002;
    private const int ExpItemThreeId = 2003;

    private const int ExpItemOneValue = 100;
    private const int ExpItemTwoValue = 600;
    private const int ExpItemThreeValue = 3000;

    private const int MaxLevel = 60;

    private GameObject levelUpPanel;

    private Button levelBtn;
    private Button levelUpCloseBtn;
    private Button confirmLevelUpBtn;

    private Button expItemOneBtn;
    private Button expItemTwoBtn;
    private Button expItemThreeBtn;

    private Text expItemOneCountText;
    private Text expItemTwoCountText;
    private Text expItemThreeCountText;

    private Text curLevelText;
    private Text needExpText;
    private Text upLevelText;

    private Image levelUpBgImage;
    private Image levelUpFillImage;

    private Color levelUpBgDefaultColor;

    private long selectedTotalExp;
    private bool isLevelUpRequesting;

    private readonly Dictionary<int, int> selectedExpItemCounts = new Dictionary<int, int>();

    private readonly List<GameObject> weaponSlotObjects = new List<GameObject>();
    private void Awake()
    {


        characterPage = transform.Find("CharactorBG").gameObject;
        equipmentPage = transform.Find("Equip").gameObject;
        allSlotPanel = transform.Find("AllSlotPanel").gameObject;
        player = transform.Find("Player").gameObject;
        levelUpPanel = transform.Find("LevelUpPanel").gameObject;

        levelUpPanel.SetActive(false);

        levelBtn = transform.Find("CharactorBG/Level/LevelBtn").GetComponent<Button>();
        levelUpCloseBtn = transform.Find("LevelUpPanel/CloseBtn").GetComponent<Button>();
        confirmLevelUpBtn = transform.Find("LevelUpPanel/LevelUp").GetComponent<Button>();
        expItemOneBtn = transform.Find("LevelUpPanel/ExpBg/Exp_1").GetComponent<Button>();
        expItemTwoBtn = transform.Find("LevelUpPanel/ExpBg/Exp_2").GetComponent<Button>();
        expItemThreeBtn = transform.Find("LevelUpPanel/ExpBg/Exp_3").GetComponent<Button>();
        expItemOneCountText = transform.Find("LevelUpPanel/ExpBg/Exp_1/ItemBg/Num").GetComponent<Text>();
        expItemTwoCountText = transform.Find("LevelUpPanel/ExpBg/Exp_2/ItemBg/Num").GetComponent<Text>();
        expItemThreeCountText = transform.Find("LevelUpPanel/ExpBg/Exp_3/ItemBg/Num").GetComponent<Text>();
        Transform levelUpBgTransform = transform.Find("LevelUpPanel/LevelUpBg");
        levelUpBgImage = levelUpBgTransform.GetComponent<Image>();
        curLevelText = levelUpBgTransform.Find("CurLevel").GetComponent<Text>();
        needExpText = levelUpBgTransform.Find("NeedExp").GetComponent<Text>();
        upLevelText = levelUpBgTransform.Find("UpLevel").GetComponent<Text>();
        levelUpFillImage = levelUpBgTransform.Find("LevelUp").GetComponent<Image>();

        baseBtn = transform.Find("SwitchBtn/SwitchBtn_1").GetComponent<Button>();
        equipmentBtn = transform.Find("SwitchBtn/SwitchBtn_3").GetComponent<Button>();
        characterNameText = transform.Find("CharactorBG/BgTwo/CharactorName").GetComponent<Text>();
        levelText = transform.Find("CharactorBG/Level/Level").GetComponent<Text>();
        hpText = transform.Find("CharactorBG/All/Hp/Num").GetComponent<Text>();
        attackText = transform.Find("CharactorBG/All/Attack/Num").GetComponent<Text>();
        impactText = transform.Find("CharactorBG/All/Impact/Num").GetComponent<Text>();
        defenseText = transform.Find("CharactorBG/All/Def/Num").GetComponent<Text>();
        criticalText = transform.Find("CharactorBG/All/critical/Num").GetComponent<Text>();
        criticalDamageText = transform.Find("CharactorBG/All/criticalDamage/Num").GetComponent<Text>();
        rawImageOne = transform.Find("BG/Text/One").GetComponent<RawImage>();
        rawImageTwo = transform.Find("BG/Text/Two").GetComponent<RawImage>();
        rawImageThree = transform.Find("BG/Text/Three").GetComponent<RawImage>();
        slotPrefab = Resources.Load<GameObject>("Prefabs/UIPrefabs/Slot");
        weaponSlotParent = transform.Find("AllSlotPanel/Scroll View/Viewport/Content");
        Transform equipmentIconTransform = transform.Find("Equip/EquipBg/Equip");
        equipmentIcon = equipmentIconTransform.GetComponent<Image>();
        equipmentIconBtn = equipmentIconTransform.GetComponent<Button>();
        unequipBtn = transform.Find("AllSlotPanel/UnequipBtn").GetComponent<Button>();
        equipmentListCloseBtn = transform.Find("AllSlotPanel/CloseBtn").GetComponent<Button>();

        emptyEquipmentSprite = equipmentIcon.sprite;
        emptyEquipmentColor = equipmentIcon.color;
        levelUpBgDefaultColor = levelUpBgImage.color;
        equipmentIconBtn.onClick.AddListener(OpenEquipmentList);
        unequipBtn.onClick.AddListener(OnClickUnequip);
        equipmentListCloseBtn.onClick.AddListener(CloseEquipmentList);
        baseBtn.onClick.AddListener(ShowCharacterPage);
        equipmentBtn.onClick.AddListener(ShowEquipmentPage);
        levelBtn.onClick.AddListener(OpenLevelUpPanel);
        levelUpCloseBtn.onClick.AddListener(CloseLevelUpPanel);
        confirmLevelUpBtn.onClick.AddListener(OnClickConfirmLevelUp);
        expItemOneBtn.onClick.AddListener(() => OnClickExpItem(ExpItemOneId, ExpItemOneValue));
        expItemTwoBtn.onClick.AddListener(() => OnClickExpItem(ExpItemTwoId, ExpItemTwoValue));
        expItemThreeBtn.onClick.AddListener(() => OnClickExpItem(ExpItemThreeId, ExpItemThreeValue));
    }
    public override void OnEnter()
    {
        base.OnEnter();
        EllenDataModel.Instance.DataChanged -= RefreshCharacter;
        EllenDataModel.Instance.DataChanged += RefreshCharacter;
        BagModel.Instance.BagChanged -= RefreshWeaponSlots;
        BagModel.Instance.BagChanged += RefreshWeaponSlots;
        BagModel.Instance.BagChanged -= RefreshLevelUpItemCounts;
        BagModel.Instance.BagChanged += RefreshLevelUpItemCounts;

        ShowCharacterPage();
        RefreshCharacter();

        PlayerPanelController.Instance.RequestEllenInfo();
        PlayerController.Instance.StateMachine.ChangeState(StateEnum.EnterStart);
    }
    public override void OnExit()
    {
        base.OnExit();
        EllenDataModel.Instance.DataChanged -= RefreshCharacter;
        BagModel.Instance.BagChanged -= RefreshWeaponSlots;
        BagModel.Instance.BagChanged -= RefreshLevelUpItemCounts;
        allSlotPanel.SetActive(false);
        levelUpPanel.SetActive(false);
        ResetLevelUpSelection();
    }
    void Update()
    {
        RawImageUVUpdate(rawImageOne, 0.05f);
        RawImageUVUpdate(rawImageTwo, 0.05f);
        RawImageUVUpdate(rawImageThree, 0.05f);
    }
    public void RawImageUVUpdate(RawImage rawImage, float speed)
    {
        Rect rect = rawImage.uvRect;

        rect.x += speed * Time.deltaTime;

        if (rect.x >= 1f)
        {
            rect.x -= 1f;
        }

        rawImage.uvRect = rect;
    }
    private void ShowCharacterPage()
    {
        characterPage.SetActive(true);
        equipmentPage.SetActive(false);
        allSlotPanel.SetActive(false);
        player.SetActive(true);
    }

    private void ShowEquipmentPage()
    {
        characterPage.SetActive(false);
        equipmentPage.SetActive(true);
        allSlotPanel.SetActive(false);
        player.SetActive(false);
    }

    private void RefreshCharacter()
    {
        EllenDataModel model = EllenDataModel.Instance;
        characterNameText.text = "艾莲";
        levelText.text = model.Level.ToString();
        hpText.text = model.CurHp + "/" + model.MaxHp;
        attackText.text = model.Attack.ToString();
        impactText.text = model.Impact.ToString();
        defenseText.text = model.Defense.ToString();
        criticalText.text = (model.Critical * 100f).ToString("0") + "%";
        criticalDamageText.text = (model.CriticalDamage * 100f).ToString("0") + "%";
        RefreshEquipmentIcon();
    }
    private void RefreshEquipmentIcon()
    {
        int itemId = EllenDataModel.Instance.EquippedItemId;
        unequipBtn.interactable = itemId != 0;
        if (itemId == 0)
        {
            equipmentIcon.sprite = emptyEquipmentSprite;
            equipmentIcon.color = emptyEquipmentColor;
            return;
        }
        ItemInfo itemInfo = ItemModel.Instance.GetItemInfo(itemId);
        if (itemInfo == null)
        {
            equipmentIcon.sprite = emptyEquipmentSprite;
            equipmentIcon.color = emptyEquipmentColor;
            return;
        }
        Sprite sprite = Resources.Load<Sprite>(itemInfo.path);
        if (sprite == null)
        {
            Debug.LogError("找不到装备图标：" + itemInfo.path);
            return;
        }
        equipmentIcon.sprite = sprite;
        equipmentIcon.color = Color.white;
    }
    private void OpenEquipmentList()
    {
        allSlotPanel.SetActive(true);

        RefreshWeaponSlots();

        BagPanelController.Instance.RequestBag();// 保证装备列表使用服务器上的最新背包数据
    }
    private void CloseEquipmentList()
    {
        allSlotPanel.SetActive(false);
    }
    private void RefreshWeaponSlots()
    {
        if (!allSlotPanel.activeSelf)
        {
            return;
        }

        foreach (GameObject slotObject in weaponSlotObjects)
        {
            if (slotObject != null)
            {
                slotObject.SetActive(false);
                Destroy(slotObject);
            }
        }

        weaponSlotObjects.Clear();

        List<BagItemData> bagItems = BagModel.Instance.GetItems;

        foreach (BagItemData itemData in bagItems)
        {
            ItemInfo itemInfo = ItemModel.Instance.GetItemInfo(itemData.ItemId);

            if (itemInfo == null || itemInfo.itemType != itemType.Weapon)
            {
                continue;
            }

            GameObject slotObject = Instantiate(slotPrefab, weaponSlotParent);

            BagSlot slot = slotObject.GetComponent<BagSlot>();

            slot.SetData(itemInfo, itemData.Count, itemData.EquippedCharacterId);

            slot.SetClickHandler(OnClickWeaponSlot);

            weaponSlotObjects.Add(slotObject);
        }
    }
    private void OnClickWeaponSlot(ItemInfo itemInfo, int count)
    {
        if (isEquipmentRequesting || itemInfo == null || count <= 0)
        {
            return;
        }

        if (itemInfo.itemId == EllenDataModel.Instance.EquippedItemId)// 已经是艾莲当前装备，不重复请求
        {
            return;
        }

        isEquipmentRequesting = true;

        PlayerPanelController.Instance.EquipEllen(itemInfo.itemId, (success) =>
        {
            isEquipmentRequesting = false;

            if (success)
            {
                unequipBtn.gameObject.SetActive(true);
            }
        });
    }
    private void OnClickUnequip()
    {
        if (isEquipmentRequesting || EllenDataModel.Instance.EquippedItemId == 0)
        {
            return;
        }

        isEquipmentRequesting = true;
        unequipBtn.gameObject.SetActive(false);

        PlayerPanelController.Instance.UnequipEllen((success) =>
            {
                isEquipmentRequesting = false;

                if (!success)
                {
                    unequipBtn.gameObject.SetActive(true);
                }
            });
    }

    private void OpenLevelUpPanel()
    {
        levelUpPanel.SetActive(true);
        levelUpPanel.transform.SetAsLastSibling();

        ResetLevelUpSelection();
        RefreshLevelUpItemCounts();

        //重新获取服务器背包数量
        BagPanelController.Instance.RequestBag();
    }

    private void CloseLevelUpPanel()
    {
        levelUpPanel.SetActive(false);
        ResetLevelUpSelection();
    }

    private void OnClickExpItem(int itemId, int expValue)
    {
        int ownedCount = BagModel.Instance.GetItemCount(itemId);

        int selectedCount = GetSelectedItemCount(itemId);

        if (selectedCount >= ownedCount)
        {
            return;
        }

        selectedExpItemCounts[itemId] = selectedCount + 1;

        selectedTotalExp += expValue;

        RefreshLevelUpItemCounts();
        RefreshLevelUpPreview();
    }

    private int GetSelectedItemCount(int itemId)
    {
        int count;

        if (selectedExpItemCounts.TryGetValue(
            itemId,
            out count))
        {
            return count;
        }

        return 0;
    }

    private void RefreshLevelUpItemCounts()
    {
        int oneCount = BagModel.Instance.GetItemCount(ExpItemOneId);
        int twoCount = BagModel.Instance.GetItemCount(ExpItemTwoId);
        int threeCount = BagModel.Instance.GetItemCount(ExpItemThreeId);
        expItemOneCountText.text = Mathf.Max(0, oneCount - GetSelectedItemCount(ExpItemOneId)).ToString();
        expItemTwoCountText.text = Mathf.Max(0, twoCount - GetSelectedItemCount(ExpItemTwoId)).ToString();
        expItemThreeCountText.text = Mathf.Max(0, threeCount - GetSelectedItemCount(ExpItemThreeId)).ToString();
    }

    private int GetNeedExp(int level)//获取升级所需经验
    {
        return 1000 + (level - 1) * 500;
    }

    private void RefreshLevelUpPreview()//刷新升级预览
    {
        EllenDataModel model = EllenDataModel.Instance;//获取Ellen数据

        int currentLevel = model.Level;//当前等级
        int previewLevel = currentLevel;//预览等级

        long previewExp = model.Exp + selectedTotalExp;//预览经验

        while (previewLevel < MaxLevel)
        {
            int needExp = GetNeedExp(previewLevel);//获取升级所需经验

            if (previewExp < needExp)//预览经验小于升级所需经验
            {
                break;
            }

            previewExp -= needExp;//预览经验减去升级所需经验
            previewLevel++;//预览等级加1
        }

        curLevelText.text = currentLevel.ToString();//当前等级
        upLevelText.text = previewLevel.ToString();//预览等级

        int currentNeedExp = GetNeedExp(currentLevel);//当前等级所需经验

        needExpText.text = (model.Exp + selectedTotalExp) + "/" + currentNeedExp;//当前等级所需经验加预览经验

        if (previewLevel > currentLevel)
        {
            levelUpBgImage.color = new Color32(31, 77, 53, 255);//升级背景颜色
        }
        else
        {
            levelUpBgImage.color = levelUpBgDefaultColor;//默认颜色
        }

        if (previewLevel >= MaxLevel)
        {
            levelUpFillImage.fillAmount = 1f;//最高等级
        }
        else
        {
            int previewNeedExp = GetNeedExp(previewLevel);//预览等级需要的经验

            levelUpFillImage.fillAmount = (float)previewExp / previewNeedExp;//填充
        }
        confirmLevelUpBtn.interactable = selectedTotalExp > 0 && !isLevelUpRequesting;//按钮是否可用
    }

    private void OnClickConfirmLevelUp()
    {
        if (isLevelUpRequesting || selectedTotalExp <= 0)
        {
            return;
        }

        List<ExpItemUseData> useItems = new List<ExpItemUseData>();

        foreach (KeyValuePair<int, int> item in selectedExpItemCounts)
        {
            if (item.Value <= 0)
            {
                continue;
            }

            useItems.Add(new ExpItemUseData
            {
                ItemId = item.Key,
                Count = item.Value
            });
        }

        isLevelUpRequesting = true;
        confirmLevelUpBtn.interactable = false;

        PlayerPanelController.Instance.LevelUpEllen(useItems, (success) =>
            {
                isLevelUpRequesting = false;

                if (!success)
                {
                    confirmLevelUpBtn.interactable = true;
                    return;
                }

                ResetLevelUpSelection();
                RefreshCharacter();
                RefreshLevelUpItemCounts();
            });
    }

    private void ResetLevelUpSelection()//重置升级选择
    {
        selectedExpItemCounts.Clear();
        selectedTotalExp = 0;

        levelUpBgImage.color = levelUpBgDefaultColor;

        RefreshLevelUpItemCounts();
        RefreshLevelUpPreview();
    }
}

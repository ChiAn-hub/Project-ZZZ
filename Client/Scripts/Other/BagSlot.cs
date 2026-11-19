using System;
using UnityEngine;
using UnityEngine.UI;

public class BagSlot : MonoBehaviour
{
    private int curCount;
    private Image icon;
    private Image characterIcon;//角色图标
    private Text numText;
    private Color emptyColor;
    private Sprite emptySprite;
    private Button slotBtn;
    private ItemInfo curItemInfo;
    private GameObject selectIcon;//选择图片
    private GameObject redPointIcon;//红点图片
    private string redPointPath;//当前格子监听的红点路径
    private Action<ItemInfo, int> clickHandler;
    void Awake()
    {
        icon = transform.Find("SlotBG/Icon").GetComponent<Image>();
        characterIcon = transform.Find("CharacterIcon").GetComponent<Image>();
        numText = transform.Find("SlotBG/TextBg/Num").GetComponent<Text>();
        slotBtn = transform.Find("SlotBG").GetComponent<Button>();
        selectIcon = transform.Find("SelectIcon").gameObject;
        redPointIcon = transform.Find("Icon").gameObject;
        characterIcon.gameObject.SetActive(false);
        redPointIcon.SetActive(false);
        selectIcon.SetActive(false);
        emptyColor = icon.color;
        emptySprite = icon.sprite;

        slotBtn.onClick.AddListener(OnClickSlot);
    }

    private void OnClickSlot()
    {
        if (curItemInfo == null || curCount <= 0)
        {
            return;
        }

        if (!string.IsNullOrEmpty(redPointPath))
        {
            RedPointManager.Instance.SetRedPoint(redPointPath, false);
        }

        if (clickHandler != null)
        {
            clickHandler(curItemInfo, curCount);
            return;
        }

        bool selected = BagPanelController.Instance.OnClickSlot(curItemInfo, curCount);
        SetSelected(selected);
    }

    private void RefreshRedPoint(bool show)
    {
        if (redPointIcon == null)
        {
            return;
        }

        redPointIcon.SetActive(show);
    }

    private void BindRedPoint(int itemId)//根据物品Id绑定红点
    {
        redPointPath = RedPointPath.GetBagItemPath(itemId);//获取物品红点路径
        RedPointManager.Instance.AddListener(redPointPath, RefreshRedPoint);
    }

    private void UnbindRedPoint()
    {
        if (!string.IsNullOrEmpty(redPointPath))//如果红点路径不为空的话
        {
            RedPointManager.Instance.RemoveListener(redPointPath, RefreshRedPoint);
        }

        redPointPath = null;

        if (redPointIcon != null)
        {
            redPointIcon.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        UnbindRedPoint();

        if (slotBtn != null)
        {
            slotBtn.onClick.RemoveListener(OnClickSlot);
        }
    }

    public void SetData(ItemInfo itemInfo, int num, int equippedCharacterId)
    {
        UnbindRedPoint();//取消绑定红点

        curItemInfo = itemInfo;
        curCount = num;

        Sprite sprite = Resources.Load<Sprite>(itemInfo.path);
        icon.color = Color.white;
        icon.sprite = sprite;
        numText.text = num.ToString();

        SetSelected(BagPanelController.Instance.IsSelected(itemInfo.itemId));
        SetCharacterIcon(equippedCharacterId);

        BindRedPoint(itemInfo.itemId);//绑定红点
    }
    public void SetCharacterIcon(int characterId)
    {
        string path = null;
        switch (characterId)
        {
            case 1: // 艾莲
                path = "ItemIcons/Character/IconRoleCircle21";
                break;

            case 2: // 安比
                path = "ItemIcons/Character/IconRoleCircle01";
                break;

            case 3: // 可林
                path = "ItemIcons/Character/IconRoleCircle09";
                break;
        }
        if (string.IsNullOrEmpty(path))
        {
            characterIcon.sprite = null;
            characterIcon.gameObject.SetActive(false);
            return;
        }
        Sprite sprite = Resources.Load<Sprite>(path);

        if (sprite == null)
        {
            Debug.LogError("找不到角色头像：" + path);
            characterIcon.gameObject.SetActive(false);
            return;
        }

        characterIcon.sprite = sprite;
        characterIcon.gameObject.SetActive(true);
    }
    public void Clear()
    {
        UnbindRedPoint();

        curCount = 0;
        curItemInfo = null;

        characterIcon.sprite = null;
        characterIcon.gameObject.SetActive(false);

        icon.sprite = emptySprite;
        icon.color = emptyColor;
        numText.text = "EMPTY";

        SetSelected(false);
    }
    public void SetSelected(bool selected)
    {
        selectIcon.SetActive(selected && curItemInfo != null && curCount > 0);
    }

    public void SetClickHandler(Action<ItemInfo, int> handler)
    {
        clickHandler = handler;
        SetSelected(false);
    }
}

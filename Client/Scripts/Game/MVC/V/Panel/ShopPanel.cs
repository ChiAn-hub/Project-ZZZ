using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
public class ShopPanel : BasePanel
{
    private Text playerSpText;
    private Text playerGoldText;
    private Text playerMoneyText;
    private GameObject shopItemPrefab;
    private Transform shopItemParent;

    private List<ShopItemSlot> shopItemSlots = new List<ShopItemSlot>();

    public override bool HideWhenPaused
    {
        get
        {
            return false;
        }
    }

    private void Awake()
    {
        playerSpText = transform.Find("Player/Player_Sp/Num").GetComponent<Text>();
        playerGoldText = transform.Find("Player/Player_Gold/Num").GetComponent<Text>();
        playerMoneyText = transform.Find("Player/Player_Money/Num").GetComponent<Text>();
        shopItemPrefab = Resources.Load<GameObject>("Prefabs/UIPrefabs/ShopItem");
        shopItemParent = transform.Find("ShopItem/Scroll View/Viewport/Content");
    }

    public override void OnEnter()
    {
        base.OnEnter();

        PlayerModel.Instance.SpChanged -= RefreshSp;
        PlayerModel.Instance.SpChanged += RefreshSp;

        PlayerModel.Instance.GoldChanged -= RefreshGold;
        PlayerModel.Instance.GoldChanged += RefreshGold;

        PlayerModel.Instance.MoneyChanged -= RefreshMoney;
        PlayerModel.Instance.MoneyChanged += RefreshMoney;
        ShopModel.Instance.ShopChanged -= RefreshShop;
        ShopModel.Instance.ShopChanged += RefreshShop;

        RefreshShop();

        ShopPanelController.Instance.RequestShop();

        RefreshSp(PlayerModel.Instance.PlayerSp);
        RefreshGold(PlayerModel.Instance.PlayerGold);
        RefreshMoney(PlayerModel.Instance.PlayerMoney);

        PlayerInfoController.Instance.RequestPlayerInfo();
    }

    public override void OnExit()
    {
        base.OnExit();

        PlayerModel.Instance.SpChanged -= RefreshSp;
        PlayerModel.Instance.GoldChanged -= RefreshGold;
        PlayerModel.Instance.MoneyChanged -= RefreshMoney;
        ShopModel.Instance.ShopChanged -= RefreshShop;
    }
    private void RefreshShop()
    {
        foreach (ShopItemSlot oldSlot in shopItemSlots)
        {
            if (oldSlot != null)
            {
                Destroy(oldSlot.gameObject);
            }
        }
        shopItemSlots.Clear();
        List<ShopProductData> products = ShopModel.Instance.GetProducts;
        foreach (ShopProductData product in products)
        {
            GameObject shopItemdata = Instantiate(shopItemPrefab, shopItemParent);
            ShopItemSlot shopItemSlot = shopItemdata.GetComponent<ShopItemSlot>();
            shopItemSlot.SetData(product);
            shopItemSlots.Add(shopItemSlot);
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
}

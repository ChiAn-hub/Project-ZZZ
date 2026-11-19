using System;
using UnityEngine;
using UnityEngine.UI;

public class OpenShopPanel : BasePanel
{
    private Image itemImage;
    private Text itemNameText;
    private Text contentText;
    private Text buyNumText;//购买数量
    private Text allNumText;//最大数量
    private Slider buySlider;
    private Button enterButton;
    private ShopProductData curProduct;
    private int buyCount;
    private int maxBuyCount;

    private RawImage rawImageOne;
    private RawImage rawImageTwo;
    private RawImage rawImageThree;
    private RawImage rawImageFour;

    private void Awake()
    {
        itemImage = transform.Find("OpenShopBG/Icon").GetComponent<Image>();
        itemNameText = transform.Find("OpenShopBG/Name").GetComponent<Text>();
        contentText = transform.Find("OpenShopBG/Content").GetComponent<Text>();
        buyNumText = transform.Find("OpenShopBG/BuyNum").GetComponent<Text>();
        allNumText = transform.Find("OpenShopBG/AllNum").GetComponent<Text>();
        buySlider = transform.Find("OpenShopBG/Slider").GetComponent<Slider>();
        enterButton = transform.Find("OpenShopBG/EnterNum").GetComponent<Button>();
        rawImageOne = transform.Find("TextBg/Text/one").GetComponent<RawImage>();
        rawImageTwo = transform.Find("TextBg/Text/two").GetComponent<RawImage>();
        rawImageThree = transform.Find("TextBg/Text/three").GetComponent<RawImage>();
        rawImageFour = transform.Find("TextBg/Text/four").GetComponent<RawImage>();
        buySlider.onValueChanged.AddListener(OnSliderChanged);
        enterButton.onClick.AddListener(OnClickEnterButton);
    }
    public override void OnEnter()
    {
        base.OnEnter();
        curProduct = OpenShopPanelController.Instance.CurProduct;
        if (curProduct == null)
        {
            Debug.LogError("没有选择商品");
            enterButton.interactable = false;
            return;
        }
        ShowProduct();
    }
    void Update()
    {
        RawImageUVUpdate(rawImageOne, 0.05f);
        RawImageUVUpdate(rawImageTwo, 0.05f);
        RawImageUVUpdate(rawImageThree, 0.05f);
        RawImageUVUpdate(rawImageFour, 0.05f);
    }
    private void ShowProduct()
    {
        ItemInfo itemInfo = ItemModel.Instance.GetItemInfo(curProduct.ItemId);
        if (itemInfo == null)
        {
            Debug.LogError("没有找到物品配置" + curProduct.ItemId);
            enterButton.interactable = false;
            return;
        }
        Sprite sprite = Resources.Load<Sprite>(itemInfo.path);
        itemImage.sprite = sprite;
        itemImage.color = Color.white;
        itemNameText.text = itemInfo.name;
        contentText.text = itemInfo.content;
        if (curProduct.Price <= 0)
        {
            maxBuyCount = 0;
        }
        else
        {
            long affordableCount = PlayerModel.Instance.PlayerGold / curProduct.Price;
            maxBuyCount = (int)Math.Min(affordableCount, 99);
        }
        buySlider.wholeNumbers = true;
        buySlider.maxValue = Mathf.Max(1, maxBuyCount);
        buySlider.minValue = 1;
        buySlider.value = 1;
        allNumText.text = maxBuyCount.ToString();
        enterButton.interactable = maxBuyCount > 0;
        if (maxBuyCount <= 0)
        {
            buyCount = 0;
            buyNumText.text = "0";
        }
        else
        {
            UpdateBuyInformation(1);
        }
    }
    private void OnSliderChanged(float value)
    {
        if (maxBuyCount <= 0)
        {
            return;
        }
        UpdateBuyInformation((int)value);
    }
    private void UpdateBuyInformation(int count)
    {
        buyCount = count;
        buyNumText.text = buyCount.ToString();
    }
    private void OnClickEnterButton()
    {
        if (curProduct == null || buyCount <= 0)
        {
            return;
        }
        enterButton.interactable = false;
        OpenShopPanelController.Instance.BuyProduct(buyCount, OnBuyResult);
    }
    private void OnBuyResult(BuyResult result)
    {
        if (result == BuyResult.Success)
        {
            Debug.Log("购买成功");
            UIManager.Instance.PopPanel();
            return;
        }
        enterButton.interactable = maxBuyCount > 0;
        switch (result)
        {
            case BuyResult.NotLogin:
                Debug.LogError("玩家未登录");
                break;
            case BuyResult.InvalidCount:
                Debug.LogError("购买数量不合法");
                break;
            case BuyResult.ProductNotFound:
                Debug.LogError("商品不存在或已下架");
                break;
            case BuyResult.NotEnoughGold:
                Debug.LogError("金币不足");
                PlayerInfoController.Instance.RequestPlayerInfo();
                break;
            default:
                Debug.LogError("购买失败");
                break;
        }
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
}

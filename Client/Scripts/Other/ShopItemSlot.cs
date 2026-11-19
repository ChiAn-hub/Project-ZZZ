using UnityEngine;
using UnityEngine.UI;
public class ShopItemSlot : MonoBehaviour
{
    private Image itemImage;
    private Text itemCountText;
    private Text priceText;
    private Button itemButton;

    private ShopProductData curProduct;

    private void Awake()
    {
        itemImage = transform.Find("ItemImage").GetComponent<Image>();
        itemCountText = transform.Find("NumBg/Num").GetComponent<Text>();
        priceText = transform.Find("SpendNum").GetComponent<Text>();
        itemButton = GetComponent<Button>();
        itemButton.onClick.AddListener(OnClickItem);
    }
    public void SetData(ShopProductData product)
    {
        curProduct = product;
        ItemInfo itemInfo = ItemModel.Instance.GetItemInfo(product.ItemId);
        if (itemInfo == null)
        {
            Debug.LogError("没有找到物品配置" + product.ItemId);
            gameObject.SetActive(false);
            return;
        }
        Sprite sprite = Resources.Load<Sprite>(itemInfo.path);
        if (sprite == null)
        {
            Debug.LogError("没有找到物品图片" + itemInfo.path);
            return;
        }
        itemImage.sprite = sprite;
        itemCountText.text = "×" + product.UnitCount;
        priceText.text = product.Price.ToString();
        gameObject.SetActive(true);
    }
    private void OnClickItem()
    {
        if (curProduct == null)
        {
            return;
        }

        ShopPanelController.Instance.OnClickShopItem(curProduct);
    }
}

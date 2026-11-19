using UnityEngine;

public class ShopPanelController : Singleton<ShopPanelController>
{
    public void RequestShop()
    {
        ProtoManager.Instance.CSShop((success, products) =>
        {
            if (!success)
            {
                Debug.LogError("获取商品列表失败");
                return;
            }
            ShopModel.Instance.ReplaceProducts(products);
        });
    }

    public void OnClickShopItem(ShopProductData product)
    {
        if (product == null)
        {
            return;
        }
        OpenShopPanelController.Instance.SetProduct(product);
        UIManager.Instance.PushPanel(UIPanelType.OpenShopPanel);
    }
}

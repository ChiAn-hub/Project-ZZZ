using Unity.VisualScripting;
using UnityEngine;

public class OpenShopPanelController : Singleton<OpenShopPanelController>
{
    public ShopProductData CurProduct { get; private set; }
    public void SetProduct(ShopProductData product)
    {
        CurProduct = product;
    }
    public void BuyProduct(int buyCount, System.Action<BuyResult> callback)
    {
        if (CurProduct == null)
        {
            callback?.Invoke(BuyResult.ProductNotFound);
            return;
        }
        ProtoManager.Instance.CSBuyItem(CurProduct.ProductId, buyCount, (msg) =>
        {
            if (msg == null)
            {
                callback?.Invoke(BuyResult.ServerError);
                return;
            }
            if (msg.Result == BuyResult.Success)
            {
                PlayerModel.Instance.ApplyGold(msg.Gold);
                BagModel.Instance.ReplaceItems(msg.Items);
            }
            callback?.Invoke(msg.Result);
        });
    }
}

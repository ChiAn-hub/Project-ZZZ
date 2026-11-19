using System;
using System.Collections.Generic;
using UnityEngine;

public class ShopModel : Singleton<ShopModel>
{
    private List<ShopProductData> products = new List<ShopProductData>();
    public event Action ShopChanged;
    public List<ShopProductData> GetProducts
    {
        get
        {
            return new List<ShopProductData>(products);
        }
    }
    public void ReplaceProducts(List<ShopProductData> newProduces)
    {
        products.Clear();
        if (newProduces != null)
        {
            products.AddRange(newProduces);
        }
        ShopChanged?.Invoke();
    }
}

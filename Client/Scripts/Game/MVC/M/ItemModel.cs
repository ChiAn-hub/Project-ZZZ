using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class ItemModel
{
    private static ItemModel instance;
    public static ItemModel Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ItemModel();
            }
            return instance;
        }
    }
    private ItemModel()
    {
        ReadInfo();
    }

    private Dictionary<int, ItemInfo> ItemIndoDict = new Dictionary<int, ItemInfo>();
    private void ReadInfo()
    {
        TextAsset textAsset = Resources.Load("Config/ItemConfig") as TextAsset;
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(textAsset.text);
        XmlNodeList nodeList = xmlDoc.SelectNodes("/Objects/Object");
        foreach (XmlNode node in nodeList)
        {
            ItemInfo itemInfo = new ItemInfo();
            itemInfo.itemId = int.Parse(node.Attributes["itemId"].Value);
            itemInfo.itemType = (itemType)System.Enum.Parse(typeof(itemType), node.Attributes["itemType"].Value);
            itemInfo.content = node.Attributes["content"].Value;
            itemInfo.name = node.Attributes["name"].Value;
            itemInfo.path = node.Attributes["path"].Value;
            itemInfo.quality = node.Attributes["quality"].Value[0];
            ItemIndoDict.Add(itemInfo.itemId, itemInfo);
        }
    }
    public ItemInfo GetItemInfo(int itemId)
    {
        ItemInfo itemInfo;

        if (ItemIndoDict.TryGetValue(itemId, out itemInfo))
        {
            return itemInfo;
        }

        Debug.LogError("XML中找不到物品配置，ItemId：" + itemId);
        return null;
    }
}

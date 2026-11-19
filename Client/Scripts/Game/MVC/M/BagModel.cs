using System;
using System.Collections.Generic;
using UnityEngine;

public class BagModel : Singleton<BagModel>
{
    private int capacity = 80;
    private bool hasLoadedItems;
    private List<BagItemData> items = new List<BagItemData>();//背包物品

    public event Action BagChanged;//背包更新时通知
    public event Action<int, int> ItemObtained;//获得物品时通知
    public int Capacity
    {
        get
        {
            return capacity;
        }
    }
    public List<BagItemData> GetItems
    {
        get
        {
            return new List<BagItemData>(items);//防止外部对数据进行篡改,清除
        }
    }
    public void ReplaceItems(List<BagItemData> newItems)//获取新列表
    {
        Dictionary<int, int> oldItemCounts = new Dictionary<int, int>();

        foreach (BagItemData item in items)//存储旧列表中每个道具的数量
        {
            if (item == null)
            {
                continue;
            }
            oldItemCounts[item.ItemId] = item.Count;
        }
        if (hasLoadedItems && newItems != null)//如果已经完成第一次的加载,并且有新列表时
        {
            foreach (BagItemData newItem in newItems)//遍历新列表
            {
                if (newItem == null)
                {
                    continue;
                }
                int oldCount = 0;
                oldItemCounts.TryGetValue(newItem.ItemId, out oldCount);//根据对应的编号找出原来的道具数量
                if (newItem.Count > oldCount)//新数量大于旧数量时,证明有新添加的和再次获取的道具
                {
                    string path = RedPointPath.GetBagItemPath(newItem.ItemId);//根据该编号设置红点路径
                    RedPointManager.Instance.SetRedPoint(path, true);//设置红结点,并显示显示

                    ItemObtained?.Invoke(newItem.ItemId, newItem.Count - oldCount);//通知获得物品
                }
                oldItemCounts.Remove(newItem.ItemId);//从旧列表中移除
            }
        }

        if (hasLoadedItems)//已经完成第一次的加载时
        {
            foreach (int removedItemId in oldItemCounts.Keys)//遍历旧列表中剩余的道具,此时剩余的道具都是被消耗的或者耗尽了的
            {
                string path = RedPointPath.GetBagItemPath(removedItemId);//根据该编号设置红点路径
                RedPointManager.Instance.SetRedPoint(path, false);//设置红结点,但不显示
            }
        }

        items.Clear();//清空列表
        if (newItems != null)
        {
            items.AddRange(newItems);//将新列表添加到列表中
        }
        hasLoadedItems = true;//完成第一次的加载
        BagChanged?.Invoke();//通知背包更新
    }
    public void ReplaceItems(List<BagItemData> newItems, int newCapacity)//获取新列表和新容量
    {
        if (newCapacity >= 0)
        {
            capacity = newCapacity;
        }
        ReplaceItems(newItems);
    }
    public void AddItem(BagItemData item)//添加物品
    {
        if (item == null || item.Count <= 0)
        {
            return;
        }
        items.Add(item);

        string path = RedPointPath.GetBagItemPath(item.ItemId);
        RedPointManager.Instance.SetRedPoint(path, true);

        ItemObtained?.Invoke(item.ItemId, item.Count);
        BagChanged?.Invoke();
    }

    public void SetCapacity(int newCapacity)//设置背包容量
    {
        if (newCapacity <= capacity)
        {
            return;
        }

        capacity = newCapacity;
        BagChanged?.Invoke();
    }

    public int GetItemCount(int itemId)//获取指定道具数量
    {
        foreach (BagItemData item in items)
        {
            if (item.ItemId == itemId)
            {
                return item.Count;
            }
        }

        return 0;
    }

    public void Clear()
    {
        items.Clear();
        capacity = 80;
        hasLoadedItems = false;

        BagChanged?.Invoke();
    }
}

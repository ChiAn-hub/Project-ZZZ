using System;
using System.Collections.Generic;
using UnityEngine;

public enum BagFilterType//通过枚举进行筛选
{
    Weapon,
    BangBoo,
    Exp,
    Other
}

public enum BagSortType//通过枚举进行排序
{
    Quality,//按质量进行排序
    Count,//按数量进行排序
}

public class BagPanelController : Singleton<BagPanelController>
{

    private BagSortType curSortType = BagSortType.Quality;

    private bool isRecycleMode;
    private bool isRecycleRequesting;//处理多次点击
    private readonly HashSet<int> selectedItemIds = new HashSet<int>();
    private BagFilterType curType = BagFilterType.Weapon;
    public event Action<ItemInfo, int> ItemSelected;
    public bool IsRecycleMode => isRecycleMode;
    public bool IsSelected(int itemId)
    {
        return selectedItemIds.Contains(itemId);
    }
    public void OnClickWeaponBtn()
    {
        curType = BagFilterType.Weapon;
    }
    public void OnClickBangBooBtn()
    {
        if (isRecycleMode)
        {
            return;
        }
        curType = BagFilterType.BangBoo;
    }
    public void OnClickExpBtn()
    {
        if (isRecycleMode)
        {
            return;
        }
        curType = BagFilterType.Exp;
    }
    public void OnClickOtherBtn()
    {
        if (isRecycleMode)
        {
            return;
        }
        curType = BagFilterType.Other;
    }
    public void OnClickRemoveBtn()
    {
        if (isRecycleRequesting)//如果处于消息请求中
        {
            return;
        }
        if (!isRecycleMode)//不处于回收模式
        {
            isRecycleMode = true;
            selectedItemIds.Clear();
            curType = BagFilterType.Weapon;
            return;
        }
        if (selectedItemIds.Count == 0)//如果没有选择的道具
        {
            ExitRecycleMode();
            return;
        }
        isRecycleRequesting = true;
        List<int> itemIds = new List<int>(selectedItemIds);
        ProtoManager.Instance.CSRecycleItems(itemIds, OnRecycleResult);
    }
    private void OnRecycleResult(MsgRecycleItems msg)
    {
        isRecycleRequesting = false;

        if (msg == null)
        {
            Debug.LogError("没有收到回收结果");
            return;
        }

        if (msg.Result == RecycleResult.Success)
        {
            ExitRecycleMode();// 先退出，防止刷新格子时再次显示选择图标

            PlayerModel.Instance.ApplyGold(msg.Gold);//更新金币
            BagModel.Instance.ReplaceItems(msg.Items);//更新道具

            Debug.Log("回收成功");
            return;
        }

        switch (msg.Result)
        {
            case RecycleResult.NotLogin:
                Debug.LogError("玩家未登录");
                break;

            case RecycleResult.InvalidSelection:
                Debug.LogError("回收选择不合法");
                break;

            case RecycleResult.ItemNotFound:
                Debug.LogError("物品不存在或数量已经改变");
                break;

            case RecycleResult.ItemNotRecyclable:
                Debug.LogError("包含不能回收的物品");
                break;
            case RecycleResult.ItemEquipped:
                Debug.LogError("已经穿戴的装备不能回收");
                break;
            default:
                Debug.LogError("服务器回收失败");
                break;
        }
    }
    public bool IsMatch(itemType type)//判断类型是否对应
    {
        switch (curType)
        {
            case BagFilterType.Weapon:
                return type == itemType.Weapon;

            case BagFilterType.BangBoo:
                return type == itemType.BangBoo;

            case BagFilterType.Exp:
                return type == itemType.CharactorExp
                    || type == itemType.WeaponExp;

            case BagFilterType.Other:
                return type == itemType.Other;
        }

        return true;
    }
    public List<BagItemData> GetDisplayItems()
    {
        List<BagItemData> allItems = BagModel.Instance.GetItems;//获取背包中全部的道具
        List<BagItemData> result = new List<BagItemData>();//用于存储筛选后的道具
        foreach (BagItemData itemData in allItems)
        {
            ItemInfo itemInfo = ItemModel.Instance.GetItemInfo(itemData.ItemId);//获取每个道具的信息
            if (itemInfo == null)
            {
                continue;
            }
            if (IsMatch(itemInfo.itemType))//判断道具类型是否匹配
            {
                result.Add(itemData);
            }
        }
        if (result.Count > 1)
        {
            QuickSort(result, 0, result.Count - 1);
        }
        return result;
    }
    public void RequestBag()//请求背包数据
    {
        ProtoManager.Instance.CSBag((success, items, capacity) =>
        {
            if (!success)
            {
                Debug.LogError("获取背包数据失败");
                return;
            }

            BagModel.Instance.ReplaceItems(items, capacity);
        });
    }
    public bool OnClickSlot(ItemInfo itemInfo, int count)//当点击格子时
    {
        if (itemInfo == null || count <= 0)
        {
            return false;
        }

        if (!isRecycleMode)//不是回收模式
        {
            ItemSelected?.Invoke(itemInfo, count);//触发事件
            return false;
        }

        if (itemInfo.itemType != itemType.Weapon)
        {
            return false;
        }

        if (selectedItemIds.Contains(itemInfo.itemId))//如果已经选中
        {
            selectedItemIds.Remove(itemInfo.itemId);//移除
        }
        else
        {
            selectedItemIds.Add(itemInfo.itemId);//添加
        }

        return selectedItemIds.Contains(itemInfo.itemId);
    }

    public void RequestExpandBag(int addCount, Action<bool> callback)
    {
        ProtoManager.Instance.CSExpandBag(addCount, (msg) =>
        {
            if (msg == null)
            {
                Debug.LogError("没有收到背包扩容结果");
                callback?.Invoke(false);
                return;
            }
            switch (msg.Result)
            {
                case ExpandBagResult.Success:
                    BagModel.Instance.SetCapacity(msg.Capacity);
                    callback?.Invoke(true);
                    break;

                case ExpandBagResult.NotLogin:
                    callback?.Invoke(false);
                    break;

                case ExpandBagResult.InvalidCount:
                    callback?.Invoke(false);
                    break;

                case ExpandBagResult.MaxCapacity:
                    callback?.Invoke(false);
                    break;

                default:
                    callback?.Invoke(false);
                    break;
            }
        });
    }

    private int GetQualityValue(int itemId)
    {
        ItemInfo itemInfo = ItemModel.Instance.GetItemInfo(itemId);
        if (itemInfo == null)
        {
            return 0;
        }
        switch (itemInfo.quality)
        {
            case 'S':
                return 4;

            case 'A':
                return 3;

            case 'B':
                return 2;

            case 'C':
                return 1;
        }
        return 0;
    }

    private int ComPareItems(BagItemData itemOne, BagItemData itemTwo)//物品比较方法
    {
        if (curSortType == BagSortType.Count)
        {
            int result = itemOne.Count.CompareTo(itemTwo.Count);//数量比较
            if (result != 0)
            {
                return result;
            }
        }
        else
        {
            int qualityOne = GetQualityValue(itemOne.ItemId);//获取物品品质
            int qualityTwo = GetQualityValue(itemTwo.ItemId);//获取物品品质

            int result = qualityOne.CompareTo(qualityTwo);//品质比较
            if (result != 0)
            {
                return result;
            }
        }
        return itemOne.ItemId.CompareTo(itemTwo.ItemId);//结果相同
    }

    private void QuickSort(List<BagItemData> bagItems, int left, int right)//快速排序
    {
        if (left > right)
        {
            return;
        }
        int i = left;
        int j = right;
        BagItemData pivot = bagItems[(left + right) / 2];
        while (i <= j)
        {
            while (ComPareItems(bagItems[i], pivot) > 0)
            {
                i++;
            }
            while (ComPareItems(bagItems[j], pivot) < 0)
            {
                j--;
            }
            if (i <= j)
            {
                BagItemData temp = bagItems[i];
                bagItems[i] = bagItems[j];
                bagItems[j] = temp;
                i++;
                j--;
            }
        }
        if (left < j)
        {
            QuickSort(bagItems, left, j);
        }

        if (i < right)
        {
            QuickSort(bagItems, i, right);
        }
    }

    public void ChangeSortType(int value)
    {
        switch (value)
        {
            case 0:
                curSortType = BagSortType.Quality;
                break;
            case 1:
                curSortType = BagSortType.Count;
                break;
        }
    }

    public void ResetFilter()
    {
        curType = BagFilterType.Weapon;
    }
    public void ExitRecycleMode()
    {
        isRecycleMode = false;
        isRecycleRequesting = false;
        selectedItemIds.Clear();
    }
}

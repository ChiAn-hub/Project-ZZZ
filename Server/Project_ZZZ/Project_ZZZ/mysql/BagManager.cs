using System;
using System.Collections.Generic;
using System.Linq;

public enum RecycleResult
{
    None = 0,//默认
    Success = 1,//成功
    NotLogin = 2,//未登录
    InvalidSelection = 3,//数据异常,
    ItemNotFound = 4,//道具未找到
    ItemNotRecyclable = 5,//不可被回收
    ServerError = 6,//服务器错误
    ItemEquipped = 7
}

public enum ExpandBagResult
{
    None = 0,
    Success = 1,
    NotLogin = 2,
    InvalidCount = 3,
    MaxCapacity = 4,
    ServerError = 5
}

namespace Project_ZZZ.mysql
{
    internal class BagManager : Singleton<BagManager>
    {

        private const int DefaultCapacity = 80;//默认背包容量
        private const int MaxCapacity = 200;//最大背包容量

        public List<PlayerItem> GetItems(int userId)
        {
            if (userId <= 0)
            {
                return null;
            }
            try
            {
                return MySqlSugarClient.Instance.m_sqlSugarClient.Queryable<PlayerItem>().Where(item => item.UserId == userId && item.Count > 0)
                    .OrderBy(item => item.ItemId).ToList();
            }
            catch (Exception e)
            {
                Debug.LogError("查询玩家背包失败：" + e);
                return null;
            }
        }
        public RecycleResult RecycleItems(int userId, List<int> itemIds, out long curGold, out List<PlayerItem> curItems)
        {
            curGold = 0;
            curItems = null;
            if (userId <= 0)
            {
                return RecycleResult.NotLogin;
            }
            if (itemIds == null || itemIds.Count == 0 || itemIds.Count > 80)
            {
                return RecycleResult.InvalidSelection;
            }
            List<int> selectedIds = itemIds.Distinct().ToList();//去重
            var db = MySqlSugarClient.Instance.m_sqlSugarClient;
            try
            {
                db.Ado.BeginTran();
                List<PlayerItem> ownedItems = db.Queryable<PlayerItem>().
                            Where(item => item.UserId == userId && selectedIds.Contains(item.ItemId) && item.Count > 0).ToList();//查询玩家所拥有的道具
                if (ownedItems.Count != selectedIds.Count)
                {
                    db.Ado.RollbackTran();
                    return RecycleResult.ItemNotFound;
                }
                List<ItemDefinition> definitions = db.Queryable<ItemDefinition>().
                    Where(item => selectedIds.Contains(item.ItemId) && item.ItemType == 1 && item.RecycleGold > 0).ToList();//查询道具所对应的回收价格
                if (definitions.Count != selectedIds.Count)
                {
                    db.Ado.RollbackTran();
                    return RecycleResult.ItemNotRecyclable;
                }
                int equippedCount = db.Queryable<PlayerEquipment>().Where(e => e.UserId == userId && selectedIds.Contains(e.ItemId)).Count();
                if (equippedCount > 0)
                {
                    db.Ado.RollbackTran();
                    return RecycleResult.ItemEquipped;
                }
                Dictionary<int, ItemDefinition> definitionDict = definitions.ToDictionary(item => item.ItemId, item => item);//用字典存储道具ID和对应的信息
                long addGold = 0;
                foreach (PlayerItem playerItem in ownedItems)
                {
                    ItemDefinition definition = definitionDict[playerItem.ItemId];
                    long itemGold = checked(definition.RecycleGold * (long)playerItem.Count);//计算回收后的价格
                    addGold = checked(addGold + itemGold);//计算回收后获取的总价格
                }
                PlayerProfile profile = db.Queryable<PlayerProfile>().Where(item => item.UserId == userId).Single();
                if (profile == null)
                {
                    throw new Exception("没有找到玩家信息");
                }
                long newGold = checked(profile.Gold + addGold);//计算出玩家金币
                int deleteResult = db.Deleteable<PlayerItem>().Where(item => item.UserId == userId && selectedIds.Contains(item.ItemId)).ExecuteCommand();//进行删除
                if (deleteResult != selectedIds.Count)
                {
                    throw new Exception("删除回收物品失败");
                }
                int updateGoldResult = db.Updateable<PlayerProfile>().SetColumns(item => item.Gold == newGold).Where(item => item.UserId == userId).ExecuteCommand();//更新金币
                if (updateGoldResult != 1)
                {
                    throw new Exception("更新玩家金币失败");
                }
                curGold = newGold;
                curItems = db.Queryable<PlayerItem>().Where(item => item.UserId == userId && item.Count > 0).OrderBy(item => item.ItemId).ToList();
                db.Ado.CommitTran();
                return RecycleResult.Success;
            }
            catch (Exception e)
            {
                try
                {
                    db.Ado.RollbackTran();
                }
                catch
                {

                }
                Debug.LogError("回收物品失败：" + e);
                return RecycleResult.ServerError;
            }
        }

        public int GetCapacity(int userId)//获取背包容量
        {
            if (userId <= 0)
            {
                return 0;
            }

            try
            {
                PlayerProfile profile = MySqlSugarClient.Instance.m_sqlSugarClient.Queryable<PlayerProfile>().Where(item => item.UserId == userId).Single();
                if (profile == null)
                {
                    return 0;
                }
                if (profile.BagCapacity <= 0)
                {
                    return DefaultCapacity;
                }
                return profile.BagCapacity;
            }
            catch (Exception e)
            {
                Debug.LogError("查询玩家背包容量失败：" + e);
                return 0;
            }
        }

        public ExpandBagResult ExpandBag(int userId, int addCount, out int newCapacity)//背包扩容
        {
            newCapacity = 0;
            if (userId <= 0)
            {
                return ExpandBagResult.NotLogin;//未登录
            }
            if (addCount <= 0)
            {
                return ExpandBagResult.InvalidCount;//扩容数量不合法
            }
            try
            {
                var db = MySqlSugarClient.Instance.m_sqlSugarClient;
                PlayerProfile profile = db.Queryable<PlayerProfile>().Where(item => item.UserId == userId).Single();
                if (profile == null)
                {
                    return ExpandBagResult.ServerError;
                }

                int oldCapacity = profile.BagCapacity;
                if (oldCapacity <= 0)
                {
                    oldCapacity = DefaultCapacity;
                }
                if (oldCapacity + addCount > MaxCapacity)
                {
                    newCapacity = oldCapacity;
                    return ExpandBagResult.MaxCapacity;//超过最大容量
                }

                newCapacity = oldCapacity + addCount;
                int capacity = newCapacity;
                int UpdateResult = db.Updateable<PlayerProfile>().SetColumns(player => player.BagCapacity == capacity).Where(item => item.UserId == userId).ExecuteCommand();
                if (UpdateResult != 1)
                {
                    newCapacity = oldCapacity;
                    throw new Exception("更新玩家背包容量失败");
                }
                return ExpandBagResult.Success;
            }
            catch (Exception e)
            {
                Debug.LogError("扩容背包失败：" + e);
                newCapacity = 0;
                return ExpandBagResult.ServerError;
            }
        }
    }
}

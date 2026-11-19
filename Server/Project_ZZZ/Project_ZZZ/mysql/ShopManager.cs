using System;
using System.Collections.Generic;

public enum BuyResult
{
    None = 0,//默认
    Success = 1,//成功
    NotLogin = 2,//未登录
    InvalidCount = 3,//数量不合法
    ProductNotFound = 4,//商品不存在
    NotEnoughGold = 5,//金币不足
    ServerError = 6//服务器错误
}

namespace Project_ZZZ.mysql
{
    internal class ShopManager : Singleton<ShopManager>
    {
        public List<ShopProduct> GetProducts()
        {
            try
            {
                return MySqlSugarClient.Instance.m_sqlSugarClient.Queryable<ShopProduct>().Where(product => product.IsActive)
                    .OrderBy(product => product.ProductId).ToList();
            }
            catch (Exception e)
            {
                Debug.LogError("获取商店商品失败：" + e);
                return null;
            }
        }
        public BuyResult BuyProduct(int userId, int productId, int buyCount, out long curGold, out List<PlayerItem> curItems)
        {
            curGold = 0;
            curItems = null;
            if (userId <= 0)
            {
                return BuyResult.NotLogin;
            }
            if (buyCount <= 0 || buyCount > 99)//数量不合法
            {
                return BuyResult.InvalidCount;
            }
            var db = MySqlSugarClient.Instance.m_sqlSugarClient;
            try
            {
                db.Ado.BeginTran();//开启事务
                ShopProduct product = db.Queryable<ShopProduct>().Where(item => item.ProductId == productId && item.IsActive).Single();
                if (product == null)//服务器未找到
                {
                    db.Ado.RollbackTran();//回滚事务
                    return BuyResult.ProductNotFound;
                }
                if (product.Price <= 0 || product.UnitCount <= 0)//数量不合法
                {
                    db.Ado.RollbackTran();//回滚事务
                    return BuyResult.InvalidCount;
                }
                long totalPrice;//总价
                int addItemCount;//添加物品数量
                try
                {
                    totalPrice = checked(product.Price * buyCount);
                    addItemCount = checked(product.UnitCount * buyCount);
                }
                catch (Exception e)
                {
                    Debug.LogError("购买商品失败：" + e);
                    db.Ado.RollbackTran();
                    return BuyResult.InvalidCount;
                }
                int updateGoldResult = db.Updateable<PlayerProfile>().SetColumns(newProfile => newProfile.Gold == newProfile.Gold - totalPrice).
                                        Where(newProfile => newProfile.UserId == userId && newProfile.Gold >= totalPrice).ExecuteCommand();
                if (updateGoldResult != 1)
                {
                    db.Ado.RollbackTran();//回滚事务
                    return BuyResult.NotEnoughGold;
                }
                string addItemsql = "INSERT INTO player_item " + //对对应的表进行插入
                                    "(`UserId`, `ItemId`, `Count`) " + //查入的各个数据的列名
                                    "VALUES (@UserId, @ItemId, @Count) " + //要插入的数据
                                    "ON DUPLICATE KEY UPDATE " +//如果已经存在，则执行下面的代码
                                    "`Count` = `Count` + @Count";//更新数量
                int addItemResult = db.Ado.ExecuteCommand(addItemsql, new
                {
                    UserId = userId,
                    ItemId = product.ItemId,
                    Count = addItemCount,
                });
                if (addItemResult <= 0)
                {
                    throw new Exception("增加背包物品失败");
                }
                PlayerProfile profile = db.Queryable<PlayerProfile>().Where(item => item.UserId == userId).Single();
                QuestEvent.RaiseProgress(userId, QuestTarget.BuyItem, addItemCount);
                if (profile == null)
                {
                    throw new Exception("购买后获取玩家信息失败");
                }
                curGold = profile.Gold;
                curItems = db.Queryable<PlayerItem>().Where(item => item.UserId == userId && item.Count > 0).ToList();
                db.Ado.CommitTran();//提交事务
                return BuyResult.Success;
            }
            catch (Exception e)
            {
                try
                {
                    db.Ado.RollbackTran();//回滚事务
                }
                catch
                {
                }
                Debug.LogError("购买商品失败：" + e);
                return BuyResult.ServerError;
            }
        }
    }
}

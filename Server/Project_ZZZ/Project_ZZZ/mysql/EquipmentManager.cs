using System;
using System.Collections.Generic;
public enum EquipAction
{
    Equip = 1,
    Unequip = 2
}

public enum EquipResult
{
    None = 0,//无
    Success = 1,//成功
    NotLogin = 2,//未登录
    InvalidAction = 3,//状态数据异常
    InvalidCharacter = 4,//角色数据异常
    ItemNotOwned = 5,//道具未获取
    NotWeapon = 6,//没有对应武器
    ServerError = 7//服务器错误
}
namespace Project_ZZZ.mysql
{
    internal class EquipmentManager : Singleton<EquipmentManager>
    {
        public EquipResult ChangeEquipment(int userId, int characterId, int itemId, EquipAction action)
        {
            if (userId <= 0)
            {
                return EquipResult.NotLogin;
            }

            if (action != EquipAction.Equip && action != EquipAction.Unequip)
            {
                return EquipResult.InvalidAction;
            }

            PlayerCharacter character = CharacterManager.Instance.GetCharacter(userId, characterId);

            if (character == null)
            {
                return EquipResult.InvalidCharacter;
            }

            if (action == EquipAction.Unequip)
            {
                return Unequip(userId, characterId);
            }
            return Equip(userId, characterId, itemId);
        }
        private EquipResult Equip(int userId, int characterId, int itemId)
        {
            var db = MySqlSugarClient.Instance.m_sqlSugarClient;
            try
            {
                PlayerItem playerItem = db.Queryable<PlayerItem>().Where(item => item.UserId == userId && item.ItemId == itemId && item.Count > 0).Single();//查询玩家所拥有的道具
                if (playerItem == null)
                {
                    return EquipResult.ItemNotOwned;
                }
                ItemDefinition definition = db.Queryable<ItemDefinition>().Where(item => item.ItemId == itemId).Single();
                if (definition == null || definition.ItemType != 1)//未查到对应的武器信息,获得类型不为武器时
                {
                    return EquipResult.NotWeapon;
                }
                db.Ado.BeginTran();
                // 卸下当前角色原来的装备。
                // 如果这个物品属于其他角色，也从其他角色身上卸下。
                PlayerEquipment oldEquipment = db.Queryable<PlayerEquipment>().
                                        Where(player => player.UserId == userId && player.CharacterId == characterId).Single();//获得当前玩家穿戴装备的信息
                if (oldEquipment != null && oldEquipment.ItemId == itemId)//玩家已经穿戴了该装备,直接退出,不重复处理
                {
                    db.Ado.CommitTran();//提交事务
                    return EquipResult.Success;
                }
                db.Deleteable<PlayerEquipment>().Where(e => e.UserId == userId && (e.CharacterId == characterId || e.ItemId == itemId)).ExecuteCommand();//删除该角色原装备和新武器在其他角色身上,也从其他角色身上卸下
                PlayerEquipment equipment = new PlayerEquipment { UserId = userId, CharacterId = characterId, ItemId = itemId };//创建武器装备信息
                int result = db.Insertable(equipment).ExecuteCommand();//插入到数据库
                if (result != 1)
                {
                    throw new Exception("写入装备数据失败");
                }
                QuestEvent.RaiseProgress(userId, QuestTarget.EquipWeapon, 1);//更新任务
                if (oldEquipment != null)
                {
                    QuestEvent.RaiseProgress(userId, QuestTarget.ChangeWeapon, 1);//更新任务
                }
                db.Ado.CommitTran();

                return EquipResult.Success;
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

                Debug.LogError("穿戴装备失败：" + e);
                return EquipResult.ServerError;
            }
        }
        private EquipResult Unequip(int userId, int characterId)
        {
            try
            {
                var db = MySqlSugarClient.Instance.m_sqlSugarClient;
                db.Deleteable<PlayerEquipment>().Where(e => e.UserId == userId && e.CharacterId == characterId).ExecuteCommand();
                // 即使原本没有装备，也视为卸下成功。
                return EquipResult.Success;
            }
            catch (Exception e)
            {
                Debug.LogError("卸下装备失败：" + e);
                return EquipResult.ServerError;
            }
        }
        public Dictionary<int, int> GetEquipmentMap(int userId)//获取穿戴信息
        {
            Dictionary<int, int> result = new Dictionary<int, int>();
            if (userId <= 0)
            {
                return result;
            }
            try
            {
                List<PlayerEquipment> equipmentList = MySqlSugarClient.Instance.m_sqlSugarClient.Queryable<PlayerEquipment>().Where(e => e.UserId == userId).ToList();
                foreach (PlayerEquipment equipment in equipmentList)
                {
                    result[equipment.ItemId] = equipment.CharacterId;
                }
            }
            catch (Exception e)
            {
                Debug.LogError("读取装备关系失败：" + e);
            }
            return result;
        }
        public ItemDefinition GetEquippedItem(int userId, int characterId)//获取穿戴对应的装备信息
        {
            try
            {
                var db = MySqlSugarClient.Instance.m_sqlSugarClient;
                PlayerEquipment equipment = db.Queryable<PlayerEquipment>().Where(e => e.UserId == userId && e.CharacterId == characterId).Single();
                if (equipment == null)
                {
                    return null;
                }
                return db.Queryable<ItemDefinition>().Where(item => item.ItemId == equipment.ItemId).Single();
            }
            catch (Exception e)
            {
                Debug.LogError("读取角色装备失败：" + e);
                return null;
            }
        }
    }
}

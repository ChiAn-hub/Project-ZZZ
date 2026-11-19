using System;
using System.Collections.Generic;

public enum LevelUpResult
{
    None = 0,
    Success = 1,
    NotLogin = 2,
    InvalidCharacter = 3,
    InvalidItems = 4,
    NotEnoughItems = 5,
    MaxLevel = 6,
    ServerError = 7
}

namespace Project_ZZZ.mysql
{
    internal class CharacterManager : Singleton<CharacterManager>
    {
        private const int EllenId = 1;
        private const int MaxLevel = 60;//最大等级

        private const int HpGrowth = 50;//每级血量成长
        private const int AttackGrowth = 5;//每级攻击成长
        private const int ImpactGrowth = 2;//每级暴击成长
        private const int DefenseGrowth = 3;//每级防御成长

        private int GetNeedExp(int level)//当前等级升到下一级需要的经验
        {
            return 1000 + (level - 1) * 500;
        }

        private int GetExpItemValue(int itemId)//经验道具提供的经验
        {
            switch (itemId)
            {
                case 2001:
                    return 100;

                case 2002:
                    return 600;

                case 2003:
                    return 3000;

                default:
                    return 0;
            }
        }

        public PlayerCharacter GetCharacter(int userId, int characterId)
        {
            if (userId <= 0 || characterId != EllenId)
            {
                return null;
            }
            try
            {
                var db = MySqlSugarClient.Instance.m_sqlSugarClient;
                PlayerCharacter character = db.Queryable<PlayerCharacter>().Where(c => c.UserId == userId && c.CharacterId == characterId).Single();
                if (character != null)
                {
                    return character;
                }
                //第一次获取时创建数据
                character = new PlayerCharacter
                {
                    UserId = userId,
                    CharacterId = EllenId,
                    Level = 1,
                    Exp = 0,

                    CurHp = 600,
                    BaseMaxHp = 600,
                    BaseAttack = 20,
                    BaseImpact = 10,
                    BaseDefense = 10,

                    CriticalRate = 20,
                    CriticalDamage = 150
                };
                int result = db.Insertable(character).ExecuteCommand();//插入进数据库
                return result == 1 ? character : null;
            }
            catch (Exception e)
            {
                Debug.LogError("获取角色数据失败：" + e);
                return null;
            }
        }

        public bool UpdateHp(int userId, int characterId, int curHp)//更新角色当前血量
        {
            if (userId <= 0 || characterId != EllenId || curHp < 0)
            {
                return false;
            }

            try
            {
                var db = MySqlSugarClient.Instance.m_sqlSugarClient;
                int result = db.Updateable<PlayerCharacter>().SetColumns(c => c.CurHp == curHp)
                    .Where(c => c.UserId == userId && c.CharacterId == characterId).ExecuteCommand();//更新数据库角色当前血量
                return result == 1;
            }
            catch (Exception e)
            {
                Debug.LogError("更新角色血量失败：" + e);
                return false;
            }
        }

        public LevelUpResult LevelUp(int userId, int characterId, List<ExpItemUseData> useItems)
        {
            if (userId <= 0)
            {
                return LevelUpResult.NotLogin;
            }

            if (characterId != EllenId)
            {
                return LevelUpResult.InvalidCharacter;
            }

            if (useItems == null || useItems.Count == 0 || useItems.Count > 3)
            {
                return LevelUpResult.InvalidItems;
            }

            Dictionary<int, int> useItemDict = new Dictionary<int, int>();
            long addExp = 0;

            foreach (ExpItemUseData item in useItems)//遍历使用道具
            {
                int expValue = GetExpItemValue(item.ItemId);

                if (expValue <= 0 || item.Count <= 0 || useItemDict.ContainsKey(item.ItemId))
                {
                    return LevelUpResult.InvalidItems;
                }

                useItemDict.Add(item.ItemId, item.Count);//记录使用的道具和数量

                addExp = checked(addExp + (long)expValue * item.Count);//计算道具加成经验
            }

            var db = MySqlSugarClient.Instance.m_sqlSugarClient;
            try
            {
                db.Ado.BeginTran();
                PlayerCharacter character = db.Queryable<PlayerCharacter>().Where(c => c.UserId == userId && c.CharacterId == characterId).Single();
                if (character == null)
                {
                    db.Ado.RollbackTran();
                    return LevelUpResult.InvalidCharacter;
                }

                if (character.Level >= MaxLevel)
                {
                    db.Ado.RollbackTran();
                    return LevelUpResult.MaxLevel;
                }

                List<int> itemIds = new List<int>(useItemDict.Keys);
                List<PlayerItem> ownedItems = db.Queryable<PlayerItem>().Where(item => item.UserId == userId && itemIds.Contains(item.ItemId)).ToList();

                if (ownedItems.Count != useItemDict.Count)
                {
                    db.Ado.RollbackTran();
                    return LevelUpResult.NotEnoughItems;
                }

                foreach (PlayerItem ownedItem in ownedItems)
                {
                    int useCount = useItemDict[ownedItem.ItemId];

                    if (ownedItem.Count < useCount)
                    {
                        db.Ado.RollbackTran();
                        return LevelUpResult.NotEnoughItems;
                    }
                }

                long newExp = checked(character.Exp + addExp);//计算总经验
                int newLevel = character.Level;//当前等级

                while (newLevel < MaxLevel)
                {
                    int needExp = GetNeedExp(newLevel);//计算升级所需经验

                    if (newExp < needExp)
                    {
                        break;
                    }

                    newExp -= needExp;//扣除升级所需经验
                    newLevel++;//等级提升
                }

                int levelUpCount = newLevel - character.Level;//计算升级次数

                character.Level = newLevel;//设置新等级
                character.Exp = newLevel >= MaxLevel ? 0 : newExp;//设置新经验
                character.BaseMaxHp = checked(character.BaseMaxHp + HpGrowth * levelUpCount);//计算最大生命值
                character.CurHp = checked(character.CurHp + HpGrowth * levelUpCount);//计算当前生命值
                character.BaseAttack = checked(character.BaseAttack + AttackGrowth * levelUpCount);//计算攻击力
                character.BaseImpact = checked(character.BaseImpact + ImpactGrowth * levelUpCount);//计算暴击率
                character.BaseDefense = checked(character.BaseDefense + DefenseGrowth * levelUpCount);//计算防御力

                foreach (PlayerItem ownedItem in ownedItems)//遍历使用的道具
                {
                    int newCount = ownedItem.Count - useItemDict[ownedItem.ItemId];//计算剩余数量
                    if (newCount <= 0)//如果剩余数量小于等于0，则删除该道具
                    {
                        int deleteResult = db.Deleteable<PlayerItem>().Where(item => item.UserId == userId && item.ItemId == ownedItem.ItemId).ExecuteCommand();

                        if (deleteResult != 1)
                        {
                            throw new Exception("删除经验道具失败");
                        }
                    }
                    else
                    {
                        int updateItemResult = db.Updateable<PlayerItem>().SetColumns(item => item.Count == newCount)
                            .Where(item => item.UserId == userId && item.ItemId == ownedItem.ItemId).ExecuteCommand();//更新道具数量

                        if (updateItemResult != 1)
                        {
                            throw new Exception("更新经验道具数量失败");
                        }
                    }
                }

                int updateCharacterResult = db.Updateable(character).ExecuteCommand();
                if (updateCharacterResult != 1)
                {
                    throw new Exception("更新角色升级数据失败");
                }

                if (levelUpCount > 0)
                {
                    QuestEvent.RaiseProgress(userId, QuestTarget.CharacterLevelUp, levelUpCount);
                }

                db.Ado.CommitTran();

                return LevelUpResult.Success;
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

                Debug.LogError("角色升级失败：" + e);

                return LevelUpResult.ServerError;
            }
        }
    }
}
using System;
using System.Collections.Generic;

public enum QuestState//任务状态
{
    NoAccept = 0,//未接取
    Accept = 1,//接取
    FinishButNotGetReward = 2,//完成但是未领取奖励
    FinishAndGetReward = 3//完成并且领取奖励
}

public enum QuestTarget//任务目标
{
    None = 0,//无
    BuyItem,//购买商品
    EquipWeapon,//穿戴装备
    CharacterLevelUp,//角色升级
    KillEnemy,//杀死敌人
    EnhanceWeapon,//强化武器
    CompleteQuest,//完成其他任务
    ChangeWeapon,//更换武器
    MoveDistance//移动一定距离
}

public enum QuestOperationResult//任务选择结果
{
    None = 0,//无
    Success = 1,//成功
    NotLogin = 2,//未登录
    QuestNotFound = 3,//任务未找到
    AlreadyAccepted = 4,//已经被接取
    InvalidState = 5,//无效状态
    ServerError = 6,//服务器错误
    InvalidAction = 7,//无效操作
    InvalidAmount = 8,//无效的任务进度数量
}

public enum QuestAction//CS任务操作
{
    GetList = 1,//获取玩家任务
    Accept = 2,//接取任务
    GiveUp = 3,//放弃任务
    GetReward = 4//领取奖励
}

namespace Project_ZZZ.mysql
{
    internal class QuestManage : Singleton<QuestManage>
    {
        private bool isInitialized;//防止重复订阅
        private const int CharacterExpItemId = 2001;//角色经验道具ID
        private const int WeaponExpItemId = 4001;//武器经验道具ID
        public void Init()
        {
            if (isInitialized)
            {
                return;
            }
            QuestEvent.ProgressChanged += OnProgressChanged;
            isInitialized = true;
        }
        private void OnProgressChanged(int userId, QuestTarget target, double amount)
        {
            AddProgress(userId, target, amount);
        }
        public List<PlayerQuest> GetPlayerAllQuest(int userId)//获取对应用户的所有任务
        {
            if (userId <= 0)
            {
                return null;
            }
            try
            {
                return MySqlSugarClient.Instance.m_sqlSugarClient.Queryable<PlayerQuest>().Where(User => User.UserId == userId)
                    .OrderBy(quest => quest.QuestId).ToList();//查询,并按任务ID排序并返回
            }
            catch (Exception e)
            {

                Debug.LogError("获取玩家任务失败：" + e);
                return null;
            }
        }
        public QuestOperationResult AcceptQuest(int userId, int questId)//玩家接取任务
        {
            if (userId <= 0)
            {
                return QuestOperationResult.NotLogin;
            }
            try
            {
                var db = MySqlSugarClient.Instance.m_sqlSugarClient;
                QuestDefinition quest = db.Queryable<QuestDefinition>().Where(Quest => Quest.QuestId == questId).Single();//任务总表中查询该任务并获取
                if (quest == null)//未查到
                {
                    return QuestOperationResult.QuestNotFound;
                }
                PlayerQuest oldQuest = db.Queryable<PlayerQuest>().Where(Quest => Quest.UserId == userId && Quest.QuestId == questId).Single();//根据该任务Id去查找对应玩家的任务状态
                if (oldQuest != null)
                {
                    if (oldQuest.State == QuestState.Accept)//任务已经被接取了
                    {
                        return QuestOperationResult.AlreadyAccepted;
                    }
                    return QuestOperationResult.InvalidState;
                }
                PlayerQuest newQuest = new PlayerQuest
                {
                    QuestId = questId,
                    UserId = userId,
                    State = QuestState.Accept,
                    CurNum = 0,
                };
                int result = db.Insertable(newQuest).ExecuteCommand();//插入数据
                return result > 0 ? QuestOperationResult.Success : QuestOperationResult.ServerError;
            }
            catch (Exception e)
            {
                Debug.LogError("接取任务失败：" + e);
                return QuestOperationResult.ServerError;
            }
        }
        public QuestOperationResult GiveUpQuest(int userId, int questId)//玩家放弃任务
        {
            if (userId <= 0)
            {
                return QuestOperationResult.NotLogin;
            }
            try
            {
                var db = MySqlSugarClient.Instance.m_sqlSugarClient;
                PlayerQuest playerQuest = db.Queryable<PlayerQuest>().Where(Quest => Quest.UserId == userId && Quest.QuestId == questId).Single();//根据该任务Id去查找对应玩家的任务状态
                if (playerQuest == null)//未找到,可能未接取
                {
                    return QuestOperationResult.QuestNotFound;
                }
                if (playerQuest.State != QuestState.Accept)//任务状态只有是接取时才能放弃
                {
                    return QuestOperationResult.InvalidState;
                }
                int result = db.Deleteable<PlayerQuest>().Where(Quest => Quest.UserId == userId && Quest.QuestId == questId).ExecuteCommand();//删除对应的数据
                return result == 1 ? QuestOperationResult.Success : QuestOperationResult.ServerError;
            }
            catch (Exception e)
            {
                Debug.LogError("放弃任务失败：" + e);
                return QuestOperationResult.ServerError;
            }
        }
        public QuestOperationResult GetReward(int userId, int questId)//玩家获取奖励
        {
            if (userId <= 0)
            {
                return QuestOperationResult.NotLogin;
            }
            var db = MySqlSugarClient.Instance.m_sqlSugarClient;
            try
            {
                db.Ado.BeginTran();//开始事务
                PlayerQuest playerQuest = db.Queryable<PlayerQuest>().Where(player => player.UserId == userId && player.QuestId == questId).Single();//根据该任务Id去查找对应玩家的任务状态
                if (playerQuest == null)//如果未找到
                {
                    db.Ado.RollbackTran();//回滚事务
                    return QuestOperationResult.QuestNotFound;
                }
                if (playerQuest.State != QuestState.FinishButNotGetReward)//任务状态只有是完成未领取奖励时才能领取
                {
                    db.Ado.RollbackTran();
                    return QuestOperationResult.InvalidState;
                }
                QuestDefinition quest = db.Queryable<QuestDefinition>().Where(Quest => Quest.QuestId == questId).Single();//任务总表中查询该任务并获取
                if (quest == null)//未查到
                {
                    db.Ado.RollbackTran();
                    return QuestOperationResult.QuestNotFound;
                }
                int updateResult = db.Updateable<PlayerQuest>().SetColumns(q => q.State == QuestState.FinishAndGetReward).
                                    Where(q => q.QuestId == questId && q.UserId == userId && q.State == QuestState.FinishButNotGetReward)
                                    .ExecuteCommand();//更新任务状态为完成并领取奖励
                if (updateResult != 1)//更新失败
                {
                    db.Ado.RollbackTran();
                    return QuestOperationResult.InvalidState;
                }
                string updatePlayerSql =//更新数据
                       "UPDATE `player_profile` " +
                       "SET `Exp` = `Exp` + @PlayerExp, " +//设置
                       "`Gold` = `Gold` + @PlayerGold " +
                       "WHERE `UserId` = @UserId";
                int result = db.Ado.ExecuteCommand(updatePlayerSql, new
                {
                    PlayerExp = quest.PlayerExp,
                    PlayerGold = quest.PlayerGold,
                    UserId = userId,
                });
                if (result != 1)
                {
                    throw new Exception("发放玩家任务奖励失败");
                }
                AddRewardItem(userId, CharacterExpItemId, quest.CharacterExp);//发放角色经验奖励
                AddRewardItem(userId, WeaponExpItemId, quest.WeaponExp);//发放武器经验奖励
                QuestEvent.RaiseProgress(userId, QuestTarget.CompleteQuest, 1);
                db.Ado.CommitTran();
                return QuestOperationResult.Success;
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

                Debug.LogError("领取任务奖励失败：" + e);
                return QuestOperationResult.ServerError;
            }
        }
        public int AddProgress(int userId, QuestTarget target, double num)//增加任务进度
        {
            if (userId <= 0 || target == QuestTarget.None || num <= 0)
            {
                return 0;
            }
            try
            {
                var db = MySqlSugarClient.Instance.m_sqlSugarClient;
                string AddProgressSql = //更新任务进程的sql语句
                    "UPDATE `player_quest` AS pq " +//更新该表,并起外号为pq方便书写
                    "INNER JOIN `quest_definition` AS q " +//与任务总表进行内连接,并起外号为q方便书写
                    "ON pq.`QuestId` = q.`QuestId` " +//连接条件,任务ID相等
                    "SET pq.`CurNum` = LEAST(" + "q.`NeedCount`, pq.`CurNum` + @Amount) " +//更新条件,更新当前进度,取最小值,防止超过任务所需数量
                    "WHERE pq.`UserId` = @UserId " + //筛选出指定任务
                    "AND pq.`State` = @AcceptState " +
                    "AND q.`Target` = @Target";
                int Count = db.Ado.ExecuteCommand(AddProgressSql, new//调用sql语句,并对应传入参数
                {
                    UserId = userId,
                    Target = target.ToString(),
                    AcceptState = (int)QuestState.Accept,
                    Amount = num
                });
                string finishQuestSql =//更新任务状态的sql语句
                    "UPDATE `player_quest` AS pq " +
                    "INNER JOIN `quest_definition` AS q " +
                    "ON pq.`QuestId` = q.`QuestId` " +
                    "SET pq.`State` = @FinishState " +
                    "WHERE pq.`UserId` = @UserId " +//筛选出指定任务:对应用户,并且接取了该任务,该任务当前进度大于等于任务所需数量时进行更改
                    "AND pq.`State` = @AcceptState " +
                    "AND pq.`CurNum` >= q.`NeedCount`";
                db.Ado.ExecuteCommand(finishQuestSql, new//调用sql语句,并对应传入参数
                {
                    FinishState = (int)QuestState.FinishButNotGetReward,
                    UserId = userId,
                    AcceptState = (int)QuestState.Accept,
                });
                return Count;//返回受影响的行数
            }
            catch (Exception e)
            {
                Debug.LogError("增加任务进度失败：" + e);
                throw;
            }
        }
        private void AddRewardItem(int userId, int itemId, int count)
        {
            if (count < 0)
            {
                throw new Exception("任务奖励数量不能小于0，ItemId：" + itemId);
            }
            if (count == 0)
            {
                return;
            }
            var db = MySqlSugarClient.Instance.m_sqlSugarClient;
            try
            {
                string addItemSql =
                    "INSERT INTO `player_item` " +//插入数据
                    "(`UserId`, `ItemId`, `Count`) " +//插入的列
                    "VALUES (@UserId, @ItemId, @Count) " +//插入的值
                    "ON DUPLICATE KEY UPDATE " +//如果存在重复的键,则更新
                    "`Count` = `Count` + @Count";//更新条件
                int result = db.Ado.ExecuteCommand(addItemSql, new
                {
                    UserId = userId,
                    ItemId = itemId,
                    Count = count
                });
                if (result <= 0)
                {
                    throw new Exception("发放任务道具失败，ItemId：" + itemId);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("添加任务奖励失败：" + e);
                throw;
            }
        }
    }
}

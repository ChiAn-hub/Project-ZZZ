using System.Collections.Generic;
using UnityEngine;

public class QuestModel
{
    private QuestInfo curQuest;//当前观看的任务
    private Dictionary<int, QuestInfo> allQuest = new Dictionary<int, QuestInfo>();//用字典存储所以任务
    private Dictionary<int, PlayerQuest> playerAcceptQuest = new Dictionary<int, PlayerQuest>();//用字典存储玩家接受到的任务
    private static QuestModel instance;
    public QuestInfo CurQuest
    {
        get
        {
            return curQuest;
        }
    }
    public static QuestModel Instance//单例
    {
        get
        {
            if (instance == null)
            {
                instance = new QuestModel();
            }
            return instance;
        }
    }
    private QuestModel()
    {
        LoadQuestConfig();
    }
    private void LoadQuestConfig()//解析配置文件
    {
        TextAsset textAsset = Resources.Load<TextAsset>("Config/Quest");
        if (textAsset == null)
        {
            Debug.LogError("未找到任务配置文件");
            return;
        }
        QuestInfoList questInfoList = JsonUtility.FromJson<QuestInfoList>(textAsset.text);
        foreach (QuestInfo q in questInfoList.QuestList)
        {
            QuestTarget target;
            if (!System.Enum.TryParse(q.Target, out target))
            {
                continue;
            }
            q.TargetType = target;
            allQuest.Add(q.Id, q);
        }
    }

    private void UpdateQuestRedPoint(int questId, QuestState oldState, QuestState newState)
    {
        string newPath = RedPointPath.GetQuestNewPath(questId);
        string completePath = RedPointPath.GetQuestCompletePath(questId);

        switch (newState)
        {
            case QuestState.Accept:
                RedPointManager.Instance.SetRedPoint(newPath, false);
                RedPointManager.Instance.SetRedPoint(completePath, false);
                break;
            case QuestState.FinishButNotGetReward:
                RedPointManager.Instance.SetRedPoint(newPath, false);
                if (oldState != QuestState.FinishButNotGetReward)
                {
                    RedPointManager.Instance.SetRedPoint(completePath, true);
                }
                break;
            case QuestState.FinishAndGetReward:
                RedPointManager.Instance.SetRedPoint(completePath, false);
                RedPointManager.Instance.SetRedPoint(newPath, false);
                break;
        }
    }

    public QuestInfo GetQuestInfo(int id)//根据id获取任务信息
    {
        if (allQuest.ContainsKey(id))
        {
            return allQuest[id];
        }
        Debug.LogError("未找到任务id为" + id + "的任务");
        return null;
    }
    public void SelectQuest(int id)//选择任务
    {
        curQuest = GetQuestInfo(id);
    }

    public QuestInfo GetCurrentQuestByType(string type)//通过类型获取当前任务
    {
        QuestInfo currentQuest = null;

        foreach (QuestInfo quest in allQuest.Values)//遍历所有任务
        {
            if (quest.Type != type)
            {
                continue;
            }

            if (playerAcceptQuest.ContainsKey(quest.Id))//判断玩家是否接收了任务
            {
                PlayerQuest playerQuest = playerAcceptQuest[quest.Id];

                if (playerQuest.State == QuestState.FinishAndGetReward)//判断任务是否完成
                {
                    continue;
                }
            }

            if (currentQuest == null || quest.Id < currentQuest.Id)//找到该类型ID的最小未完成任务
            {
                currentQuest = quest;
            }
        }

        return currentQuest;
    }
    public QuestState GetQuestState(int id)//获取任务状态
    {
        if (!allQuest.ContainsKey(id))
        {
            Debug.LogError("任务配置中不存在Id：" + id);
            return QuestState.NoAccept;
        }

        PlayerQuest playerQuest;

        if (!playerAcceptQuest.TryGetValue(id, out playerQuest))//如果玩家没有接受该任务
        {
            return QuestState.NoAccept;
        }

        return playerQuest.State;
    }
    public void UpdateProgress(QuestTarget target, float amount)//根据任务目标更新任务进度
    {
        if (target == QuestTarget.None || amount <= 0)
        {
            return;
        }
        foreach (KeyValuePair<int, PlayerQuest> pair in playerAcceptQuest)//遍历所有接收的任务
        {
            PlayerQuest playerQuest = pair.Value;
            if (playerQuest.State != QuestState.Accept)//状态不是接受
            {
                continue;
            }
            QuestInfo quest;
            if (!allQuest.TryGetValue(pair.Key, out quest))
            {
                continue;
            }
            if (quest.TargetType != target)//任务目标类型不是当前目标
            {
                continue;
            }
            playerQuest.CurNum += amount;
            if (playerQuest.CurNum >= quest.Num)//任务的目标数量已满
            {
                QuestState oldState = playerQuest.State;

                playerQuest.CurNum = quest.Num;
                playerQuest.State = QuestState.FinishButNotGetReward;

                UpdateQuestRedPoint(pair.Key, oldState, playerQuest.State);
            }
        }
    }
    public float GetQuestProgress(int id)//获取任务进程
    {
        PlayerQuest quest;
        if (playerAcceptQuest.TryGetValue(id, out quest))
        {
            return quest.CurNum;
        }
        return 0;
    }
    public void ReplacePlayerQuests(List<QuestProgressData> questDatas)//使用服务器任务数据更新本地任务
    {
        Dictionary<int, QuestState> oldQuestStates = new Dictionary<int, QuestState>();//保存旧任务状态

        foreach (KeyValuePair<int, PlayerQuest> pair in playerAcceptQuest)//遍历原来玩家玩家接取的任务并复制到新的字典中
        {
            oldQuestStates[pair.Key] = pair.Value.State;
        }

        playerAcceptQuest.Clear();//清空玩家接取的任务

        if (questDatas == null)
        {
            foreach (KeyValuePair<int, QuestState> pair in oldQuestStates)//遍历旧任务状态
            {
                string completePath = RedPointPath.GetQuestCompletePath(pair.Key);//获取任务完成红点路径
                RedPointManager.Instance.SetRedPoint(completePath, false);//设置任务完成红点为不显示
            }
            return;
        }

        HashSet<int> newQuestIds = new HashSet<int>();//使用集合存储新任务Id

        foreach (QuestProgressData questData in questDatas)//遍历服务器下发的新任务数据
        {
            if (!allQuest.ContainsKey(questData.QuestId))//如果任务Id不存在于所有任务中
            {
                Debug.LogError("没有 questId 为 " + questData.QuestId + " 的任务");
                continue;
            }

            QuestState oldState = QuestState.NoAccept;
            oldQuestStates.TryGetValue(questData.QuestId, out oldState);//获取旧任务状态

            PlayerQuest playerQuest = new PlayerQuest//创建新的玩家任务
            {
                State = questData.State,
                CurNum = (float)questData.CurNum,
            };
            playerAcceptQuest.Add(questData.QuestId, playerQuest);//添加玩家任务
            newQuestIds.Add(questData.QuestId);//添加任务ID

            UpdateQuestRedPoint(questData.QuestId, oldState, playerQuest.State);//更新任务红点
        }

        foreach (KeyValuePair<int, QuestState> pair in oldQuestStates)//遍历旧任务状态,给新的任务添加上红点
        {
            if (newQuestIds.Contains(pair.Key))//旧任务状态包含新任务ID
            {
                continue;
            }
            string completePath = RedPointPath.GetQuestCompletePath(pair.Key);//获取任务完成红点
            RedPointManager.Instance.SetRedPoint(completePath, false);//设置任务完成红点为false
        }
    }
    public bool HasActiveQuestTarget(QuestTarget target)
    {
        if (target == QuestTarget.None)
        {
            return false;
        }
        foreach (KeyValuePair<int, PlayerQuest> pair in playerAcceptQuest)
        {
            PlayerQuest playerQuest = pair.Value;
            if (playerQuest.State != QuestState.Accept)
            {
                continue;
            }
            QuestInfo quest;
            if (!allQuest.TryGetValue(pair.Key, out quest))
            {
                continue;
            }
            if (quest.TargetType == target)
            {
                return true;
            }
        }
        return false;
    }

    public void Clear()
    {
        curQuest = null;
        playerAcceptQuest.Clear();
    }

}

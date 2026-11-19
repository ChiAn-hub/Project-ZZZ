using System;
using UnityEngine;
using UnityEngine.UI;

public class OpenQuestPanelController : Singleton<OpenQuestPanelController>
{
    public void RequestQuestList(Action<bool> callback)//请求任务列表
    {
        ProtoManager.Instance.CSQuest(QuestAction.GetList, 0, (msg) =>//客户端发送任务列表请求,然后等待回复后调用回调
        {
            bool success = ApplyServerQuest(msg);
            callback?.Invoke(success);
        });
    }
    public void OnClickQuestBtn(Action<bool> callback)//点击任务按钮
    {
        QuestInfo quest = QuestModel.Instance.CurQuest;//获取当前任务信息
        if (quest == null)
        {
            Debug.Log("没有任务");
            callback?.Invoke(false);
            return;
        }
        QuestState state = QuestModel.Instance.GetQuestState(quest.Id);//根据任务信息获取任务状态
        QuestAction action;
        switch (state)//对应获取任务状态
        {
            case QuestState.NoAccept:
                action = QuestAction.Accept;
                break;

            case QuestState.Accept:
                action = QuestAction.GiveUp;
                break;

            case QuestState.FinishButNotGetReward:
                action = QuestAction.GetReward;
                break;

            case QuestState.FinishAndGetReward:
                Debug.Log("该任务奖励已经领取");
                callback?.Invoke(false);
                return;

            default:
                callback?.Invoke(false);
                return;
        }
        ProtoManager.Instance.CSQuest(action, quest.Id, (msg) =>
        {
            bool success = ApplyServerQuest(msg);
            if (success && action == QuestAction.GetReward)
            {
                PlayerInfoController.Instance.RequestPlayerInfo();//刷新玩家信息
                BagPanelController.Instance.RequestBag();//刷新背包

                string completePath = RedPointPath.GetQuestCompletePath(quest.Id);
                RedPointManager.Instance.SetRedPoint(completePath, false);

                QuestInfo nextQuest = QuestModel.Instance.GetCurrentQuestByType(quest.Type);
                if (nextQuest != null && nextQuest.Id != quest.Id)
                {
                    string newPath = RedPointPath.GetQuestNewPath(nextQuest.Id);
                    RedPointManager.Instance.SetRedPoint(newPath, true);

                    QuestModel.Instance.SelectQuest(nextQuest.Id);
                }
            }
            callback?.Invoke(success);
        });
    }
    public bool ApplyServerQuest(MsgQuest msg)//在服务器回答时调用该方法
    {
        if (msg == null)
        {
            Debug.LogError("没有收到服务器任务消息");
            return false;
        }

        if (msg.Result != QuestOperationResult.Success)
        {
            Debug.LogError("任务操作失败：" + msg.Result);
            return false;
        }

        QuestModel.Instance.ReplacePlayerQuests(msg.Quests);//同步任务状态
        return true;
    }
}

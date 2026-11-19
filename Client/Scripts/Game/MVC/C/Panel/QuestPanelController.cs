using UnityEngine;

public class QuestPanelController : Singleton<QuestPanelController>
{
    public QuestInfo GetCurQuest(string type)
    {
        return QuestModel.Instance.GetCurrentQuestByType(type);
    }
    private void OpenQuest(string type)
    {
        QuestInfo quest = QuestModel.Instance.GetCurrentQuestByType(type);
        if (quest == null)
        {
            Debug.Log("没有任务");
            return;
        }

        string newPath = RedPointPath.GetQuestNewPath(quest.Id);
        RedPointManager.Instance.SetRedPoint(newPath, false);

        QuestModel.Instance.SelectQuest(quest.Id);
        UIManager.Instance.PushPanel(UIPanelType.OpenQuestPanel);
    }
    public void OnClickQuestBtnSix()
    {
        OpenQuest(QuestType.WeaponDevelopment.ToString());
    }

    public void OnClickQuestBtnFive()
    {
        OpenQuest(QuestType.Spend.ToString());
    }

    public void OnClickQuestBtnFour()
    {
        OpenQuest(QuestType.Fight.ToString());
    }

    public void OnClickQuestBtnThree()
    {
        OpenQuest(QuestType.Daily.ToString());
    }

    public void OnClickQuestBtnTwo()
    {
        OpenQuest(QuestType.CharactorDevelopment.ToString());
    }

    public void OnClickQuestBtnOne()
    {
        OpenQuest(QuestType.Move.ToString());
    }
}

using System;
using UnityEngine;
using UnityEngine.UI;

public class OpenQuestPanel : BasePanel
{
    private Text TitleText;
    private Text ContentText;
    private Text btnText;
    private Text CharacterExpNumText;
    private Text WeaponExpNumText;
    private Text GoldNumText;
    private Button QuestBtn;

    private void Awake()
    {
        TitleText = transform.Find("OpenQuestBg/Title").GetComponent<Text>();
        ContentText = transform.Find("OpenQuestBg/Title/Content").GetComponent<Text>();
        btnText = transform.Find("OpenQuestBg/AcceptBtn/Text").GetComponent<Text>();
        CharacterExpNumText = transform.Find("OpenQuestBg/Item/ItemBg_1/ItemBg/Num").GetComponent<Text>();
        WeaponExpNumText = transform.Find("OpenQuestBg/Item/ItemBg_2/ItemBg/Num").GetComponent<Text>();
        GoldNumText = transform.Find("OpenQuestBg/Item/ItemBg_3/ItemBg/Num").GetComponent<Text>();

        QuestBtn = transform.Find("OpenQuestBg/AcceptBtn").GetComponent<Button>();
        QuestBtn.onClick.AddListener(OnClickQuestBtn);
    }
    public override void OnEnter()
    {
        base.OnEnter();
        QuestBtn.interactable = false;
        OpenQuestPanelController.Instance.RequestQuestList((success) =>
        {
            RefreshQuest();
        });
    }

    private void RefreshQuest()
    {
        QuestInfo quest = QuestModel.Instance.CurQuest;
        if (quest == null)
        {
            Debug.Log("没有任务");
            return;
        }

        string newPath = RedPointPath.GetQuestNewPath(quest.Id);
        RedPointManager.Instance.SetRedPoint(newPath, false);

        TitleText.text = quest.Name;
        float Num = QuestModel.Instance.GetQuestProgress(quest.Id);
        ContentText.text = quest.Content + "\n" + Mathf.FloorToInt(Num) + "/" + quest.Num;
        CharacterExpNumText.text = quest.CharactorExp.ToString();
        WeaponExpNumText.text = quest.WeaponExp.ToString();
        GoldNumText.text = quest.PlayerGold.ToString();
        QuestBtn.interactable = true;

        switch (QuestModel.Instance.GetQuestState(quest.Id))
        {
            case QuestState.NoAccept:
                btnText.text = "接受";
                break;

            case QuestState.Accept:
                btnText.text = "放弃";
                break;

            case QuestState.FinishButNotGetReward:
                btnText.text = "领取";
                break;

            case QuestState.FinishAndGetReward:
                btnText.text = "已领取";
                QuestBtn.interactable = false;
                break;
        }
    }

    private void OnClickQuestBtn()
    {
        QuestBtn.interactable = false;
        OpenQuestPanelController.Instance.OnClickQuestBtn((success) =>
        {
            RefreshQuest();
        });
    }
}

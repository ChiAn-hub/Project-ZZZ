using UnityEngine;

public class MainPanelController : Singleton<MainPanelController>
{
    public void OnClickEmailBtn()
    {

    }

    public void OnClickNotityBtn()
    {

    }

    public void OnClickSystemBtn()
    {
        UIManager.Instance.PushPanel(UIPanelType.SystemPanel);
    }

    public void OnClickShopBtn()
    {
        UIManager.Instance.PushPanel(UIPanelType.ShopPanel);
    }

    public void OnClickMoreBtn()
    {

    }

    public void OnClickEmplyBtn()
    {

    }

    public void OnClickQuestBtn()
    {
        UIManager.Instance.PushPanel(UIPanelType.QuestPanel);
    }

    public void OnClickBagBtn()
    {
        UIManager.Instance.PushPanel(UIPanelType.BagPanel);
    }

    public void OnClickCharactorBtn()
    {
        UIManager.Instance.PushPanel(UIPanelType.PlayerPanel);
    }

    public void OnClickAchieveBtn()
    {

    }
}

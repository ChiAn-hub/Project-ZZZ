using System;
using UnityEngine;

public class CityPanelController : Singleton<CityPanelController>
{
    public void OnClickTVBtn()
    {

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

    public void OnClickNoteBtn()
    {
        UIManager.Instance.PushPanel(UIPanelType.QuestPanel);
    }

    public void OnClickCharactorBtn()
    {
        UIManager.Instance.PushPanel(UIPanelType.PlayerPanel);
    }

    public void OnClickSwitchBtn()
    {
        UIManager.Instance.PushPanel(UIPanelType.MainPanel);
        PlayerInput.SetCanControl(false);
    }
    public void SetInputControl(bool b)
    {
        PlayerInput.SetCanControl(b);
    }

    public void Revive(Action<bool> callback = null)
    {
        EllenDataModel model = EllenDataModel.Instance;

        ProtoManager.Instance.CSCharacterRevive(model.CharacterId, (msg) =>
            {
                if (msg == null || !msg.Success)
                {
                    callback?.Invoke(false);
                    return;
                }

                model.ApplyRevive(msg.CurHp, msg.MaxHp);

                callback?.Invoke(true);
            });
    }

}

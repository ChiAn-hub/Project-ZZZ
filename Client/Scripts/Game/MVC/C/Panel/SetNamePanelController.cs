using UnityEngine;
using UnityEngine.UI;
public class SetNamePanelController : Singleton<SetNamePanelController>
{

    public void SetNameCallBack(SetNameType setNameType, string Name)
    {
        switch (setNameType)
        {
            case SetNameType.Success:
                PlayerSessionModel.Instance.IsSetName = true;
                PlayerSessionModel.Instance.PlayerName = Name;
                PlayerInfoController.Instance.RequestPlayerInfo((success) =>
                {
                    if (!success)
                    {
                        Debug.LogError("获取玩家信息失败");
                        return;
                    }
                    UIManager.Instance.PopPanel();//注册成功
                    UIManager.Instance.PushPanel(UIPanelType.CityPanel);
                });
                break;
            case SetNameType.Failed:
                Debug.Log("设置昵称失败");
                break;
        }
    }
    public void SetName(string Name)
    {
        ProtoManager.Instance.CSSetName(Name, SetNameCallBack);
    }
}

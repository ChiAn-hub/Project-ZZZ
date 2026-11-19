using UnityEngine;
using UnityEngine.UI;
public class LoadPanelController : Singleton<LoadPanelController>
{

    public void LoginCallBack(LoadType loadType, bool IsSetName)
    {
        switch (loadType)
        {
            case LoadType.Success:

                ClearRuntimeData();

                PlayerSessionModel.Instance.IsSetName = IsSetName;
                PlayerInfoController.Instance.RequestPlayerInfo((success) =>
                {
                    if (!success)
                    {
                        Debug.LogError("获取玩家信息失败");
                        return;
                    }
                    UIManager.Instance.PushPanel(UIPanelType.LoadingPanel);
                });
                break;
            case LoadType.Failed:
                Debug.Log("登录失败");
                break;
            case LoadType.PasswordError:
                Debug.Log("密码错误");
                break;
            case LoadType.NoUser:
                Debug.Log("无此用户");
                break;
        }
    }
    public void Login(string Account, string Password)
    {
        ProtoManager.Instance.CSLogin(Account, Password, LoginCallBack);
    }

    private void ClearRuntimeData()
    {
        RedPointManager.Instance.Clear();
        BagModel.Instance.Clear();
        QuestModel.Instance.Clear();
        PlayerSessionModel.Instance.Clear();
    }

}

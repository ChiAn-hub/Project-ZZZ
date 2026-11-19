using UnityEngine;
using UnityEngine.UI;
public class RegisterPanelController : Singleton<RegisterPanelController>
{
    public void RegisterCallBack(RegisterType registerType)
    {
        switch (registerType)
        {
            case RegisterType.Success:
                PlayerSessionModel.Instance.IsSetName = false;
                UIManager.Instance.PopPanel();
                break;
            case RegisterType.Failed:
                Debug.Log("注册失败");
                break;
            case RegisterType.UserExist:
                Debug.Log("用户已存在");
                break;
        }
    }
    public void Register(string Account, string Password, string rePassword)
    {
        if (Password == "" || rePassword == "")
        {
            Debug.Log("请输入密码");
            return;
        }
        if (Password != rePassword)
        {
            Debug.Log("密码不一致");
            return;
        }
        ProtoManager.Instance.CSRegister(Account, Password, RegisterPanelController.Instance.RegisterCallBack);
    }
}

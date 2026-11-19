using UnityEngine;

public class SystemPanelController : Singleton<SystemPanelController>
{
    public void OnDropdownChanged(int arg0)
    {
        switch (arg0)
        {
            case 0:
                Application.targetFrameRate = 30;
                break;
            case 1:
                Application.targetFrameRate = 45;
                break;
            case 2:
                Application.targetFrameRate = 60;
                break;
            case 3:
                Application.targetFrameRate = -1;
                break;
        }
    }
}

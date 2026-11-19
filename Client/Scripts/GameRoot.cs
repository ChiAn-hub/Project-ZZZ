using UnityEngine;
using UnityEngine.Audio;

public class GameRoot : MonoBehaviour
{
    private static GameRoot instance;
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        Application.targetFrameRate = -1;//设置帧率
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        NetManager.Instance.Connect("127.0.0.1", 8012);
        UIManager.Instance.PushPanel(UIPanelType.LoadMainPanel);
    }

    // Update is called once per frame
    void Update()
    {
        NetManager.Instance.MsgUpdate();
    }
}

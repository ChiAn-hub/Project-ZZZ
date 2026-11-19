using UnityEngine;
using UnityEngine.UI;

public class LoadMainPanel : BasePanel
{
    private Button enterButton;
    private Button systemBtn;
    private Text enterText;

    public override void Start()
    {
        systemBtn = GameObject.Find("SettingBtn").GetComponent<Button>();
        enterButton = GameObject.Find("EnterBtn").GetComponent<Button>();
        enterText = GameObject.Find("EnterBtn/Enter").GetComponent<Text>();
        enterButton.onClick.AddListener(OnenterBtnClick);
        systemBtn.onClick.AddListener(OnClickSystemBtn);
    }

    void Update()
    {
        TextAnim();
    }
    private void TextAnim()
    {
        Color c = enterText.color;
        c.a = Mathf.PingPong(Time.time * 5.0f, 1);
        enterText.color = c;
    }
    public void OnenterBtnClick()
    {
        UIManager.Instance.PushPanel(UIPanelType.LoadPanel);
    }
    public void OnClickSystemBtn()
    {
        UIManager.Instance.PushPanel(UIPanelType.SystemPanel);
    }
}

using UnityEngine;
using UnityEngine.UI;

public class LoadPanel : BasePanel
{
    private RawImage rawImageOne;
    private RawImage rawImageTwo;
    private RawImage rawImageThree;
    private RawImage rawImageFour;
    private Button registerBtn;
    private Button LoadBtn;
    private InputField AccountInput;
    private InputField PasswordInput;
    private void Awake()
    {
        rawImageOne = transform.Find("TextBg/Text/one").GetComponent<RawImage>();
        rawImageTwo = transform.Find("TextBg/Text/two").GetComponent<RawImage>();
        rawImageThree = transform.Find("TextBg/Text/three").GetComponent<RawImage>();
        rawImageFour = transform.Find("TextBg/Text/four").GetComponent<RawImage>();
        AccountInput = transform.Find("Account").GetComponent<InputField>();
        PasswordInput = transform.Find("Password").GetComponent<InputField>();

        registerBtn = transform.Find("Register/RegisterBtn").GetComponent<Button>();
        registerBtn.onClick.AddListener(OnClickRegister);

        LoadBtn = transform.Find("LoadBtn").GetComponent<Button>();
        LoadBtn.onClick.AddListener(OnClickLoadBtn);

    }

    // Update is called once per frame
    void Update()
    {
        RawImageUVUpdate(rawImageOne, 0.05f);
        RawImageUVUpdate(rawImageTwo, 0.05f);
        RawImageUVUpdate(rawImageThree, 0.05f);
        RawImageUVUpdate(rawImageFour, 0.05f);
    }
    public void OnClickLoadBtn()
    {
        LoadPanelController.Instance.Login(AccountInput.text, PasswordInput.text);
    }
    public void OnClickRegister()
    {
        UIManager.Instance.PushPanel(UIPanelType.RegisterPanel);
    }
    public void RawImageUVUpdate(RawImage rawImage, float speed)
    {
        Rect rect = rawImage.uvRect;

        rect.x += speed * Time.deltaTime;

        if (rect.x >= 1f)
        {
            rect.x -= 1f;
        }

        rawImage.uvRect = rect;
    }
}

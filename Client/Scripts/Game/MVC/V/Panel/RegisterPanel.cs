using UnityEngine;
using UnityEngine.UI;
public class RegisterPanel : BasePanel
{
    private RawImage rawImageOne;
    private RawImage rawImageTwo;
    private RawImage rawImageThree;
    private RawImage rawImageFour;
    private Button loadBtn;
    private Button RegisterBtn;
    private InputField accountInput;
    private InputField passwordInput;
    private InputField rePasswordInput;

    private void Awake()
    {
        rawImageOne = transform.Find("TextBg/Text/one").GetComponent<RawImage>();
        rawImageTwo = transform.Find("TextBg/Text/two").GetComponent<RawImage>();
        rawImageThree = transform.Find("TextBg/Text/three").GetComponent<RawImage>();
        rawImageFour = transform.Find("TextBg/Text/four").GetComponent<RawImage>();
        accountInput = transform.Find("Account").GetComponent<InputField>();
        passwordInput = transform.Find("Password").GetComponent<InputField>();
        rePasswordInput = transform.Find("RePassword").GetComponent<InputField>();

        loadBtn = transform.Find("LoadBtn").GetComponent<Button>();
        loadBtn.onClick.AddListener(OnClickloadBtn);

        RegisterBtn = transform.Find("RegisterBtn").GetComponent<Button>();
        RegisterBtn.onClick.AddListener(OnClickRegisterBtn);

    }

    // Update is called once per frame
    void Update()
    {
        RawImageUVUpdate(rawImageOne, 0.05f);
        RawImageUVUpdate(rawImageTwo, 0.05f);
        RawImageUVUpdate(rawImageThree, 0.05f);
        RawImageUVUpdate(rawImageFour, 0.05f);
    }

    public void OnClickloadBtn()
    {
        UIManager.Instance.PopPanel();
    }
    public void OnClickRegisterBtn()
    {
        RegisterPanelController.Instance.Register(accountInput.text, passwordInput.text, rePasswordInput.text);
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

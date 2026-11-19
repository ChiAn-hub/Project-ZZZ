using UnityEngine;
using UnityEngine.UI;
public class SetNamePanel : BasePanel
{
    private RawImage rawImageOne;
    private RawImage rawImageTwo;
    private RawImage rawImageThree;
    private Button SetNameBtn;
    private InputField SetNameInput;

    private void Awake()
    {
        rawImageOne = transform.Find("Text/One").GetComponent<RawImage>();
        rawImageTwo = transform.Find("Text/Two").GetComponent<RawImage>();
        rawImageThree = transform.Find("Text/Three").GetComponent<RawImage>();
        SetNameInput = transform.Find("Icon/SetNameInput").GetComponent<InputField>();

        SetNameBtn = transform.Find("Icon/EnterNameBtn").GetComponent<Button>();
        SetNameBtn.onClick.AddListener(OnClickSetNameBtn);
    }

    // Update is called once per frame
    void Update()
    {
        RawImageUVUpdate(rawImageOne, 0.05f);
        RawImageUVUpdate(rawImageTwo, 0.05f);
        RawImageUVUpdate(rawImageThree, 0.05f);
    }


    public void OnClickSetNameBtn()
    {
        SetNamePanelController.Instance.SetName(SetNameInput.text);
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

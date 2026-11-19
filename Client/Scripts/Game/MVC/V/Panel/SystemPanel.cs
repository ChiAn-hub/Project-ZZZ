using System;
using UnityEngine;
using UnityEngine.UI;

public class SystemPanel : BasePanel
{
    private Slider VideoSlider;
    private Dropdown dropdown;
    private RawImage rawImageOne;
    private RawImage rawImageTwo;
    private RawImage rawImageThree;
    private void Awake()
    {
        VideoSlider = transform.Find("Panel/One/Slider").GetComponent<Slider>();
        dropdown = transform.Find("Panel/Two/Dropdown").GetComponent<Dropdown>();
        rawImageOne = transform.Find("Bg/Text/One").GetComponent<RawImage>();
        rawImageTwo = transform.Find("Bg/Text/Two").GetComponent<RawImage>();
        rawImageThree = transform.Find("Bg/Text/Three").GetComponent<RawImage>();
        VideoSlider.onValueChanged.AddListener(OnSliderChanged);
        dropdown.onValueChanged.AddListener(OnDropdownChanged);
    }

    private void OnDropdownChanged(int arg0)//改变帧率
    {
        SystemPanelController.Instance.OnDropdownChanged(arg0);
    }

    private void OnSliderChanged(float arg0)//调节音量
    {
        MusicManage.Instance.MusicSize(arg0);
    }

    void Update()
    {
        RawImageUVUpdate(rawImageOne, 0.05f);
        RawImageUVUpdate(rawImageTwo, 0.05f);
        RawImageUVUpdate(rawImageThree, 0.05f);
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

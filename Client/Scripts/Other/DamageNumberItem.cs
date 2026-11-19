using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageNumberItem : MonoBehaviour
{
    [SerializeField]
    private Transform numberRoot;//数字父节点
    [SerializeField]
    private Image digitTemplate;//数字模板

    [SerializeField]
    private Sprite[] numberSprites = new Sprite[10];//0到9的数字图片

    private Camera mainCamera;

    private List<Image> digitImages;//已经创建的数字Image

    private float lifeTime;
    private const float MaxLifeTime = 1.5f;//存在时间

    private void Awake()
    {
        mainCamera = Camera.main;
        digitImages = new List<Image>();
    }

    void Update()
    {
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)
        {
            Release();
        }
    }

    public void Init(int damage)//生成伤害数字
    {
        HideAllDigits();
        lifeTime = MaxLifeTime;

        if (damage <= 0 || numberRoot == null || digitTemplate == null || numberSprites == null || numberSprites.Length < 10)
        {
            Release();
            return;
        }

        string damageText = damage.ToString();

        for (int i = 0; i < damageText.Length; i++)
        {
            int number = damageText[i] - '0';

            if (number < 0 || number > 9)
            {
                continue;
            }

            Image numberImage = GetDigitImage(i);

            numberImage.sprite = numberSprites[number];
            numberImage.gameObject.SetActive(true);
            numberImage.SetNativeSize();
        }
    }

    private Image GetDigitImage(int index)//获取对应图片
    {
        if (index < digitImages.Count)
        {
            return digitImages[index];
        }

        Image newDigit = Instantiate(digitTemplate, numberRoot);
        digitImages.Add(newDigit);
        return newDigit;
    }

    private void HideAllDigits()//隐藏上一次使用的数字
    {
        foreach (Image digitImage in digitImages)
        {
            if (digitImage != null)
            {
                digitImage.gameObject.SetActive(false);
            }
        }
    }

    private void Release()//回收到对象池
    {
        HideAllDigits();
        DamageNumberPool.Instance.ReleaseDamageNumber(this);
    }

    private void LateUpdate()//始终朝向摄像机
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (mainCamera == null)
        {
            return;
        }

        transform.rotation = mainCamera.transform.rotation;
    }

}

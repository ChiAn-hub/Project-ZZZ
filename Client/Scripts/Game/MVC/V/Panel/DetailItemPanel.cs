using UnityEngine;
using UnityEngine.UI;

public class DetailItemPanel : MonoBehaviour
{
    private Image itemImage;
    private Text itemNameText;
    private Text numText;
    private Text contentText;
    private Image level;

    private void Awake()
    {
        itemImage = transform.Find("ItemImage").GetComponent<Image>();

        itemNameText = transform.Find("ItemName").GetComponent<Text>();

        numText = transform.Find("Num").GetComponent<Text>();

        contentText = transform.Find("Content").GetComponent<Text>();

        level = transform.Find("Level").GetComponent<Image>();
    }

    public void ShowItem(ItemInfo itemInfo, int count)
    {
        if (itemInfo == null)
        {
            return;
        }

        gameObject.SetActive(true);

        itemImage.sprite =
            Resources.Load<Sprite>(itemInfo.path);

        itemNameText.text = itemInfo.name;

        numText.text = "当前拥有：" + count;

        contentText.text = itemInfo.content;

        level.sprite = Resources.Load<Sprite>("Prefabs/UIPrefabs/" + itemInfo.quality
        );
    }

    public void HideItem()
    {
        gameObject.SetActive(false);
    }
}
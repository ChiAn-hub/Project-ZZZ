using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class BasePanel : MonoBehaviour
{
    protected CanvasGroup canvasGroup;
    protected Button closeBtn;
    protected Transform closeTrans;

    public virtual bool HideWhenPaused
    {
        get
        {
            return true;
        }
    }

    public virtual void Start()
    {

    }
    public virtual void OnEnter()//打开界面
    {
        gameObject.SetActive(true);
        transform.SetAsLastSibling();
        if (canvasGroup == null)
        {
            canvasGroup = this.GetComponent<CanvasGroup>();
        }
        if (closeBtn == null)
        {
            closeTrans = transform.Find("CloseBtn");
            if (closeTrans != null)
            {
                closeBtn = closeTrans.GetComponent<Button>();
                closeBtn.onClick.AddListener(OnClockCloseBtn);
            }
        }
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
    }
    public virtual void OnExit()//退出界面
    {
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
        gameObject.SetActive(false);
    }
    public virtual void OnResume()//恢复界面
    {
        canvasGroup.blocksRaycasts = true;
        gameObject.SetActive(true);
        transform.SetAsLastSibling();
    }
    public virtual void OnPause()//暂停界面
    {
        canvasGroup.blocksRaycasts = false;
        if (HideWhenPaused)
        {
            gameObject.SetActive(false);
        }
    }
    public void OnClockCloseBtn()//关闭按钮
    {
        UIManager.Instance.PopPanel();
    }
}

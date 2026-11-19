using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingPanel : BasePanel
{
    private Coroutine loadingCoroutine;
    public override void OnEnter()
    {
        base.OnEnter();
        if (loadingCoroutine == null)
        {
            loadingCoroutine = StartCoroutine(AfterLoadScene());
        }
    }

    private IEnumerator AfterLoadScene()
    {
        // 1. 开始异步加载 MainScene
        AsyncOperation operation = LoadingPanelController.Instance.LoadSceneAsync("MainScene");

        // 2. 等待场景加载完成
        while (!operation.isDone)
        {
            yield return null;
        }

        yield return new WaitForSeconds(3.0f);

        UIManager.Instance.ClearAllPanel();

        if (!PlayerSessionModel.Instance.IsSetName)
        {
            UIManager.Instance.PushPanel(UIPanelType.SetNamePanel);
        }
        else
        {
            UIManager.Instance.PushPanel(UIPanelType.CityPanel);
        }

        loadingCoroutine = null;
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingPanelController : Singleton<LoadingPanelController>
{
    // 返回 AsyncOperation 方便外部获取加载进度
    public AsyncOperation LoadSceneAsync(string sceneName)
    {
        return SceneManager.LoadSceneAsync(sceneName);
    }
}

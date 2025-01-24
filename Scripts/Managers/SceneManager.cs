using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class SceneManager : MonoSingleton<SceneManager>
{
    public static string currScene = null;
    static string toScene = null;

    Action<AsyncOperation> loadCallback;

    public void LoadScene(string sceneName, Action<AsyncOperation> callback = null)
    {
        toScene = sceneName;
        loadCallback = callback;
        StartCoroutine(LoadScene());
    }

    #region LoadScene
    private IEnumerator LoadScene()
    {
        DebugTool.WarningFormat("准备传送到地图 [" + toScene + "]");
        BeforeLoadScene();
        yield return null;

        AsyncOperation async = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(toScene);
        async.allowSceneActivation = true;
        async.completed += LoadSceneCB;
    }

    private void BeforeLoadScene()
    {

    }

    private void AfterLoadScene()
    {

    }

    private void LoadSceneCB(AsyncOperation _)
    {
        if (loadCallback != null)
        {
            loadCallback.Invoke(_);
            loadCallback = null;
        }
        currScene = toScene;
        toScene = null;
        AfterLoadScene();
    }
    #endregion
}

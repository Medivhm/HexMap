//using System.Collections;
//using UnityEngine;
////using YooAsset;

//public class GameInit : MonoBehaviour
//{
//    void Start()
//    {
//        StartCoroutine(DownLoadDll());
//    }

//    IEnumerator DownLoadDll()
//    {
//        bool success = true;

//        // 这里写远端下载代码 //




//        ////////////////////////

//        yield return null;
//        if (success)
//        {
//            yield return LoadGame();
//        }
//        else
//        {
//            Debug.Log("下载远端资源失败");
//        }
//    }

//    IEnumerator LoadGame()
//    {
//        yield return LoadDefaultPackage();
//        yield return InitEntry();
//    }

//    IEnumerator LoadDefaultPackage()
//    {
//        // 设置资源包的路径或加载方式
//        string packageName = "DefaultPackage";

//        // 初始化资源系统
//        YooAssets.Initialize();

//        // 创建默认的资源包
//        var package = YooAssets.CreatePackage(packageName);
//        YooAssets.SetDefaultPackage(package);
//        string version = "1.0.0";

//        var buildinFileSystemParams = FileSystemParameters.CreateDefaultBuildinFileSystemParameters();
//        var initParameters = new OfflinePlayModeParameters();
//        initParameters.BuildinFileSystemParameters = buildinFileSystemParams;
//        var initOperation = package.InitializeAsync(initParameters);
//        yield return initOperation;

//        if (initOperation.Status == EOperationStatus.Succeed)
//            Debug.Log("资源包初始化成功！");
//        else
//            Debug.LogError($"资源包初始化失败：{initOperation.Error}");

//        var operation = package.RequestPackageVersionAsync();
//        yield return operation;
//        if (operation.Status != EOperationStatus.Succeed)
//            yield break;

//        var operation2 = package.UpdatePackageManifestAsync(version);
//        yield return operation2;
//        if (operation2.Status != EOperationStatus.Succeed)
//            yield break;
//    }

//    IEnumerator InitEntry()
//    {
//        loadPath = "Assets/Prefabs/Other/Canvas";
//        yield return LoadGameObject();
//        loadPath = "Assets/Prefabs/Other/GameStart";
//        yield return LoadGameObject();
//        yield return null;
//        GameObject.Destroy(gameObject);
//    }

//    string loadPath;
//    AssetHandle assetHandle;
//    IEnumerator LoadGameObject()
//    {
//        var loadHandle = YooAssets.LoadAssetAsync<GameObject>(loadPath);
//        while (!loadHandle.IsDone)
//        {
//            yield return null;
//        }

//        if (loadHandle.Status == EOperationStatus.Succeed)
//        {
//            GameObject.Instantiate(loadHandle.AssetObject);
//        }
//        else
//        {
//            Debug.LogError($"Failed to load {loadPath}");
//            loadHandle.Dispose();
//        }
//    }
//}
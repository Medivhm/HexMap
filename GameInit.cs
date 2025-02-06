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

//        // ����дԶ�����ش��� //




//        ////////////////////////

//        yield return null;
//        if (success)
//        {
//            yield return LoadGame();
//        }
//        else
//        {
//            Debug.Log("����Զ����Դʧ��");
//        }
//    }

//    IEnumerator LoadGame()
//    {
//        yield return LoadDefaultPackage();
//        yield return InitEntry();
//    }

//    IEnumerator LoadDefaultPackage()
//    {
//        // ������Դ����·������ط�ʽ
//        string packageName = "DefaultPackage";

//        // ��ʼ����Դϵͳ
//        YooAssets.Initialize();

//        // ����Ĭ�ϵ���Դ��
//        var package = YooAssets.CreatePackage(packageName);
//        YooAssets.SetDefaultPackage(package);
//        string version = "1.0.0";

//        var buildinFileSystemParams = FileSystemParameters.CreateDefaultBuildinFileSystemParameters();
//        var initParameters = new OfflinePlayModeParameters();
//        initParameters.BuildinFileSystemParameters = buildinFileSystemParams;
//        var initOperation = package.InitializeAsync(initParameters);
//        yield return initOperation;

//        if (initOperation.Status == EOperationStatus.Succeed)
//            Debug.Log("��Դ����ʼ���ɹ���");
//        else
//            Debug.LogError($"��Դ����ʼ��ʧ�ܣ�{initOperation.Error}");

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
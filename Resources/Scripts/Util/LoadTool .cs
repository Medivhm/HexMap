using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
//using YooAsset;

public class LoadTool
{
    public static Sprite LoadSprite(string path)
    {
//#if UNITY_EDITOR
//        //return AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/Sprite/{path}.png");
//        return (Sprite)YooAssets.LoadAssetSync($"Assets/Sprite/{path}").AssetObject;
//#else
//        return Resources.Load<Sprite>($"Delete/Sprite/{path}.png");
//        //return (Sprite)YooAssets.LoadAssetSync($"Assets/Prefabs/{path}").AssetObject;
//#endif


        return Resources.Load<Sprite>($"Delete/Sprite/{path}.png");
    }

    public static GameObject LoadPrefab(string path)
    {
        //#if UNITY_EDITOR
        //        //return AssetDatabase.LoadAssetAtPath<GameObject>($"Assets/Prefabs/{path}.prefab");
        //        return (GameObject)YooAssets.LoadAssetSync($"Assets/Prefabs/{path}").AssetObject;
        //#else
        //        var patha = $"Delete/Prefabs/"+ path;
        //        return Resources.Load<GameObject>(patha);//($"Delete/Prefabs/{path}.prefab");
        //        //return (GameObject)YooAssets.LoadAssetSync($"Assets/Prefabs/{path}").AssetObject;
        //#endif



        var patha = $"Delete/Prefabs/" + path;
        return Resources.Load<GameObject>(patha);//($"Delete/Prefabs/{path}.prefab");
    }

    public static GameObject LoadTile(string path)
    {
        return LoadPrefab($"HexTile/{path}");
    }

    public static GameObject LoadPlayer(string path)
    {
        return LoadPrefab($"Player/{path}");
    }

    public static UIEntity LoadUI(string path)
    {
        return LoadPrefab($"UI/{path}").GetComponent<UIEntity>();
    }
}
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class LoadTool
{
    public static Sprite LoadSprite(string path)
    {
#if UNITY_EDITOR
        return AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/Sprites/{path}.png");
#else

#endif
    }

    public static GameObject LoadPrefab(string path)
    {
#if UNITY_EDITOR
        return AssetDatabase.LoadAssetAtPath<GameObject>($"Assets/Prefabs/{path}.prefab");
#else

#endif
    }

    public static GameObject LoadTile(string path)
    {
        return LoadPrefab($"HexTiles/{path}");
    }
}
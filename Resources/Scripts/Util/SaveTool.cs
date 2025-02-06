using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;

public static class SaveTool
{
    private static string MapDataPath = Application.persistentDataPath + "/HexMap.json";
    private static string PlayerDataPath = Application.persistentDataPath + "/Player.json";

    public static void SaveMapData(MapData data)
    {
        string json = JsonConvert.SerializeObject(data);
        //if (!File.Exists(MapDataPath))
        //{
        //    File.Create(MapDataPath);
        //}
        File.WriteAllText(MapDataPath, json);
    }

    public static MapData LoadMapData()
    {
        if(File.Exists(MapDataPath))
        {
            string json = File.ReadAllText(MapDataPath);
            return JsonConvert.DeserializeObject<MapData>(json);
        }
        return null;
    }

    public static void SavePlayerData(PlayerData data)
    {
        string json = JsonConvert.SerializeObject(data);
        //if (!File.Exists(PlayerDataPath))
        //{
        //    File.Create(PlayerDataPath);
        //}
        File.WriteAllText(PlayerDataPath, json);
    }

    public static PlayerData LoadPlayerData()
    {
        if (File.Exists(PlayerDataPath))
        {
            string json = File.ReadAllText(PlayerDataPath);
            return JsonConvert.DeserializeObject<PlayerData>(json);
        }
        return null;
    }

    [MenuItem("DataDir/OpenDataDir")]
    public static void OpenDataDir()
    {
        EditorUtility.RevealInFinder(Application.persistentDataPath + "/Pathfinding");
    }
}
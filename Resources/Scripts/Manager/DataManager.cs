using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tiles;
using Unity.VisualScripting;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    public Dictionary<Vector2Int, HexSaveData> HexDataCache = new Dictionary<Vector2Int, HexSaveData>();
    public PlayerData PlayerDataCache = new PlayerData();

    public Dictionary<int, string> HexName = new Dictionary<int, string>()
    {
        {1, "DefaultHex" },
        {2, "ObstacleHex" },
        {3, "Hex_All" },
        {4, "Hex_DL_TR" },
        {5, "Hex_DR_TL" },
        {6, "Hex_L_R" },
    };

    public Dictionary<Vector2Int, HexSaveData> HexDatasInit = new Dictionary<Vector2Int, HexSaveData>
    {
        {new Vector2Int(1,1), new HexSaveData (1, 1, new HexDetail(3)) },
        {new Vector2Int(1,2), new HexSaveData (1, 2, new HexDetail(3)) },
        {new Vector2Int(1,3), new HexSaveData (1, 3, new HexDetail(3)) },
        {new Vector2Int(2,1), new HexSaveData (2, 1, new HexDetail(6)) },
        {new Vector2Int(0,3), new HexSaveData (0, 3, new HexDetail(5)) },
        {new Vector2Int(1,4), new HexSaveData (1, 4, new HexDetail(5)) },
        {new Vector2Int(2,3), new HexSaveData (2, 3, new HexDetail(6)) },
        {new Vector2Int(3,3), new HexSaveData (3, 3, new HexDetail(6)) },
        {new Vector2Int(4,3), new HexSaveData (4, 3, new HexDetail(6)) },
        {new Vector2Int(4,4), new HexSaveData (4, 4, new HexDetail(4)) },
        {new Vector2Int(4,5), new HexSaveData (4, 5, new HexDetail(4)) },
        {new Vector2Int(4,6), new HexSaveData (4, 6, new HexDetail(4)) },
        {new Vector2Int(5,3), new HexSaveData (5, 3, new HexDetail(3)) },
        {new Vector2Int(6,2), new HexSaveData (6, 2, new HexDetail(3)) },
        {new Vector2Int(7,2), new HexSaveData (7, 2, new HexDetail(6)) },
        {new Vector2Int(8,2), new HexSaveData (8, 2, new HexDetail(3)) },
        {new Vector2Int(7,3), new HexSaveData (7, 3, new HexDetail(3)) },
        {new Vector2Int(6,4), new HexSaveData (6, 4, new HexDetail(3)) },
        {new Vector2Int(5,5), new HexSaveData (5, 5, new HexDetail(3)) },
        {new Vector2Int(5,6), new HexSaveData (5, 6, new HexDetail(3)) },
        {new Vector2Int(4,7), new HexSaveData (7, 7, new HexDetail(3)) },
    };

    public void SaveAll()
    {
        SaveHexData();
        SavePlayerData();
    }

    public string FindName(Vector2Int mapCoord)
    {
        if (HexDataCache.ContainsKey(mapCoord))
        {
            return HexName[HexDataCache[mapCoord].detail.hexID];
        }
        return null;
    }

    public void LoadHexData()
    {
        MapData data = SaveTool.LoadMapData();
        if (data == null || data.data == null)
        {
            HexDataCache = HexDatasInit;
            SaveHexData();
            return;
        }
        else
        {
            HexDataCache.Clear();
            foreach (var hexData in data.data)
            {
                HexDataCache.Add(hexData.qr, hexData);
            }
        }
    }

    public void SaveHexData()
    {
        SaveTool.SaveMapData(new MapData
        (
            1,
            HexDataCache.Values.ToList()
        ));
    }

    public void SetHexData()
    {

    }

    public void LoadPlayerData()
    {
        PlayerData data = SaveTool.LoadPlayerData();
        if (data == null)
        {
            PlayerDataCache.lastPos = new Vector2Int(1, 1);
            return;
        }
        else
        {
            PlayerDataCache = data;
        }
    }

    public void SavePlayerData()
    {
        SavePlayerPos();
        SaveTool.SavePlayerData(PlayerDataCache);
    }

    public void SavePlayerPos()
    {
        PlayerDataCache.lastPos = Main.Instance.MainPlayer.Unit.HexCoord;
    }
}

public class PlayerData
{
    public Vector2Int lastPos;
}

public class MapData
{
    public int mapType;
    public List<HexSaveData> data;

    public MapData() { }

    public MapData(int type, List<HexSaveData> data)
    {
        this.mapType = type;
        this.data = data;
    }
}

public class HexSaveData
{
    public Vector2Int qr;
    public HexDetail detail;

    public HexSaveData() { }

    public HexSaveData(int q, int r, int hexID, int type = 2, bool isLock = false)
        : this(
        q, 
        r,
        new HexDetail
        (hexID, type, isLock)){}

    public HexSaveData(int q, int r, HexDetail detail)
    {
        this.qr = new Vector2Int(q, r);
        this.detail = detail;
    }
}

public class HexDetail
{
    public int hexID;
    public int type;
    public bool isLock;

    public HexDetail() { }

    public HexDetail(int hexID, int type = 2, bool isLock = false)
    {
        this.hexID = hexID;
        this.type = type;
        this.isLock = isLock;
    }
}
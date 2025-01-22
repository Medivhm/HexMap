using System.Collections.Generic;
using UnityEngine;

public static class TempIdToPrefabName
{
    public struct HexSaveData
    {
        public Vector2Int qr;
        public int hexID;

        public HexSaveData(int q, int r, int hexID)
        {
            this.qr = new Vector2Int(q, r);
            this.hexID = hexID;
        }
    }

    public static Dictionary<int, string> HexName = new Dictionary<int, string>()
    {
        {1, "DefaultHex" },
        {2, "ObstacleHex" },
        {3, "Hex_All" },
        {4, "Hex_DL_TR" },
        {5, "Hex_DR_TL" },
        {6, "Hex_L_R" },
    };

    public static Dictionary<Vector2Int, int> HexDatas = new Dictionary<Vector2Int, int>
    {
        {new Vector2Int(1,1), 3 },
        {new Vector2Int(1,2), 3 },
        {new Vector2Int(1,3), 3 },
        {new Vector2Int(2,1), 6 },
        {new Vector2Int(0,3), 5 },
        {new Vector2Int(1,4), 5 },
        {new Vector2Int(2,3), 6 },
        {new Vector2Int(3,3), 6 },
        {new Vector2Int(4,3), 6 },
        {new Vector2Int(4,4), 4 },
        {new Vector2Int(4,5), 4 },
        {new Vector2Int(4,6), 4 },
    };

    public static string FindName(Vector2Int mapCoord)
    {
        if (HexDatas.ContainsKey(mapCoord))
        {
            return HexName[HexDatas[mapCoord]];
        }
        return null;
    }
}
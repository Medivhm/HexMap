using Tiles;
using UniFramework.Machine;
using UnityEngine;

public class CreatePlayer : IStateNode
{
    private StateMachine _machine;

    public void OnCreate(StateMachine machine)
    {
        _machine = machine;
    }

    public void OnEnter()
    {
        DebugTool.Log("创建游戏主角");

        GridManager.Instance.SetTileInitCB(() =>
        {
            TileInitCB();
        });
    }

    public void OnExit()
    {
    }

    public void OnUpdate()
    {

    }

    private void TileInitCB()
    {
        CreatePlayerr();
        _machine.ChangeState<InitOver>();
    }

    private void CreatePlayerr()
    {
        string playerPrefabPath;
        GameObject playerPrefab;

        //playerPrefabPath = "2D/demo";
        //playerPrefab = LoadTool.LoadPlayer(playerPrefabPath);
        //Main.Instance.MainPlayer = new Player(GridManager.Instance.SpawnPlayerUnit(GridManager.Instance.Tiles[new Vector2(0, 3)], playerPrefab));

        playerPrefabPath = "3D/coolgirl";
        //playerPrefabPath = "3D/demo2";
        playerPrefab = LoadTool.LoadPlayer(playerPrefabPath);
        Vector2Int playerInitPos = DataManager.Instance.PlayerDataCache.lastPos;
        Main.Instance.MainPlayer = new Player(GridManager.Instance.SpawnPlayerUnit(GridManager.Instance.Tiles[playerInitPos], playerPrefab));

        Main.Instance.MainPlayer.Unit.gameObject.name = $"Player_{playerPrefabPath}";
    }
}
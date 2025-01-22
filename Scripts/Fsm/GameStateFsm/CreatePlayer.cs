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
        var sprite = LoadTool.LoadSprite("Units/Player");
        Main.Instance.MainPlayer = new Player(GridManager.Instance.SpawnSpriteUnit(GridManager.Instance.Tiles[new Vector2(0, 3)], sprite));
        Main.Instance.MainPlayer.Unit.gameObject.name = "Player";
    }
}
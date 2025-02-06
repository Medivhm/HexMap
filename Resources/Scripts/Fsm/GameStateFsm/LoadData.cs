using UniFramework.Machine;

public class LoadData : IStateNode
{
    private StateMachine _machine;

    public void OnCreate(StateMachine machine)
    {
        _machine = machine;
    }

    public void OnEnter()
    {
        DebugTool.Log("加载地图人物数据...");
        DataManager.Instance.LoadHexData();
        DataManager.Instance.LoadPlayerData();
        _machine.ChangeState<DoOtherThing>();
    }

    public void OnExit()
    {
    }

    public void OnUpdate()
    {

    }
}
using UniFramework.Machine;

public class HotFix : IStateNode
{
    private StateMachine _machine;

    public void OnCreate(StateMachine machine)
    {
        _machine = machine;
    }

    public void OnEnter()
    {
        DebugTool.Log("热更开始");
        _machine.ChangeState<LoadData>();
    }

    public void OnExit()
    {
    }

    public void OnUpdate()
    {

    }
}
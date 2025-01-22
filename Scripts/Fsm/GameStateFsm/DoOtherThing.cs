using UniFramework.Machine;

public class DoOtherThing : IStateNode
{
    private StateMachine _machine;

    public void OnCreate(StateMachine machine)
    {
        _machine = machine;
    }

    public void OnEnter()
    {
        DebugTool.Log("ÓÎÏ·×¼±¸");
        _machine.ChangeState<LoadGridManager>();
    }

    public void OnExit()
    {
    }

    public void OnUpdate()
    {

    }
}
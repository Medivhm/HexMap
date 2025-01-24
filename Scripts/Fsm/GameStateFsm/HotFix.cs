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
        DebugTool.Log("�ȸ���ʼ");
        _machine.ChangeState<LoadData>();
    }

    public void OnExit()
    {
    }

    public void OnUpdate()
    {

    }
}
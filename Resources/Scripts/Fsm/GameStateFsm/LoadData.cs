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
        DebugTool.Log("���ص�ͼ��������...");
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
using UniFramework.Machine;
using UnityEngine;

public class LoadGridManager : IStateNode
{
    private StateMachine _machine;

    public void OnCreate(StateMachine machine)
    {
        _machine = machine;
    }

    public void OnEnter()
    {
        DebugTool.Log("º”‘ÿGridManager");

        GameObject.Instantiate(Resources.Load("GridManager"));
        _machine.ChangeState<CreatePlayer>();
    }

    public void OnExit()
    {
        
    }

    public void OnUpdate()
    {
        
    }
}
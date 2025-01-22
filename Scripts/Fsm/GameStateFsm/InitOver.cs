using System.Collections;
using Tiles;
using UniFramework.Machine;
using UnityEngine;

public class InitOver : IStateNode
{
    private StateMachine _machine;

    public void OnCreate(StateMachine machine)
    {
        _machine = machine;
    }

    public void OnEnter()
    {
        DebugTool.Log("游戏初始化完成");
        //GameManager.Instance.StartCoroutine(Delay());
        Main.Instance.MainCameraController.StartGameCamera();
    }

    public void OnExit()
    {
    }

    public void OnUpdate()
    {

    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(1f);
    }
}
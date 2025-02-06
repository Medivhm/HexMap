using UnityEngine;
using StateMachine = UniFramework.Machine.StateMachine;

public class GameStart : MonoBehaviour
{
    StateMachine _gameState;

    private void Update()
    {
#if UNITY_EDITOR
        ///Test
        if(Input.GetKeyDown(KeyCode.F))
        {
            Main.Instance.MainCameraController.MoveToPlayer(() =>
            {
                Main.Instance.MainCameraController.FollowPlayer();
            });
        }
        ///
#endif
    }

    private void Awake()
    {
        Application.targetFrameRate = 200;
        LocalInit();

        _gameState = new StateMachine(this);
        _gameState.AddNode<LoadData>();
        _gameState.AddNode<DoOtherThing>();
        _gameState.AddNode<LoadGridManager>();
        _gameState.AddNode<CreatePlayer>();
        _gameState.AddNode<InitOver>();
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        _gameState.Run<LoadData>();
    }

    // 非ab包内容初始化
    private void LocalInit()
    {
        GameManager.Instance.Behaviour = this;
    }

    private void OnApplicationQuit()
    {
        DataManager.Instance.SaveAll();
    }
}
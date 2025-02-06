using System.Collections;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    /// <summary>
    /// 协程启动器
    /// </summary>
    public MonoBehaviour Behaviour;

    /// <summary>
    /// 开启一个协程
    /// </summary>
    public Coroutine StartCoroutine(IEnumerator enumerator)
    {
        return Behaviour.StartCoroutine(enumerator);
    }

    /// <summary>
    /// 关闭一个协程
    /// </summary>
    public void StopCoroutine(Coroutine coroutine)
    {
        Behaviour.StopCoroutine(coroutine);
    }
}
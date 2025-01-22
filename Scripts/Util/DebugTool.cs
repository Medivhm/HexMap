using Unity.VisualScripting;
using UnityEngine;

public class DebugTool
{
    public static void Error(object message)
    {
        Debug.LogError(message);
    }

    public static void ErrorFormat(string format, params object[] args)
    {
        Debug.LogErrorFormat(format, args);
    }

    public static void Warning(object message)
    {
        Debug.LogWarning(message);
    }

    public static void WarningFormat(string format, params object[] args)
    {
        Debug.LogWarningFormat(format, args);
    }

    public static void Log(object message)
    {
        Debug.Log(message);
    }

    public static void LogFormat(string format, params object[] args)
    {
        Debug.LogFormat(format, args);
    }
}
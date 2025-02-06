using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class UtilExtension
{
    public static void RemoveAllChildren(this Transform parent)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            GameObject.Destroy(parent.GetChild(i));
        }
    }

    public static void ForceRebuildLayout(this Transform trans)
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(trans.GetComponent<RectTransform>());
    }

    public static void ForceRebuildLayout(this RectTransform rect)
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
    }

    public static void ListCleanNull<T>(this List<T> list)
    {
        list.RemoveAll(item => item == null);
    }

    public static bool IsNotNull(this object obj)
    {
        return obj != null;
    }

    public static bool IsNull(this object obj)
    {
        return obj == null;
    }

    public static int ClosestIntToZero(this float value)
    {
        return (value > 0) ? Mathf.FloorToInt(value) : Mathf.CeilToInt(value);
    }

    public static Vector3 CopyAndChangeZ(this Vector3 vec3, float value)
    {
        vec3.z = value;
        return vec3;
    }

    public static Vector3 CopyAndChangeY(this Vector3 vec3, float value)
    {
        vec3.y = value;
        return vec3;
    }

    public static Vector3 CopyAndChangeX(this Vector3 vec3, float value)
    {
        vec3.x = value;
        return vec3;
    }
    public static bool IsMe(this GameObject go, GameObject otherGo)
    {
        return go.GetInstanceID().Equals(otherGo.GetInstanceID());
    }

    public static bool IsNotMe(this GameObject go, GameObject otherGo)
    {
        return !IsMe(go, otherGo);
    }
}
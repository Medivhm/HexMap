using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (T)FindObjectOfType<T>();
            }
            return instance;
        }

    }

    void Awake()
    {
        if (FindObjectsOfType<T>().Length > 1)
        {
            Destroy(this.gameObject);
            return;
        }
        this.OnAwake();
    }

    protected virtual void OnAwake()
    {

    }
}
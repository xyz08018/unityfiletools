using UnityEngine;

public class Singleton<T> where T : new()
{
    static T instance;

    public static T Instance
    {
        get
        {
            if (null == instance)
                instance = new T();
            return instance;
        }
    }

}
public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    static T instance;

    public static T Instance
    {
        get
        {
            if (null == instance)
                instance = FindObjectOfType<T>();
            return instance;
        }
    }

}

using UnityEngine;
using System.Collections;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T instance = null;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<T>() as T;
                if(FindObjectsOfType<T>().Length > 1)
                {
                    #if !NO_DEBUG
                    Debug.LogError("More than 1 singleton found");
                    #endif
                    return instance;
                }
                if(instance == null)
                {
                    GameObject singleton = new GameObject();
                    instance = singleton.AddComponent<T>();
                    singleton.name = "(Singleton) " + typeof(T).ToString();
                    #if FULL_DEBUG
                    Debug.LogWarning("Created " + singleton.name);
                    #endif
                }
            }
            return instance;
        }
    }
  
}
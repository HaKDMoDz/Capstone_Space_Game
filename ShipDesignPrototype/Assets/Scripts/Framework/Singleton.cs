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
                    Debug.LogError("More than 1 singleton found");
                    return instance;
                }
                if(instance == null)
                {
                    GameObject singleton = new GameObject();
                    instance = singleton.AddComponent<T>();
                    singleton.name = "(Singleton) " + typeof(T).ToString();
                    Debug.Log("Created " + singleton.name);
                }
            }
            return instance;
        }
    }

}
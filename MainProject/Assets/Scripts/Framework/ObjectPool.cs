using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

#region AdditionalData
[Serializable]
public struct ObjectPoolEntry
{
    public GameObject gameObject;
    public int amountToBuffer;
}
#endregion //AdditionalData

public class ObjectPool : Singleton<ObjectPool>
{

    #region Fields

    #region EditorExposed
    [SerializeField]
    private List<ObjectPoolEntry> PrefabList; //the object prefabs that the object pool will create from
    [SerializeField]
    private int defaultBufferAmount = 5;
    #endregion //EditorExposed

    #region InternalFields
    private List<GameObject> pooledObjects;
    private Transform container; //an empty to parent pooled objects under
    #endregion //InternalFields

    #endregion//Fields

    #region Methods

    #region Public

    #endregion //Public Methods

    #region Private
    //private void Start()
    //{
    //    container = new GameObject("ObjectPool").transform;

    //    pooledObjects = new List<GameObject>(PrefabList.Count);

    //    //create a list for each object prefab
    //    for (int i = 0; i < PrefabList.Count; i++)
    //    {
    //        pooledObjects[i] = new List<GameObject>();

    //        int bufferAmount = i < amountToBuffer.Length ? amountToBuffer[i] : defaultBufferAmount;

    //        for (int j = 0; j < bufferAmount; j++)
    //        {
    //            GameObject newObj = Instantiate(objectPrefabs[i]) as GameObject;
    //            newObj.name = objectPrefabs[i].name;
    //            PoolObject(newObj);
    //        }
    //    }
    //}
    #endregion //private methods
    #endregion //Methods

    

    //#endregion //Private Methods

    //#endregion //Methods
    
    

    ///// <summary>
    ///// Returns a new GameObject based on the name (objectType) provided. Will return null if object is not found.
    ///// </summary>
    ///// <param name="objectName">Name of the game object to instantiate</param>
    ///// <param name="onlyPooled">
    ///// If true, will only return a pooled object, if false, will instantiate a new object if out of pooled objects
    ///// </param>
    //public GameObject GetPooledObject(string objectName, bool onlyPooled)
    //{
    //    for (int i = 0; i < objectPrefabs.Length; i++)
    //    {
    //        if(objectPrefabs[i].name == objectName)
    //        {
    //            if(pooledObjects[i].Count>0)
    //            {
    //                GameObject pooledObject = pooledObjects[i][0];
    //                pooledObjects[i].RemoveAt(0);
    //                pooledObject.transform.parent = null;
    //                pooledObject.SetActive(true);
    //                return pooledObject;
    //            }
    //            else if(!onlyPooled)
    //            {
    //                Debug.LogError("Out of Pooled objects - instantiating new object");
    //                return Instantiate(objectPrefabs[i]) as GameObject;
    //            }
    //            break;
    //        }
    //    }
    //    Debug.LogError("Object not found or out of pooled objects");
    //    return null; //object was not found
    //}

    //public void PoolObject(GameObject obj)
    //{
    //    for (int i = 0; i < objectPrefabs.Length; i++)
    //    {
    //        if(objectPrefabs[i].name ==obj.name)
    //        {
    //            obj.SetActive(false);
    //            obj.transform.position = container.position;
    //            obj.transform.parent = container;
    //            pooledObjects[i].Add(obj);
    //            return;
    //        }
    //    }
    //}




}

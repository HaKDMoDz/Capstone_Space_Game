using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

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
    private List<ObjectPoolEntry> prefabList; //the object prefabs that the object pool will create from
    #endregion //EditorExposed

    #region InternalFields
    //private List<GameObject> pooledObjects;
    private Dictionary<GameObject, Queue<GameObject>> pooledObjectTable;
    private Transform container; //an empty to parent pooled objects under
    #endregion //InternalFields

    #endregion//Fields

    #region Methods

    #region Public
    /// <summary>
    /// Returns a new GameObject based on the name (objectType) provided. Will return null if object is not found.
    /// </summary>
    /// <param name="objectName">Name of the game object to instantiate</param>
    /// <param name="onlyPooled">
    /// If true, will only return a pooled object, if false, will instantiate a new object if out of pooled objects
    /// </param>
    public GameObject GetPooledObject(string objectName, bool onlyPooled)
    {
        //for (int i = 0; i < objectPrefabs.Length; i++)
        //{
        //    if (objectPrefabs[i].name == objectName)
        //    {
        //        if (pooledObjects[i].Count > 0)
        //        {
        //            GameObject pooledObject = pooledObjects[i][0];
        //            pooledObjects[i].RemoveAt(0);
        //            pooledObject.transform.parent = null;
        //            pooledObject.SetActive(true);
        //            return pooledObject;
        //        }
        //        else if (!onlyPooled)
        //        {
        //            Debug.LogError("Out of Pooled objects - instantiating new object");
        //            return Instantiate(objectPrefabs[i]) as GameObject;
        //        }
        //        break;
        //    }
        //}

#if !NO_DEBUG
        Debug.LogError("Object not found or out of pooled objects");
#endif
        return null; //object was not found
    }

    public void PoolObject(GameObject obj)
    {
        //for (int i = 0; i < objectPrefabs.Length; i++)
        //{
        //    if (objectPrefabs[i].name == obj.name)
        //    {
        //        obj.SetActive(false);
        //        obj.transform.position = container.position;
        //        obj.transform.parent = container;
        //        pooledObjects[i].Add(obj);
        //        return;
        //    }
        //}
    }
    #endregion //Public Methods

    #region Private
    private void Start()
    {

#if UNITY_EDITOR

        for (int i = 0; i < prefabList.Count; i++)
        {
            int objNameCount = prefabList.Where(p => p.gameObject.name == prefabList[i].gameObject.name).Count();
            if(objNameCount > 1)
            {
                Debug.LogError("Duplicate prefab names found", prefabList[i].gameObject);
                return;
            }
        }
#endif
        
        container = new GameObject("ObjectPool").transform;
        pooledObjectTable = new Dictionary<GameObject, Queue<GameObject>>();

        //create a list for each object prefab
        for (int i = 0; i < prefabList.Count; i++)
        {
            pooledObjectTable.Add(prefabList[i].gameObject, new Queue<GameObject>());

            for (int j = 0; j < prefabList[i].amountToBuffer; j++)
            {
                GameObject newObj = Instantiate(prefabList[i].gameObject) as GameObject;
                newObj.name = prefabList[i].gameObject.name;
                PoolObject(newObj);
            }
        }
    }
    #endregion //private methods
    #endregion //Methods
}

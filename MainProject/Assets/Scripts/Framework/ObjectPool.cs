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
    private Queue<GameObject> requiredPool;
    private GameObject requiredObject;
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
    public GameObject GetPooledObject(GameObject prefab, bool onlyPooled)
    {
        //if a pool exists for the prefab
        if (pooledObjectTable.TryGetValue(prefab, out requiredPool))
        {
            //out of pooled objects
            if (requiredPool.Count <= 0)
            {
                //cannot spawn more on demand
                if (onlyPooled)
                {
#if !NO_DEBUG
                    Debug.LogError("Out of pooled objects");
#endif
                    return null;
                }
                else //can spawn more objects if out
                {
#if FULL_DEBUG || LOW_DEBUG
                    Debug.LogWarning("Out of Pooled objects - instantiating new object and adding to pool");
#endif
                    InstantiateAndPoolObject(prefab);
                }
            }
            requiredObject = requiredPool.Dequeue();
            requiredObject.transform.SetParent(null);
            requiredObject.SetActive(true);
            return requiredObject;
        }
        else //the prefab was not found in the pool
        {
#if !NO_DEBUG
            Debug.LogError("Object not found or out of pooled objects");
#endif
            return null; //object was not found
        }
    }

    public void PoolObject(GameObject prefab, GameObject spawnedObject)
    {
        if (pooledObjectTable.TryGetValue(prefab, out requiredPool))
        {
            spawnedObject.SetActive(false);
            spawnedObject.transform.SetParent(container);
            spawnedObject.transform.position = Vector3.zero;
            requiredPool.Enqueue(spawnedObject);
        }
        else
        {
#if FULL_DEBUG
            Debug.LogWarning("Object to pool not found in pool - adding new entry");
#endif
            pooledObjectTable.Add(prefab, new Queue<GameObject>());
            pooledObjectTable[prefab].Enqueue(spawnedObject);
        }
    }
    #endregion //Public Methods

    #region Private
    private void Start()
    {

#if LOW_DEBUG || FULL_DEBUG

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
                InstantiateAndPoolObject(prefabList[i].gameObject);
            }
        }
    }
    private void InstantiateAndPoolObject(GameObject prefab)
    {
        GameObject newObj = Instantiate(prefab) as GameObject;
        newObj.name = prefab.name;
        PoolObject(prefab, newObj);
    }
    
    #endregion //private methods
    #endregion //Methods
}

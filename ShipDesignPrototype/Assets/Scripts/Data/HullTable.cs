using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

[Serializable]
public class HullTableEntry
{
    public int ID;
    public GameObject hullPrefab;
    public HullTableEntry(int _ID, GameObject _hullPrefab)
    {
        ID = _ID;
        hullPrefab = _hullPrefab;
    }
}

[Serializable]
public class HullTable : ScriptableObject 
{
    public int ID;
    public GameObject hullPrefab;

    //[SerializeField]
    //List<int> IDs;
    //[SerializeField]
    //List<GameObject> hullPrefabs;

    //Dictionary<int, GameObject> hullTable;
    [SerializeField]
    List<HullTableEntry> hullTable;

    public List<HullTableEntry> HullTable1
    {
        get { return hullTable; }
    }

    void Start()
    {
        //hullTable = new Dictionary<int, GameObject>();
        Debug.Log("new list");
        hullTable = new List<HullTableEntry>();
    }

    public void AddEntry(int _ID, GameObject _hullPrefab)
    {
        if(hullTable==null)
        {
            hullTable = new List<HullTableEntry>();
        }
        hullTable.Add(new HullTableEntry(_ID, _hullPrefab));
        //IDs.Add(_ID);
        //hullPrefabs.Add(_hullPrefab);
    }
    public bool IDExists(int _id)
    {
        if(hullTable==null)
        {
            return false;
        }
        //return hullTable.Contains(_id);
        return hullTable.Any(entry => entry.ID == _id);
    }
    public bool PrefabExists(GameObject _hullPrefab)
    {
        if (hullTable == null)
        {
            return false;
        }
        //return hullTable.ContainsValue(_hullPrefab);
        return hullTable.Any(entry => entry.hullPrefab == _hullPrefab);
    }
    public void WipeTable()
    {
        if (hullTable != null)
        {
            hullTable.Clear();
            
        }
    }
    public void DisplayTable()
    {
        if (hullTable != null)
        {
            Debug.Log(hullTable);
        }
    }
}

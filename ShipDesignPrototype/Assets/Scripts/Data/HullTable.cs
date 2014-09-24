using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

[Serializable]
public class HullTableEntry
{
    public int ID;
    public Hull hull;
    public HullTableEntry(int _ID, Hull _hull)
    {
        ID = _ID;
        hull = _hull;
    }
}

[Serializable]
public class HullTable : ScriptableObject 
{
    //public int ID;
    //public GameObject hullPrefab;

    //Dictionary<int, GameObject> hullTable;
    [SerializeField]
    List<HullTableEntry> hullTable;

    public List<HullTableEntry> HullTable1
    {
        get { return hullTable; }
    }

    public void AddEntry(int _ID, Hull _hull)
    {
        if(hullTable==null)
        {
            hullTable = new List<HullTableEntry>();
        }
        hullTable.Add(new HullTableEntry(_ID, _hull));
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
    public bool HullExists(Hull _hull)
    {
        if (hullTable == null)
        {
            return false;
        }
        //return hullTable.ContainsValue(_hullPrefab);
        return hullTable.Any(entry => entry.hull == _hull);
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

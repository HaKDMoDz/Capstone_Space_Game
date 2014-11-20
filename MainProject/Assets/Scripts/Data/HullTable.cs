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

public class HullTable : ScriptableObject 
{
    //public int ID;
    //public GameObject hullPrefab;

    //Dictionary<int, GameObject> hullTable;
    [SerializeField]
    List<HullTableEntry> hullTable;

    public List<HullTableEntry> HullTableProp
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
        _hull.ID = _ID;
    }

    public void AutoGenIDAndAdd(Hull hull)
    {
        AddEntry(GenNextID(), hull);
    }
    public int GenNextID()
    {
        int genID = 0;
        if(hullTable==null)
        {
            hullTable = new List<HullTableEntry>();
        }
        while (hullTable.Any(entry => entry.ID == genID))
        {
            genID++;
        }
        return genID;
    }
    public bool IDExists(int id)
    {
        if(hullTable==null)
        {
            return false;
        }
        //return hullTable.Contains(_id);
        return hullTable.Any(entry => entry.ID == id);
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
    
}

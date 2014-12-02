using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

#region AdditionalData
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
#endregion //Additional Data

public class HullTable : ScriptableObject
{
    #region Fields
    #region EditorExposed
    [SerializeField]
    List<HullTableEntry> hullTableEntryList;
    public List<HullTableEntry> HullTableProp
    {
        get { return hullTableEntryList; }
    }
    #endregion //EditorExposed
    #endregion //Fields

    #region Methods

    #region GUI
    public void AddEntry(int _ID, Hull _hull)
    {
        if(hullTableEntryList==null)
        {
            hullTableEntryList = new List<HullTableEntry>();
        }
        hullTableEntryList.Add(new HullTableEntry(_ID, _hull));
        _hull.ID = _ID;
    }

    public void AutoGenIDAndAdd(Hull hull)
    {
        AddEntry(GenNextID(), hull);
    }
    public int GenNextID()
    {
        int genID = 0;
        if(hullTableEntryList==null)
        {
            hullTableEntryList = new List<HullTableEntry>();
        }
        while (hullTableEntryList.Any(entry => entry.ID == genID))
        {
            genID++;
        }
        return genID;
    }
    public bool IDExists(int id)
    {
        if(hullTableEntryList==null)
        {
            return false;
        }
        return hullTableEntryList.Any(entry => entry.ID == id);
    }
    public bool HullExists(Hull _hull)
    {
        if (hullTableEntryList == null)
        {
            return false;
        }
        return hullTableEntryList.Any(entry => entry.hull == _hull);
    }
    public void WipeTable()
    {
        if (hullTableEntryList != null)
        {
            hullTableEntryList.Clear();
        }
    }
    #endregion //GUI
    #endregion //Methods

}

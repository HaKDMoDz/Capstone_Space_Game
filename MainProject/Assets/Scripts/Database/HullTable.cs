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
    public List<HullTableEntry> HullTableProp { get; private set; }
    
    #endregion //EditorExposed
    #endregion //Fields

    #region Methods

    #region GUI
    public void AddEntry(int _ID, Hull _hull)
    {
        if(HullTableProp==null)
        {
            HullTableProp = new List<HullTableEntry>();
        }
        HullTableProp.Add(new HullTableEntry(_ID, _hull));
        _hull.ID = _ID;
    }

    public void AutoGenIDAndAdd(Hull hull)
    {
        AddEntry(GenNextID(), hull);
    }
    public int GenNextID()
    {
        int genID = 0;
        if(HullTableProp==null)
        {
            HullTableProp = new List<HullTableEntry>();
        }
        while (HullTableProp.Any(entry => entry.ID == genID))
        {
            genID++;
        }
        return genID;
    }
    public bool IDExists(int id)
    {
        if(HullTableProp==null)
        {
            return false;
        }
        return HullTableProp.Any(entry => entry.ID == id);
    }
    public bool HullExists(Hull _hull)
    {
        if (HullTableProp == null)
        {
            return false;
        }
        return HullTableProp.Any(entry => entry.hull == _hull);
    }
    public void WipeTable()
    {
        if (HullTableProp != null)
        {
            HullTableProp.Clear();
        }
    }
    #endregion //GUI
    #endregion //Methods

}

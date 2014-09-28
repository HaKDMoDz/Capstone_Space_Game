using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

[Serializable]
public class CompTableEntry
{
    public int ID;
    public ShipComponent component;
    public CompTableEntry(int _ID, ShipComponent _component)
    {
        ID = _ID;
        component = _component;
    }
}

[Serializable]
public class ComponentTable : ScriptableObject 
{
    [SerializeField]
    List<CompTableEntry> compTable;
    public List<CompTableEntry> CompTable
    {
        get { return compTable; }
    }

    public void AutoGenIDAndAdd(ShipComponent component)
    {
        AddEntry(GenNextID(), component);
    }

    public int GenNextID()
    {
        int genID = 0;

        while (compTable.Any(entry => entry.ID == genID))
        {
            genID++;
        }
        return genID;
    }

    public void AddEntry(int ID, ShipComponent component)
    {
        if(compTable==null)
        {
            compTable = new List<CompTableEntry>();
        }
        compTable.Add(new CompTableEntry(ID, component));
    }

    public bool IDExists(int id)
    {
        if(compTable==null)
        {
            return false;
        }
        return compTable.Any(entry=>entry.ID==id);
    }
    public bool ComponentExists(ShipComponent _component)
    {
        if(compTable==null)
        {
            return false;
        }
        return compTable.Any(entry => entry.component = _component);
    }
    public void WipeTable()
    {
        if(compTable!=null)
        {
            compTable.Clear();
        }
    }
}

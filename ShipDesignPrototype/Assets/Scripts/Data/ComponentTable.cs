using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

[Serializable]
public class ComponentTableEntry
{
    public int ID;
    public ShipComponent component;
    public ComponentTableEntry(int _ID, ShipComponent _component)
    {
        ID = _ID;
        component = _component;
    }
}

[Serializable]
public class ComponentTable : ScriptableObject 
{
    [SerializeField]
    List<ComponentTableEntry> componentList;
    public List<ComponentTableEntry> ComponentList
    {
        get { return componentList; }
    }


    public void AddEntry(int ID, ShipComponent component)
    {
        if(componentList == null)
        {
            componentList = new List<ComponentTableEntry>();
        }
        componentList.Add((new ComponentTableEntry(ID, component)));
    }
    public void AutoGenIDandAdd(ShipComponent comp)
    {
        AddEntry(GenID(), comp);
    }
    public int GenID()
    {
        int genID = 0;
        if(componentList == null)
        {
            componentList = new List<ComponentTableEntry>();
        }
        while(componentList.Any(entry=>entry.ID == genID))
        {
            genID++;
        }
        return genID;
    }
    public bool IDExists(int id)
    {
        if(componentList==null)
        {
            return false;
        }
        return componentList.Any(entry => entry.ID == id);
    }
    public bool ComponentExists(ShipComponent comp)
    {
        if(componentList==null)
        {
            return false;
        }
        return componentList.Any(entry => entry.component == comp);
    }
    public void WipeTable()
    {
        if(componentList!=null)
        {
            componentList.Clear();
        }
    }

}

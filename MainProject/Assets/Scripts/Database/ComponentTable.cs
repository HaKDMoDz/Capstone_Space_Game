using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

#region AdditionalData
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
#endregion//Additional Data

public class ComponentTable : ScriptableObject
{
    #region Fields
    #region EditorExposed
    [SerializeField]
    List<ComponentTableEntry> componentList;
    public List<ComponentTableEntry> ComponentList
    {
        get { return componentList; }
    }
    #endregion //EditorExposed
    #endregion //Fields


    #region Methods
    #region GUI
    public void AddEntry(int ID, ShipComponent component)
    {
        if(componentList == null)
        {
            componentList = new List<ComponentTableEntry>();
        }
        component.ID = ID;
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
    #endregion //GUI
    #endregion //Methods
}

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
    public List<ComponentTableEntry> ComponentList { get; private set; }
    #endregion //EditorExposed
    #endregion //Fields


    #region Methods
    #region GUI
    public void AddEntry(int ID, ShipComponent component)
    {
        if(ComponentList == null)
        {
            ComponentList = new List<ComponentTableEntry>();
        }
        component.ID = ID;
        ComponentList.Add((new ComponentTableEntry(ID, component)));
        
    }
    public void AutoGenIDandAdd(ShipComponent comp)
    {
        AddEntry(GenID(), comp);
    }
    public int GenID()
    {
        int genID = 0;
        if(ComponentList == null)
        {
            ComponentList = new List<ComponentTableEntry>();
        }
        while(ComponentList.Any(entry=>entry.ID == genID))
        {
            genID++;
        }
        return genID;
    }
    public bool IDExists(int id)
    {
        if(ComponentList==null)
        {
            return false;
        }
        return ComponentList.Any(entry => entry.ID == id);
    }
    public bool ComponentExists(ShipComponent comp)
    {
        if(ComponentList==null)
        {
            return false;
        }
        return ComponentList.Any(entry => entry.component == comp);
    }
    public void WipeTable()
    {
        if(ComponentList!=null)
        {
            ComponentList.Clear();
        }
    }
    #endregion //GUI
    #endregion //Methods
}

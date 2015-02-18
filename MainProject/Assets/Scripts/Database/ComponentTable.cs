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
    private List<ComponentTableEntry> comp_id_List;
    public List<ComponentTableEntry> Comp_id_List
    {
        get { return comp_id_List; }
    }
    #endregion EditorExposed
    //Database Access
    public static Dictionary<int, ShipComponent> id_comp_table { get; private set; }
    public static Dictionary<ShipComponent, int> comp_id_table { get; private set; }
    #endregion Fields


    #region Methods
    #region Public 
    public void Init()
    {
        id_comp_table = comp_id_List.ToDictionary(c => c.ID, c => c.component);
        comp_id_table = comp_id_List.ToDictionary(c => c.component, c => c.ID);
    }
    #region DatabaseAccess
    public static ShipComponent GetComp(int compID)
    {
#if FULL_DEBUG
        ShipComponent comp = null;
        if(!id_comp_table.TryGetValue(compID, out comp))
        {
            Debug.LogError("Component with ID " + compID + " not found");
        }
        return comp;
#else
        return id_comp_table[compID];
#endif
    }
    public static int GetID(ShipComponent component)
    {
#if FULL_DEBUG
        int compID;
        if(!comp_id_table.TryGetValue(component, out compID))
        {
            Debug.LogError("Component " + component.componentName + " not found");
        }
        return compID;
#else
        return comp_id_table[component];
#endif
    }

    #endregion DatabaseAccess

    #region GUI_Access
#if UNITY_EDITOR
    
    public void AddEntry(int ID, ShipComponent component)
    {
        if(Comp_id_List == null)
        {
            comp_id_List = new List<ComponentTableEntry>();
        }
        Comp_id_List.Add((new ComponentTableEntry(ID, component)));
        
    }
    public void AutoGenIDandAdd(ShipComponent comp)
    {
        AddEntry(GenID(), comp);
    }
    public void RemoveEntry(int _ID)
    {
        Debug.Log("Removing ID " + _ID);
        ComponentTableEntry toDelete = comp_id_List.Find(c => c.ID == _ID);
        if(toDelete == null)
        {
            Debug.LogError("Component to delete not found");
            return;
        }
        comp_id_List.Remove(toDelete);

    }
    public int GenID()
    {
        int genID = 0;
        if(Comp_id_List == null)
        {
            comp_id_List = new List<ComponentTableEntry>();
        }
        while(Comp_id_List.Any(entry=>entry.ID == genID))
        {
            genID++;
        }
        return genID;
    }
    public bool IDExists(int id)
    {
        if(Comp_id_List==null)
        {
            return false;
        }
        return Comp_id_List.Any(entry => entry.ID == id);
    }
    public bool ComponentExists(ShipComponent comp)
    {
        if(Comp_id_List==null)
        {
            return false;
        }
        return Comp_id_List.Any(entry => entry.component == comp);
    }
    public void WipeTable()
    {
        if(Comp_id_List!=null)
        {
            Comp_id_List.Clear();
        }
    }
#endif

    #endregion GUI_Access
    #endregion Public
    #region UnityCallbacks
    private void OnEnable()
    {
        Init();
        //id_comp_table = comp_id_List.ToDictionary(c => c.ID, c => c.component);
        //comp_id_table = comp_id_List.ToDictionary(c => c.component, c => c.ID);
    }
    #endregion UnityCallbacks
    #endregion Methods
}

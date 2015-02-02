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
        //if (id_comp_table == null)
        //{
        //    Debug.Log("force init");
        //    FindObjectOfType<ComponentTable>().OnEnable();
        //}
        return id_comp_table[compID];
    }
    public static int GetID(ShipComponent component)
    {
        //if (comp_id_table == null)
        //{
        //    Debug.Log("force init");
        //    FindObjectOfType<ComponentTable>().OnEnable();
        //}
        return comp_id_table[component];
    }
    #endregion DatabaseAccess
    #region GUI_Access
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

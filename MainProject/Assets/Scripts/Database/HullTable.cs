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
    private List<HullTableEntry> hull_id_List;
    public List<HullTableEntry> Hull_id_List
    {
        get { return hull_id_List; }
    }

    #endregion EditorExposed

    //Database Reference
    public static Dictionary<int, Hull> id_hull_table { get; private set; }
    //public static Dictionary<int, Hull> ID_Hull_Table
    //{
    //    get
    //    {
    //        if (id_hull_table == null)
    //        {
    //            Debug.Log("force init");
    //            FindObjectOfType<HullTable>().OnEnable();
    //        }
    //        return id_hull_table;
    //    }
    //}

    public static Dictionary<Hull, int> hull_id_table { get; private set; }
    //public static Dictionary<Hull, int> Hull_ID_Table
    //{
    //    get
    //    {
    //        if (hull_id_table == null)
    //        {
    //            Debug.Log("force init");
    //            FindObjectOfType<HullTable>().OnEnable();
    //        }
    //        return hull_id_table;
    //    }
    //}

    #endregion Fields

    #region Methods
    #region Public
    public void Init()
    {
        id_hull_table = hull_id_List.ToDictionary(h => h.ID, h => h.hull);
        hull_id_table = hull_id_List.ToDictionary(h => h.hull, h => h.ID);
    }
    #region DatabaseAccess
    public static Hull GetHull(int hull_ID)
    {
        //if (id_hull_table == null)
        //{
        //    Debug.Log("force init");
        //    FindObjectOfType<HullTable>().OnEnable();
        //}
        return id_hull_table[hull_ID];
    }
    public static int GetID(Hull hull)
    {
        //if (hull_id_table == null)
        //{
        //    Debug.Log("force init");
        //    FindObjectOfType<HullTable>().OnEnable();
        //}
        return hull_id_table[hull];
    }
    #endregion DatabaseAccess
    #region GUI_Access
    public void AddEntry(int _ID, Hull _hull)
    {
        if (Hull_id_List == null)
        {
            hull_id_List = new List<HullTableEntry>();
        }
        Hull_id_List.Add(new HullTableEntry(_ID, _hull));
        _hull.ID = _ID;
    }

    public void AutoGenIDAndAdd(Hull hull)
    {
        AddEntry(GenNextID(), hull);
    }
    public int GenNextID()
    {
        int genID = 0;
        if (Hull_id_List == null)
        {
            hull_id_List = new List<HullTableEntry>();
        }
        while (Hull_id_List.Any(entry => entry.ID == genID))
        {
            genID++;
        }
        return genID;
    }
    public bool IDExists(int id)
    {
        if (Hull_id_List == null)
        {
            return false;
        }
        return Hull_id_List.Any(entry => entry.ID == id);
    }
    public bool HullExists(Hull _hull)
    {
        if (Hull_id_List == null)
        {
            return false;
        }
        return Hull_id_List.Any(entry => entry.hull == _hull);
    }
    public void WipeTable()
    {
        if (Hull_id_List != null)
        {
            Hull_id_List.Clear();
        }
    }
    #endregion GUI_Access
    #endregion Public
    #region Private
    #region UnityCallbacks
    //private void OnEnable()
    //{
    //    Debug.Log("hull table enable");
    //    id_hull_table = hull_id_List.ToDictionary(h => h.ID, h => h.hull);
    //    hull_id_table = hull_id_List.ToDictionary(h => h.hull, h => h.ID);

    #endregion UnityCallbacks
    #endregion Private
    #endregion Methods

}

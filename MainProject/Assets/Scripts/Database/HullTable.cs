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
    public static Dictionary<Hull, int> hull_id_table { get; private set; }

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
#if FULL_DEBUG
        Hull hull = null;
        if(!id_hull_table.TryGetValue(hull_ID, out hull))
        {
            Debug.LogError("Hull with ID " + hull_ID + " not found");
        }
        return hull;
#else
        return id_hull_table[hull_ID];
#endif
    }
    public static int GetID(Hull hull)
    {
#if FULL_DEBUG
        int hull_ID;
        if (!hull_id_table.TryGetValue(hull, out hull_ID))
        {
            Debug.LogError("Hull " +hull.hullName + " not found");
        }
        return hull_ID;
#else
        return hull_id_table[hull];
#endif
    }
    #endregion DatabaseAccess
    #region GUI_Access
#if UNITY_EDITOR
    public void AddEntry(int _ID, Hull _hull)
    {
        if (Hull_id_List == null)
        {
            hull_id_List = new List<HullTableEntry>();
        }
        Hull_id_List.Add(new HullTableEntry(_ID, _hull));
    }

    public void AutoGenIDAndAdd(Hull hull)
    {
        AddEntry(GenNextID(), hull);
    }

    public void RemoveEntry(int _ID)
    {
        Debug.Log("Remove ID" + _ID);
        HullTableEntry toDelete = Hull_id_List.Find(h => h.ID == _ID);
        if(toDelete == null)
        {
            Debug.LogError("Hull to delete not found");
            return;
        }
        hull_id_List.Remove(toDelete);
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
#endif

    #endregion GUI_Access
    #endregion Public
    #region UnityCallbacks
    private void OnEnable()
    {
        Init();
    }

    #endregion UnityCallbacks
    #endregion Methods

}

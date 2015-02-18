using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Hull : MonoBehaviour
{

    #region Fields
    
    #region EditorExposed
    public string hullName;
    [SerializeField]
    private float hullHP;
    public float HullHP
    {
        get { return hullHP; }
    }
    [SerializeField]
    private List<ComponentSlot> emptyComponentGrid;
    public List<ComponentSlot> EmptyComponentGrid
    {
        get { return emptyComponentGrid; }
    }
    #endregion EditorExposed

    #region Internal
    
    public Dictionary<int, ComponentSlot> index_slot_table { get; private set; }
    public bool unlocked { get; private set; }

    #endregion Internal

    #endregion Fields

    #region Methods
    
    /// <summary>
    /// Should be called AFTER INSTANTIATION of the ship from a prefab. Goes through all the component slots installed on the ship and puts it into the index_slot_table dictionary
    /// </summary>
    public void Init()
    {
        //Debug.Log("Hull Init");
        index_slot_table = new Dictionary<int, ComponentSlot>();

        for (int i = 0; i < EmptyComponentGrid.Count; i++)
        {
            //Debug.Log("added a component");
            index_slot_table.Add(emptyComponentGrid[i].index, EmptyComponentGrid[i]);
        }
    }

    #if FULL_DEBUG
    public void OutputSlotTable()
    {
        Debug.Log("Slot Table");
        foreach (var item in index_slot_table)
        {
            Debug.Log("index: " + item.Key + " slot: " + item.Value.index);
        }
    }
    #endif

    #endregion Methods



}

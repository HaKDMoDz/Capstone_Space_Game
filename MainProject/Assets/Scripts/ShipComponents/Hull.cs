using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Hull : MonoBehaviour
{

    #region Fields
    
    #region EditorExposed
    public int ID;
    public string hullName;
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
    
    public void Init()
    {
        //emptyComponentGrid = new List<ComponentSlot>(GetComponentsInChildren<ComponentSlot>());
        //Debug.Log("Hull Init");
        index_slot_table = new Dictionary<int, ComponentSlot>();

        for (int i = 0; i < EmptyComponentGrid.Count; i++)
        {
            //emptyComponentGrid[i].index = i;
            index_slot_table.Add(EmptyComponentGrid[i].index, EmptyComponentGrid[i]);
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

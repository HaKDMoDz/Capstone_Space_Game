using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Hull : MonoBehaviour
{

    #region Fields
    
    #region EditorExposed
    [SerializeField]
    List<ComponentSlot> emptyComponentGrid;
    public List<ComponentSlot> EmptyComponentGrid
    {
        get { return emptyComponentGrid; }
    }
    public int ID;
    #endregion EditorExposed

    #region PublicFields
    private Dictionary<int, ComponentSlot> slotTable;
    public Dictionary<int, ComponentSlot> SlotTable
    {
        get { return slotTable; }
    }
    
    bool unlocked;
    public bool Unlocked
    {
        get { return unlocked; }
        set { unlocked = value; }
    }
    #endregion PublicFields

    #endregion Fields

    #region Methods
    #region Public
    public void Init()
    {
        //emptyComponentGrid = new List<ComponentSlot>(GetComponentsInChildren<ComponentSlot>());
        //Debug.Log("Hull Init");
        slotTable = new Dictionary<int, ComponentSlot>();

        for (int i = 0; i < emptyComponentGrid.Count; i++)
        {
            //emptyComponentGrid[i].index = i;
            slotTable.Add(emptyComponentGrid[i].index, emptyComponentGrid[i]);
        }
    }
    public void OutputSlotTable()
    {
        Debug.Log("Slot Table");
        foreach (var item in slotTable)
        {
            Debug.Log("index: " + item.Key + " slot: " + item.Value.index);
        }
    }
    #endregion Public

    #region Private

    #endregion Private

    #endregion Methods

    

}

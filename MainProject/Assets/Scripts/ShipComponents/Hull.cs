using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Hull : MonoBehaviour
{

    #region Fields
    
    #region EditorExposed
    [SerializeField]
    public List<ComponentSlot> EmptyComponentGrid { get; private set; }
    public int ID;
    #endregion EditorExposed

    #region Internal
    
    public Dictionary<int, ComponentSlot> SlotTable { get; private set; }
    public bool unlocked { get; private set; }

    #endregion Internal

    #endregion Fields

    #region Methods
    #region Public
    public void Init()
    {
        //emptyComponentGrid = new List<ComponentSlot>(GetComponentsInChildren<ComponentSlot>());
        //Debug.Log("Hull Init");
        SlotTable = new Dictionary<int, ComponentSlot>();

        for (int i = 0; i < EmptyComponentGrid.Count; i++)
        {
            //emptyComponentGrid[i].index = i;
            SlotTable.Add(EmptyComponentGrid[i].index, EmptyComponentGrid[i]);
        }
    }
    public void OutputSlotTable()
    {
        Debug.Log("Slot Table");
        foreach (var item in SlotTable)
        {
            Debug.Log("index: " + item.Key + " slot: " + item.Value.index);
        }
    }
    #endregion Public

    #region Private

    #endregion Private

    #endregion Methods

    

}

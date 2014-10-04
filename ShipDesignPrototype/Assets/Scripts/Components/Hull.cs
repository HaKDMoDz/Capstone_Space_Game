using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Hull : MonoBehaviour 
{
    public int ID;
    
    List<ComponentSlot> emptyComponentGrid;
    public List<ComponentSlot> EmptyComponentGrid
    {
        get { return emptyComponentGrid; }
    }

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


    public GameObject hullPrefab;

    public void Init()
    {
        emptyComponentGrid = new List<ComponentSlot>(GetComponentsInChildren<ComponentSlot>());
        slotTable = new Dictionary<int, ComponentSlot>();

        for (int i = 0; i < emptyComponentGrid.Count; i++)
        {
            emptyComponentGrid[i].index = i;
            slotTable.Add(i, emptyComponentGrid[i]);
        }
    }

}

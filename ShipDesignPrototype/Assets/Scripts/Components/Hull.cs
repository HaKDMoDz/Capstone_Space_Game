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
        for (int i = 0; i < emptyComponentGrid.Count; i++)
        {
            emptyComponentGrid[i].index = i;
        }
    }

}

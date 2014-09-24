using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Hull : MonoBehaviour 
{
    public uint ID;
    public List<ComponentSlot> emptyComponentGrid;
    public bool unlocked;


    public GameObject hullPrefab;

    public void Init()
    {
        emptyComponentGrid = new List<ComponentSlot>(GetComponentsInChildren<ComponentSlot>());
        //if(hullPrefab)
        //{
        //    emptyComponentGrid = new List<ComponentSlot>(GetComponentsInChildren<ComponentSlot>());
        //}
        //else
        //{
        //    Debug.LogError("no hullPrefab assigned", this);
        //}
    }

}

/*
  ComponentSlot.cs
  Mission: Invasion
  Created by Rohun Banerji on Nov 12/2014
  Copyright (c) 2014 Rohun Banerji. All rights reserved.
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ComponentSlot : MonoBehaviour
{
    #region Fields

    public int index; //assigned in the prefab in advance when the component grid is created for a given ship model
    //[SerializeField]
    //private AI_Fleet.PlacementType placement;
    //public AI_Fleet.PlacementType Placement
    //{
    //    get { return placement; }
    //    set { placement = value; }
    //}
    [SerializeField]
    private ShipComponent installedComponent = null;
    public ShipComponent InstalledComponent
    {
        get { return installedComponent; }
        set 
        {
            installedComponent = value;
            //Debug.LogError("Set installed component " + value);
        }
    }

    public void Init()
    {
        InstalledComponent = null;
    }
    #endregion Fields

}

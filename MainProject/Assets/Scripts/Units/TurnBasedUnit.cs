﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class TurnBasedUnit : MonoBehaviour
{
    #region Fields
    [SerializeField]
    private float turnDelay;
    public float TurnDelay
    {
        get { return turnDelay; }
        set { turnDelay = value; }
    }

    //references
    public ShipBlueprintMetaData shipBPMetaData { get; private set; }
    public ShipMove shipMove { get; private set; }
    protected ShipBlueprint shipBP;


    #endregion Fields

    #region Methods
    #region PublicMethods
    
    public virtual void Init(ShipBlueprint shipBP, ShipMove shipMove)
    {
        #if FULL_DEBUG
        Debug.Log("TurnBasedUnit Init");
        #endif
        this.shipBP = shipBP;
        this.shipBPMetaData = shipBP.metaData;
        this.shipMove = shipMove;
    }

    public IEnumerator ExecuteTurn()
    {
        #if FULL_DEBUG
        Debug.Log(gameObject.name + "executing turn");
        #endif

        yield return null;
    }
    #endregion PublicMethods
    #endregion Methods
}

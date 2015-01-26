using UnityEngine;
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
        set
        {
            turnDelay = value;
            TimeLeftToTurn = turnDelay;
        }
    }
    private float timeLeftToTurn;
    public float TimeLeftToTurn
    {
        get { return timeLeftToTurn; }
        set
        {
            if (value <= 0.0f)
            {
                timeLeftToTurn = turnDelay;
            }
            else
            {
                timeLeftToTurn = value;
            }
        }
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
        this.shipBP = shipBP;
        this.shipBPMetaData = shipBP.metaData;
        this.shipMove = shipMove;
        this.shipMove.Init();
        timeLeftToTurn = turnDelay;
    }

    public virtual IEnumerator ExecuteTurn()
    {
        #if FULL_DEBUG
        Debug.Log(shipBPMetaData.blueprintName + " executing turn");
        #endif

        yield return null;
    }
    #endregion PublicMethods

    #region InternalCallbacks
    
    #endregion InternalCallbacks

    #endregion Methods
}

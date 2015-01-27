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

    protected Camera componentCamera;

    //TEMP
    private float hullHP = 100;

    //TEMP
    /// <summary>
    /// Coroutine for a ship taking Damage. Accounts for 0 or less hull HP
    /// Will need to be replaced once component system is active
    /// </summary>
    /// <param name="_amountOfDamage">The amount of Damage done to the ship</param>
    /// <returns>null or the Destroy() Coroutine</returns>
    public IEnumerator TakeDamage(float _amountOfDamage)
    {
        hullHP -= _amountOfDamage;
        if (hullHP <= 0)
        {
            yield return StartCoroutine(Destroy());
        }
    }

    /// <summary>
    /// Coroutine for Destroying ships when HP is 0 (or less)
    /// </summary>
    /// <returns>null after it finishes</returns>
    private IEnumerator Destroy()
    {
        //play explosion particle effect
        //play explosion sound
        //play explosion juice (screen shake, etc)
        //remove ship
        Debug.Log("Ship Destroyed");
        yield return null;
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
        componentCamera = GetComponentInChildren<Camera>();
        #if FULL_DEBUG
        if(componentCamera==null)
        {
            Debug.LogError("No Component camera found");
        }
	    #endif
        componentCamera.enabled = false;
    }



    public virtual IEnumerator ExecuteTurn()
    {
        #if FULL_DEBUG
        Debug.Log(shipBPMetaData.blueprintName + " executing turn");
        #endif

        yield return null;
    }

    public void ShowComponentSelection(bool show)
    {
        componentCamera.enabled = show;
    }
    #endregion PublicMethods

    #region InternalCallbacks
    
    #endregion InternalCallbacks

    #endregion Methods
}

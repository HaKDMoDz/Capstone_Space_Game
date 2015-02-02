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

    private float maxHullHP;
    public float MaxHullHP
    {
        get { return maxHullHP; }
    }

    //references
    [SerializeField]
    private GameObject componentCamera;
    protected GameObject ComponentCamera
    {
        get { return componentCamera; }
    }
    [SerializeField]
    protected GameObject expolosionObject;
    public GameObject getExplosionObject()
    {
        return expolosionObject;
    }

    //TEMP
    private float hullHP;
    public float HullHP { get { return hullHP; } private set { hullHP = value; } }

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
#if FULL_DEBUG
        Debug.Log("damage taken. remaining HP: " + hullHP);
#endif
        if (hullHP <= 0)
        {
            yield return StartCoroutine(Destroy());
        }
    }

    /// <summary>
    /// Coroutine for Destroying ships when HP is 0 (or less)
    /// </summary>
    /// <returns>null after it finishes</returns>
    protected virtual IEnumerator Destroy()
    {
        //play explosion particle effect
        expolosionObject.SetActive(true);

        //play explosion sound

        //play explosion juice (screen shake, etc)
        yield return new WaitForSeconds(1.75f);
        Camera.main.GetComponent<CameraDirector>().DoShake();
        
        //wait for explosion to finish
        yield return new WaitForSeconds(1.0f);
        
        //remove ship
        TurnBasedCombatSystem.Instance.KillShip(this);
        
        yield return new WaitForSeconds(1.0f);

        #if FULL_DEBUG
        Debug.Log("Ship Destroyed");
        #endif  
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
        
        componentCamera = GetComponentInChildren<Camera>().gameObject;
        componentCamera.SetActive(false);
        expolosionObject = transform.FindChild("Explosion").gameObject;
       
        maxHullHP = shipBP.hull.HullHP;
        hullHP = maxHullHP;
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
        componentCamera.SetActive(show);
    }
    #endregion PublicMethods

    #region InternalCallbacks
    
    #endregion InternalCallbacks

    #endregion Methods
}

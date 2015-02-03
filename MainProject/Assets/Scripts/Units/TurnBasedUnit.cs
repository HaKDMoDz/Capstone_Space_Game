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

    private float maxPower;
    public float MaxPower
    {
        get { return maxPower; }
    }

    private float currentPower;
    public float CurrentPower
    {
        get { return currentPower; }
        protected set { currentPower = value; }
    }

    protected List<ShipComponent> components = new List<ShipComponent>();
    public List<ShipComponent> Components
    {
        get { return components; }
    }

    //references
    [SerializeField]
    private GameObject componentCamera;
    protected GameObject ComponentCamera
    {
        get { return componentCamera; }
    }
    [SerializeField]
    private GameObject targetingCamera;
    protected GameObject TargetingCamera
    {
        get { return targetingCamera; }
    }

    [SerializeField]
    protected GameObject expolosionObject;
    public GameObject getExplosionObject()
    {
        return expolosionObject;
    }
    
    public ShipBlueprintMetaData shipBPMetaData { get; private set; }
    public ShipMove shipMove { get; private set; }
    protected ShipBlueprint shipBP;
    private Transform trans;

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

        foreach (ShipComponent component in shipBP.slot_component_table.Values)
        {
            component.Init();
            components.Add(component);
        }

        trans = transform;

        #if FULL_DEBUG
        if (trans.FindChild("ComponentCamera")==null)
        {
            Debug.LogError("No Component camera found");
        }
        #endif
        componentCamera = trans.FindChild("ComponentCamera").gameObject;
        componentCamera.SetActive(false);

        #if FULL_DEBUG
        if (trans.FindChild("TargetingCamera") == null)
        {
            Debug.LogError("No Targeting camera found");
        }
        #endif
        targetingCamera = trans.FindChild("TargetingCamera").gameObject;
        targetingCamera.SetActive(false);

        expolosionObject = trans.FindChild("Explosion").gameObject;
       
        maxHullHP = shipBP.hull.HullHP;
        hullHP = maxHullHP;

        maxPower = shipBPMetaData.excessPower;
        currentPower = MaxPower;
    }



    public virtual IEnumerator ExecuteTurn()
    {
        #if FULL_DEBUG
        Debug.Log(shipBPMetaData.blueprintName + " executing turn");
        #endif
        currentPower = MaxPower;

        yield return null;
    }

    public void ShowComponentSelection(bool show)
    {
        componentCamera.SetActive(show);
    }

    public void ShowTargetingPanel(bool show)
    {
        CombatSystemInterface.Instance.ShowTargetingPanel(show, name);
        targetingCamera.SetActive(show);
    }

    #endregion PublicMethods

    #region InternalCallbacks
    
    #endregion InternalCallbacks

    #endregion Methods
}

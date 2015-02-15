using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
            #if FULL_DEBUG || LOW_DEBUG
            if(value < 0.0f)
            {
                Debug.LogError("Time left to turn is being set to a negative number: " + value);
            }
            #endif
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

    public float MoveCost {get; private set;}

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
    protected Transform trans;

    //TEMP
    private float hullHP;
    public float HullHP //set hp and hp bar
    {
        get { return hullHP; }
        private set 
        {
            float damage = hullHP - value;
            hpBar.value -= damage / MaxHullHP;
            hullHP = value; 
        }
    }

    public float MaxShields { get; private set; }
    private float shieldStrength;
    public float ShieldStrength //set shield and shield bar
    {
        get { return shieldStrength; }
        private set
        {
            float damage = shieldStrength - value;
            shieldBar.value -= damage / MaxShields;
            shieldStrength = value; 
        }
    }

    [SerializeField]
    private Slider hpBar;
    private Slider shieldBar;

    #endregion Fields


    #region Methods

    #region PublicMethods

    /// <summary>
    /// Coroutine for a ship taking Damage. Shield takes damage first.
    /// </summary>
    /// <param name="_amountOfDamage">The amount of Damage done to the ship</param>
    /// <returns>null or the Destroy() Coroutine</returns>
    public IEnumerator TakeDamage(float _amountOfDamage)
    {
        if (ShieldStrength >= _amountOfDamage)
        {
            ShieldStrength -= _amountOfDamage;
        }
        else //damage bleeds over to hull
        {
            _amountOfDamage -= ShieldStrength;
            ShieldStrength = 0.0f;
            HullHP -= _amountOfDamage;
            #if FULL_DEBUG
            Debug.Log(name + " taking " + _amountOfDamage + " damage. Remaining HP: " + hullHP);
            #endif
        }
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
        Debug.Log(name+" Destroyed");
        #endif
    }
    /// <summary>
    /// sets up references
    /// </summary>
    /// <param name="shipBP"></param>
    /// <param name="shipMove"></param>
    public virtual void Init(ShipBlueprint shipBP, ShipMove shipMove)
    {
        this.shipBP = shipBP;
        this.shipBPMetaData = shipBP.MetaData;
        this.shipMove = shipMove;
        this.shipMove.Init();

        InitStats();
        InitReferences();

    }//Init


    /// <summary>
    /// Base virtual method to start the turn. Sets power to max.
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator ExecuteTurn()
    {
        #if FULL_DEBUG
        Debug.Log(shipBPMetaData.BlueprintName + " executing turn");
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
        ShowHPBars(show);
    }

    public void ShowHPBars(bool show)
    {
        hpBar.gameObject.SetActive(show);
        shieldBar.gameObject.SetActive(show);
    }

    #endregion PublicMethods


    #region PrivateMethods

    //Helper
    private void InitStats()
    {
        timeLeftToTurn = turnDelay;

        foreach (Slider slider in GetComponentsInChildren<Slider>())
        {
            if (slider.name == "HPbar")
            {
                hpBar = slider;
            }
            else if (slider.name == "ShieldBar")
            {
                shieldBar = slider;
            }
        }

        #if FULL_DEBUG
        if (!hpBar)
        {
            Debug.LogError("Could not find HPbar");
        }
        if (!shieldBar)
        {
            Debug.LogError("Could not find ShieldBar");
        }
        #endif
        ShowHPBars(false);

        maxHullHP = shipBP.Hull.HullHP;
        hullHP = maxHullHP;


        MaxShields = 0.0f;
        foreach (ShipComponent component in shipBP.Slot_component_table.Values)
        {
            component.Init(this);
            components.Add(component);
            if (component is Comp_Def_Shield)
            {
                MaxShields += ((Comp_Def_Shield)component).shieldStrength;
            }
        }
        ShieldStrength = MaxShields;

        maxPower = shipBPMetaData.ExcessPower;
        currentPower = MaxPower;

        int numThrusters = components.Count(c => c is Comp_Eng_Thruster);
        #if FULL_DEBUG || LOW_DEBUG
        if (numThrusters <= 0)
        {
            Debug.LogError("No Thrusters on Ship");
        }
        #endif
        
        float thrust = ((Comp_Eng_Thruster)components.Find(c => c is Comp_Eng_Thruster)).Thrust;
        float totalThrust = thrust * numThrusters;
        int mass = shipBP.Hull.EmptyComponentGrid.Count;
        MoveCost = mass / totalThrust * 1.5f;
    }

    private void InitReferences()
    {
        trans = transform;

        #if FULL_DEBUG
        if (trans.FindChild("ComponentCamera") == null)
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

        
    }
    #endregion PrivateMethods

    #endregion Methods
}

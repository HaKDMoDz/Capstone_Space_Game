/*
  TurnBasedUnit.cs
  Mission: Invasion
  Created by Rohun Banerji on Jan 14/2015
  Copyright (c) 2015 Rohun Banerji. All rights reserved.
*/
#region Usings
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#endregion Usings
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
        set 
        { 
            currentPower = value;
            CombatSystemInterface.Instance.UpdateStats(CurrentPower, MoveCost); 
        }
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
    public GameObject TargetingCamera
    {
        get { return targetingCamera; }
    }

    [SerializeField]
    protected GameObject expolosionObject;
    public GameObject getExplosionObject()
    {
        return expolosionObject;
    }

    private ShipBlueprintMetaData shipBPMetaData;
    public ShipBlueprintMetaData ShipBPMetaData
    {
        get { return shipBPMetaData; }
        private set { shipBPMetaData = value; }
    }

    protected ShipMove shipMove { get; private set; }
    protected ShipBlueprint shipBP;
    protected Transform trans;

    private float hullHP;
    public float HullHP //set hp and hp bar
    {
        get { return hullHP; }
        private set 
        {
            float damage = hullHP - value;
            hpBar.ChangeValue(-damage / MaxHullHP);
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
            shieldBar.ChangeValue(-damage / MaxShields);
            shieldStrength = value; 
        }
    }
    private Transform componentGridTrans;
    public Transform ComponentGridTrans
    {
        get { return componentGridTrans; }
    }

    private ShipShield shipShield;
    private FillBar hpBar;
    private FillBar shieldBar;

    private Vector3 defaultTargetCamEuler;
    private Transform targetCamTrans;

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
        //Debug.Log("Shield: " + ShieldStrength + " Damage " + _amountOfDamage);
        if (ShieldStrength >= _amountOfDamage)
        {
            ShieldStrength -= _amountOfDamage;
            //display shield damage effect
            StartCoroutine(trans.FindChild("ShieldEffect").GetComponent<DisableEffectAfterTime>().StartEffect());
        }
        else //damage bleeds over to hull
        {
            _amountOfDamage -= ShieldStrength;
            ShieldStrength = 0.0f;
            HullHP -= _amountOfDamage;
            #if FULL_DEBUG
            Debug.Log(name + " taking " + _amountOfDamage + " damage. Remaining HP: " + HullHP);
            #endif
            if (HullHP <= 0.0f)
            {
                yield return StartCoroutine(Destroy());
            }
        }
    }
    public void DestroyComponent(ShipComponent component)
    {
        components.Remove(component);
        Destroy(component);
    }
    /// <summary>
    /// This method only plays the directional shield effect, does not do any actual damage
    /// </summary>
    /// <param name="hitPoint"></param>
    public void PlayShieldEffect(Vector3 hitPoint)
    {
        //shipShield.gameObject.SetActive(true);
        shipShield.TakeDamage(hitPoint);
    }
    /// <summary>
    /// Coroutine for Destroying ships when HP is 0 (or less)
    /// </summary>
    /// <returns>null after it finishes</returns>
    protected virtual IEnumerator Destroy()
    {
        //play explosion particle effect
        expolosionObject.SetActive(true);
        yield return new WaitForSeconds(0.75f);

        //remove ship graphics
        if (GetComponent<Hull>().hullName == "Organic Corvette")
        {
            transform.FindChild("OrganicCorvette").gameObject.SetActive(false);
        }

        if (GetComponent<Hull>().hullName == "Organic Frigate")
        {
            transform.FindChild("OrganicFrigate").gameObject.SetActive(false);
        }
        transform.FindChild("ComponentGrid").gameObject.SetActive(false);

        //play explosion sound

        //play explosion juice (screen shake, etc)
        yield return new WaitForSeconds(0.75f);
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
        this.ShipBPMetaData = shipBP.MetaData;
        this.shipMove = shipMove;
        this.shipMove.Init();

        InitStats();
        InitReferences();

    }//Init

    protected abstract void PreTurnActions();
    protected abstract void PostTurnActions();
    /// <summary>
    /// Base virtual method to start the turn. Sets power to max.
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator ExecuteTurn()
    {
        #if FULL_DEBUG
        Debug.Log(ShipBPMetaData.BlueprintName + " executing turn");
        #endif
        currentPower = MaxPower;

        yield return null;
    }

    public void ShowComponentSelection(bool show)
    {
        componentCamera.SetActive(show);
    }

    public void ShowTargetingPanel(bool show, Transform targeter)
    {
        targetingCamera.SetActive(show);
        if (show)
        {
            Vector3 directionToTarget = targeter.position - trans.position;
            float angle = Vector3.Angle(trans.forward, directionToTarget);
            Vector3 perp = Vector3.Cross(trans.forward, directionToTarget);
            float dot = Vector3.Dot(perp, trans.up);
            angle = dot > 0.0f ? angle : -angle;
            //Debug.Log("Angle to targeter " + angle);
            targetCamTrans.localEulerAngles = new Vector3(defaultTargetCamEuler.x, defaultTargetCamEuler.y + angle, defaultTargetCamEuler.z);
        }
        //ShowHPBars(show);
    }

    public void ShowHPBars(bool show)
    {
        //Debug.Log("HP bars: " + show);
        hpBar.gameObject.SetActive(show);
        shieldBar.gameObject.SetActive(show);
    }

    #endregion PublicMethods


    #region PrivateMethods

    //Helper
    private void InitStats()
    {
        timeLeftToTurn = turnDelay;

        foreach (FillBar fillBar in GetComponentsInChildren<FillBar>())
        {
            if (fillBar.name == "HPbar")
            {
                hpBar = fillBar;
            }
            else if (fillBar.name == "ShieldBar")
            {
                shieldBar = fillBar;
            }
#if FULL_DEBUG
            else
            {
                Debug.LogError("fillbar is not hp bar nor shield bar, but " + fillBar.name);
            }
#endif

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
        HullHP = maxHullHP;


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

        maxPower = ShipBPMetaData.ExcessPower;
        currentPower = MaxPower;

//        int numThrusters = components.Count(c => c is Comp_Eng_Thruster);
//#if FULL_DEBUG || LOW_DEBUG
//        if (numThrusters <= 0)
//        {
//            Debug.LogError("No Thrusters on Ship");
//        }
//#endif

//        float thrust = ((Comp_Eng_Thruster)components.Find(c => c is Comp_Eng_Thruster)).Thrust;
//        float totalThrust = thrust * numThrusters;
//        int mass = shipBP.Hull.EmptyComponentGrid.Count;
//        MoveCost = mass / totalThrust * 1.5f;
        MoveCost = shipBPMetaData.MoveCost;
        //Debug.Log("Move cost " + MoveCost);
    }

    private void InitReferences()
    {
        trans = transform;
        componentGridTrans = shipBP.Hull.ComponnentGridTrans;

        #if FULL_DEBUG
        if(!componentGridTrans)
        {
            Debug.LogError("No Component Grid trans found");
        }

        if (trans.FindChild("ComponentCamera") == null)
        {
            Debug.LogError("No Component camera found");
        }
        #endif
        componentCamera = trans.FindChild("ComponentCamera").gameObject;
        componentCamera.SetActive(false);

        #if FULL_DEBUG
        targetCamTrans = trans.FindChild("TargetingCamera");
        if (targetCamTrans == null)
        {
            Debug.LogError("No Targeting camera found");
        }
        #endif
        defaultTargetCamEuler = targetCamTrans.eulerAngles;
        targetingCamera = targetCamTrans.gameObject;
        targetingCamera.SetActive(false);

        expolosionObject = trans.FindChild("Explosion").gameObject;

        shipShield = GetComponentInChildren<ShipShield>();
        #if FULL_DEBUG 
        if(!shipShield)
        {
            Debug.LogError("Ship shield not found");
        }
        #endif
        
    }
    #endregion PrivateMethods

    #endregion Methods
}

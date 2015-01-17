#region Usings
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#endregion Usings

public class TurnBasedCombatSystem : Singleton<TurnBasedCombatSystem>
{
    #region Fields
    #region EditorExposed
    [SerializeField]
    private SpaceGround spaceGround;
    [SerializeField]
    private float turnDelayFactor = 200; //lower means higher penalty for having high power
    #endregion EditorExposed
    #region Internal
    public List<TurnBasedUnit> units { get; private set; }
    public bool combatOn { get; private set; }

    private float currentTurnTime;
    private List<TurnBasedUnit> unitsWithSameTime;
    private int numUnitsWithSameTime;
    private bool turnsForUnitsWithSameTime;
    #endregion Internal
    #endregion Fields

    #region Methods

    #region PublicMethods
    public void Init()
    {
        units = new List<TurnBasedUnit>();
        unitsWithSameTime = new List<TurnBasedUnit>();
        numUnitsWithSameTime = 0;
        turnsForUnitsWithSameTime = false;
        spaceGround.OnGroundRightClick += SpaceGroundClick;
    }

    
    public IEnumerator StartCombat()
    {
        #if FULL_DEBUG
        Debug.LogWarning("Starting Combat");
        #endif
        combatOn = true;
        PrepareForCombat();
        while (combatOn)
        {
            SortUnitsByTurnDelay();
            
            yield return StartCoroutine(ExecuteTurnForFirstUnit());
            PostTurnAction();
        }
    }
    public void AddShip(TurnBasedUnit unit)
    {
#if FULL_DEBUG
        if (units.Contains(unit))
        {
            Debug.LogError("Unit already exists in list");
            return;
        }
        Debug.Log("Adding unit " + unit.shipBPMetaData.blueprintName + " to combat with excess power: " + unit.shipBPMetaData.excessPower);
        units.Add(unit);
#else
        units.Add(unit);
#endif
    }

    #endregion PublicMethods

    #region PrivateMethods
    private void PrepareForCombat()
    {
        CalculateTurnDelay();
        
        foreach (TurnBasedUnit unit in units)
        {
            CombatSystemInterface.Instance.AddShipButton(unit);
        }
    }
    private void CalculateTurnDelay()
    {
        float minPower = units.Min(s => s.shipBPMetaData.excessPower);
        foreach (TurnBasedUnit unit in units)
        {
            float shipPower = unit.shipBPMetaData.excessPower;
            float turnFrequency = shipPower / minPower - (shipPower - minPower) / turnDelayFactor;
            unit.TurnDelay = 1 / turnFrequency;
            //Debug.Log("Turn delay for " + unit.shipBPMetaData.blueprintName + ": " + unit.TurnDelay);
        }
    }
    private void SortUnitsByTurnDelay()
    {
        if (unitsWithSameTime.Count > 0)
        {
            numUnitsWithSameTime--;
            units = units
                .Skip(numUnitsWithSameTime)//units with the same item do not get sorted
                .OrderBy(s => s.TimeLeftToTurn).ToList();
            units = unitsWithSameTime.Concat(units).ToList();
        }
        else
        {
            units = units.OrderBy(s => s.TimeLeftToTurn).ToList();
        }
        currentTurnTime = units[0].TimeLeftToTurn;
        if (!turnsForUnitsWithSameTime)
        {
            numUnitsWithSameTime = units.Count(unit => unit.TimeLeftToTurn == currentTurnTime);
        }
        if (numUnitsWithSameTime > 1)
        {
            turnsForUnitsWithSameTime = true;
            unitsWithSameTime = units.Where(unit => unit.TimeLeftToTurn == currentTurnTime).ToList();
            unitsWithSameTime.Shuffle();
            for (int i = 0; i < numUnitsWithSameTime; i++)//re-order units with same time within original list
            {
                units[i] = unitsWithSameTime[i];
            }
        }
        else
        {
            unitsWithSameTime.Clear();
        }

        //update GUI
        CombatSystemInterface.Instance.UpdateTurnOrderPanel(units);
    }
    private void PostTurnAction()
    {
        if (numUnitsWithSameTime > 1)
        {
            units.RemoveAt(0);
            units.Add(unitsWithSameTime[0]);
            unitsWithSameTime.RemoveAt(0);
            if (numUnitsWithSameTime == 0)
            {
                turnsForUnitsWithSameTime = false;
            }
        }
        if (turnsForUnitsWithSameTime && numUnitsWithSameTime == 1)
        {
            units[0].TimeLeftToTurn -= currentTurnTime;
            turnsForUnitsWithSameTime = false;
        }
        else
        {
            foreach (TurnBasedUnit unit in units)
            {
                if (!unitsWithSameTime.Contains(unit))
                {
                    unit.TimeLeftToTurn -= currentTurnTime;
                }
            }
        }
    }
    private IEnumerator ExecuteTurnForFirstUnit()
    {
        yield return StartCoroutine(CameraDirector.Instance.MoveToFocusOn(units[0].transform, 1.0f));
        yield return StartCoroutine(units[0].ExecuteTurn());
    }
    private void EndCombat()
    {

    }
    #region InternalCallbacks
    void SpaceGroundClick(Vector3 worldPosition)
    {
        Debug.Log("Click on ground at position: "+worldPosition);
        if(units[0] is PlayerShip)
        {
            ((PlayerShip)units[0]).Move(worldPosition);
        }
    }
    #endregion InternalCallbacks
    #region UnityCallbacks

    #endregion UnityCallbacks

    #endregion PrivateMethods

    #endregion Methods




}

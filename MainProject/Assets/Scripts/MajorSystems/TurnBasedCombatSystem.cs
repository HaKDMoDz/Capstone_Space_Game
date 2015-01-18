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
    public TurnBasedUnit firstUnit { get; private set; }
    //private int numUnitsWithSameTime;
    //private bool turnsForUnitsWithSameTime;
    #endregion Internal
    #endregion Fields

    #region Methods

    #region PublicMethods
    public void Init()
    {
        units = new List<TurnBasedUnit>();
        unitsWithSameTime = new List<TurnBasedUnit>();
        //numUnitsWithSameTime = 0;
        //turnsForUnitsWithSameTime = false;
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
            PreTurnActions();
            yield return StartCoroutine(ExecuteTurnForFirstUnit());
            PostTurnActions();
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

    private void PreTurnActions()
    {
        if(unitsWithSameTime.Count > 0 )
        {
            CombatSystemInterface.Instance.UpdateTurnOrderPanel(units);
            firstUnit = units[0];
        }
        else
        {
            units = units.OrderBy(s => s.TimeLeftToTurn).ToList();
            firstUnit = units[0];
            currentTurnTime = firstUnit.TimeLeftToTurn;

            //more than 1 unit with the same time as first unit
            if(units.Count(unit=>unit.TimeLeftToTurn==currentTurnTime)>1)
            {
                unitsWithSameTime = units.Where(unit => unit.TimeLeftToTurn == currentTurnTime).ToList();
                foreach (TurnBasedUnit unit in units)
                {
                    unit.TimeLeftToTurn -= currentTurnTime;
                }
            }
            else
            {
                CombatSystemInterface.Instance.UpdateTurnOrderPanel(units);
            }
        }
    }
    //first unit takes turn
    private void PostTurnActions()
    {
        if(unitsWithSameTime.Count > 0)
        {
            units.RemoveAt(0);
            units.Add(unitsWithSameTime[0]);
            unitsWithSameTime.RemoveAt(0);
            if(unitsWithSameTime.Count>0)
            {
                units = units
                .Skip(unitsWithSameTime.Count)//units with the same item do not get sorted
                .OrderBy(s => s.TimeLeftToTurn).ToList();
                units = unitsWithSameTime.Concat(units).ToList();
            }
        }
        else
        {
            foreach (TurnBasedUnit unit in units)
            {
                unit.TimeLeftToTurn -= currentTurnTime;
            }
        }
    }
    private IEnumerator ExecuteTurnForFirstUnit()
    {
        yield return StartCoroutine(CameraDirector.Instance.MoveToFocusOn(firstUnit.transform, 1.0f));
        yield return StartCoroutine(firstUnit.ExecuteTurn());
    }
    private void EndCombat()
    {

    }
    #region InternalCallbacks
    void SpaceGroundClick(Vector3 worldPosition)
    {
        //Debug.Log("Click on ground at position: "+worldPosition);

        if(units[0] is PlayerShip)
        {
            ((PlayerShip)units[0]).Move(worldPosition);
        }
    }
    #endregion InternalCallbacks

    #endregion PrivateMethods

    #endregion Methods


    #region UnusedRandomizationCode
    //private void SortUnitsByTurnDelay()
    //{
    //    if (unitsWithSameTime.Count > 0)
    //    {
    //        numUnitsWithSameTime--;
    //        units = units
    //            .Skip(numUnitsWithSameTime)//units with the same item do not get sorted
    //            .OrderBy(s => s.TimeLeftToTurn).ToList();
    //        //List<TurnBasedUnit> tempList = units;
    //        //units = unitsWithSameTime;
    //        //units = units.Concat(tempList).ToList();
    //        units = unitsWithSameTime.Concat(units).ToList();
    //    }
    //    else
    //    {
    //        units = units.OrderBy(s => s.TimeLeftToTurn).ToList();
    //    }
    //    currentTurnTime = units[0].TimeLeftToTurn;
    //    if (!turnsForUnitsWithSameTime)
    //    {
    //        numUnitsWithSameTime = units.Count(unit => unit.TimeLeftToTurn == currentTurnTime);
    //        if (numUnitsWithSameTime > 1)
    //        {
    //            turnsForUnitsWithSameTime = true;
    //            unitsWithSameTime = units.Where(unit => unit.TimeLeftToTurn == currentTurnTime).ToList();
    //            unitsWithSameTime.Shuffle();
    //            for (int i = 0; i < numUnitsWithSameTime; i++)//re-order units with same time within original list
    //            {
    //                units[i] = unitsWithSameTime[i];
    //            }
    //        }
    //        else
    //        {
    //            unitsWithSameTime.Clear();
    //        }
    //    }
    //    //update GUI
    //    CombatSystemInterface.Instance.UpdateTurnOrderPanel(units);
    //}
    //private void PostTurnAction()
    //{
    //    if (numUnitsWithSameTime > 1)
    //    {
    //        units.RemoveAt(0);
    //        units.Add(unitsWithSameTime[0]);
    //        unitsWithSameTime.RemoveAt(0);
    //        if (numUnitsWithSameTime == 0)
    //        {
    //            turnsForUnitsWithSameTime = false;
    //            //unitsWithSameTime.Clear();
    //        }
    //    }
    //    if (turnsForUnitsWithSameTime && numUnitsWithSameTime == 1)
    //    {
    //        units[0].TimeLeftToTurn -= currentTurnTime;
    //        turnsForUnitsWithSameTime = false;
    //        // unitsWithSameTime.Clear();
    //    }
    //    else
    //    {
    //        foreach (TurnBasedUnit unit in units)
    //        {
    //            if (!unitsWithSameTime.Contains(unit))
    //            {
    //                unit.TimeLeftToTurn -= currentTurnTime;
    //            }
    //        }
    //    }
    //}
    #endregion UnusedRandomizationCode


}

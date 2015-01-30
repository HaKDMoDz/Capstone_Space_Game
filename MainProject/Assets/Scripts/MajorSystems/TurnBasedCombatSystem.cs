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
    //[SerializeField]
    //private SpaceGround spaceGround;
    #endregion EditorExposed

    #region Internal
    public List<TurnBasedUnit> units { get; private set; }
    public List<PlayerShip> playerShips { get; private set; }
    public List<AI_Ship> ai_Ships { get; private set; }

    public bool combatOn { get; private set; }

    private float currentTurnTime;
    private List<TurnBasedUnit> unitsWithSameTime;
    public TurnBasedUnit firstUnit { get; private set; }
    
    #endregion Internal
    #endregion Fields

    #region Methods

    #region PublicMethods

    /// <summary>
    /// Initializes combat system - called by CombatSceneController when before combat starts
    /// </summary>
    public void Init()
    {
        units = new List<TurnBasedUnit>();
        unitsWithSameTime = new List<TurnBasedUnit>();
        playerShips = new List<PlayerShip>();
        ai_Ships = new List<AI_Ship>();
        //raised whenever user clicks on the "ground"
    }

    /// <summary>
    /// Starts the combat turns. Runs some preparations, and then loops through turns while combat is on
    /// </summary>
    public IEnumerator StartCombat()
    {
        #if FULL_DEBUG
        Debug.LogWarning("Starting Combat");
        #endif
        combatOn = true;
        PrepareForCombat();

        //main combat loop
        while (combatOn)
        {
            PreTurnActions();
            // 
            yield return StartCoroutine(ExecuteTurnForFirstUnit());

            PostTurnActions();
        }

        Debug.Log("Combat Complete!");
    }

    /// <summary>
    /// Adds a ship to the turn-based combat system
    /// </summary>
    /// <param name="unit"></param>
    public void AddShip(TurnBasedUnit unit)
    {
#if FULL_DEBUG
        if (units.Contains(unit))
        {
            Debug.LogError("Unit already exists in list");
            return;
        }
        if (unit == null)
        {
            Debug.Log("adding null unit");
        }
       // Debug.Log("Adding unit " + unit.shipBPMetaData.blueprintName + " to combat with excess power: " + unit.shipBPMetaData.excessPower);
        units.Add(unit);
#else
        units.Add(unit);
#endif
    }

    public void KillShip(TurnBasedUnit unit)
    {
        units.Remove(unit);
        unitsWithSameTime.Remove(unit);
        if(unit is AI_Ship)
        {
            ai_Ships.Remove((AI_Ship)unit);
        }
        else if(unit is PlayerShip)
        {
            playerShips.Remove((PlayerShip)unit);
        }
        StartCoroutine(Explode(unit));

        if (ai_Ships.Count <= 0 || playerShips.Count <= 0)
        {
            combatOn = false;
        }
    }

    #region GUIAccess
    public void ShowingSelectionPanel(bool show)
    {
        firstUnit.ShowComponentSelection(show);
    }



    #endregion GUIAccess

    #endregion PublicMethods

    #region PrivateMethods

    private IEnumerator Explode(TurnBasedUnit unit)
    {
        unit.getExplosionObject().SetActive(true);
        yield return new WaitForSeconds(1.25f);
        Destroy(unit.gameObject);
    }

    /// <summary>
    /// does some preliminary actions like calculating each ship's turn delay and setting up the GUI
    /// </summary>
    private void PrepareForCombat()
    {
        CalculateTurnDelay();

        //add each ship to the turn order list in the GUI
        foreach (TurnBasedUnit unit in units)
        {
            if(unit is PlayerShip)
            {
                playerShips.Add((PlayerShip)unit);
            }
            else if(unit is AI_Ship)
            {
                ai_Ships.Add((AI_Ship)unit);
            }
            #if FULL_DEBUG
            else
            {
                Debug.LogWarning("Not player nor AI");
            }
            #endif

            CombatSystemInterface.Instance.AddShipButton(unit);
        }
    }
    /// <summary>
    /// Loops through each ship and calculates it's turn delay based on the formula
    /// </summary>
    private void CalculateTurnDelay()
    {
        if(units == null)
        {
            Debug.Log("units null");
        }
            for (int i = 0; i < units.Count; i++)
        {
            if(units[i]==null)
            {
                Debug.Log("item "+i+" null");
            }
                if(units[i].shipBPMetaData==null)
                {
                    Debug.Log(i + "meta null");
                }
        }
        float minPower = units.Min(s => s.shipBPMetaData.excessPower);
        foreach (TurnBasedUnit unit in units)
        {
            float shipPower = unit.shipBPMetaData.excessPower;
            float turnFrequency = shipPower / minPower - (shipPower - minPower) /GlobalVars.TurnDelayFactor;
            unit.TurnDelay = 1 / turnFrequency;
            //Debug.Log("Turn delay for " + unit.shipBPMetaData.blueprintName + ": " + unit.TurnDelay);
        }
    }

    /// <summary>
    /// called right before each ship takes it's turn. Checks if there are multiple units with the same turn delay and updates the GUI with the current turn order
    /// </summary>
    private void PreTurnActions()
    {
        //if there are multiple ships with the same time, it just updates the GUI and sets the first unit
        if(unitsWithSameTime.Count > 0 )
        {
            CombatSystemInterface.Instance.UpdateTurnOrderPanel(units);
            firstUnit = units[0];
        }
        else //if there are no other ships with the same turn delay
        {
            //sorts the units based on  their time left to turn
            units = units.OrderBy(s => s.TimeLeftToTurn).ToList();
            firstUnit = units[0];

            //records the time left to turn for the first unit - is used to subtract from all other units
            currentTurnTime = firstUnit.TimeLeftToTurn;

            //more than 1 unit with the same time as first unit
            if(units.Count(unit=>unit.TimeLeftToTurn==currentTurnTime)>1)
            {
                //copies all units with the same time into another list
                unitsWithSameTime = units.Where(unit => unit.TimeLeftToTurn == currentTurnTime).ToList();
                //updates the time left to turn for all units - will not update again until all the units with the same time have taken their turn
                foreach (TurnBasedUnit unit in units)
                {
                    unit.TimeLeftToTurn -= currentTurnTime;
                }
            }
            else
            {
                //if there are no units with the same turn, just update the GUI - the sorting has already been taken care of
                CombatSystemInterface.Instance.UpdateTurnOrderPanel(units);
            }
        }
    }
    /// <summary>
    /// Called after a ship takes it's turn. Subtracts the current Time from each ship and other necessary work if there are multiple ships with the same time
    /// </summary>
    private void PostTurnActions()
    {
        //multiple ships with the same time
        if(unitsWithSameTime.Count > 0)
        {
            //moves the unit that just took it's turn to the back of the list
            units.RemoveAt(0);
            units.Add(unitsWithSameTime[0]);
            //remove the first unit from the list for units with the same time as well
            unitsWithSameTime.RemoveAt(0);
            //if there are still any units left that have the same time
            if(unitsWithSameTime.Count>0)
            {
                //sort the units in the main list, but only those that do not have the same time and so still need to take their turn
                units = units
                .Skip(unitsWithSameTime.Count)//units with the same item do not get sorted
                .OrderBy(s => s.TimeLeftToTurn).ToList();
                units = unitsWithSameTime.Concat(units).ToList();
            }
        }
        else
        {
            //if there are no ships with the same time, update the time for all units
            foreach (TurnBasedUnit unit in units)
            {
                unit.TimeLeftToTurn -= currentTurnTime;
            }
        }
    }
    /// <summary>
    /// Executes the turn for the first unit. Let's the camera know to focus on the current unit and then calls Execute Turn on the first unit
    /// </summary>
    /// <returns></returns>
    private IEnumerator ExecuteTurnForFirstUnit()
    {
        firstUnit.transform.FindChild("SelectionHalo").gameObject.SetActive(true);
        yield return StartCoroutine(CameraDirector.Instance.MoveToFocusOn(firstUnit.transform, GlobalVars.CameraMoveToFocusPeriod));
        yield return StartCoroutine(firstUnit.ExecuteTurn());
        CombatSystemInterface.Instance.EnableComponentSelectionPanel(false);
        firstUnit.transform.FindChild("SelectionHalo").gameObject.SetActive(false);
    }
    private void EndCombat()
    {

    }
    
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

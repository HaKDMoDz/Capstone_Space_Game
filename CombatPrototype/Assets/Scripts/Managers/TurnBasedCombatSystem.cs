﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TurnBasedCombatSystem : SingletonComponent<TurnBasedCombatSystem>
{

    public bool combatOn = true;

    List<TurnBasedUnit> units;
    List<TurnBasedUnit> turnHistory;
    //List<TurnBasedUnit> predictedTurnOrder;

    float currentTime = 0f;
    int unitCount;
    int numUnitsWithSameTime;
    TurnBasedUnit firstUnit;
    List<TurnBasedUnit> unitsWithSameTime; //used if 2 units end up with the same turnDelay
    

    void Start()
    {
        StartCoroutine(ExecuteCombat());
    }
    public IEnumerator ExecuteCombat()
    {
        Init();

        unitCount = units.Count; //caching list count


        while (combatOn && unitCount > 0)
        {
            //gets the unit with the lowest turn delay
            firstUnit = units.Aggregate((current, next) =>
                           current.TimeLeftToTurn < next.TimeLeftToTurn ?
                           current : next);

            //saves the current turnDelay for the unit that will execute it's turn, to subtract from all units
            currentTime = firstUnit.TimeLeftToTurn;

            numUnitsWithSameTime = units.Count(unit => unit.TimeLeftToTurn == currentTime);

            if (numUnitsWithSameTime > 1)
            {
                //if more than 1 unit ends up with the same turnDelay, randomly select the order
                unitsWithSameTime = units.Where(unit => unit.TimeLeftToTurn == currentTime).ToList();

                for (int i = 0; i < numUnitsWithSameTime; i++)
                {
                    int unitToTakeTurn = Random.Range(0, numUnitsWithSameTime - i);
                    yield return StartCoroutine(unitsWithSameTime[unitToTakeTurn].ExecuteTurn());
                    turnHistory.Add(unitsWithSameTime[unitToTakeTurn]);
                    unitsWithSameTime.RemoveAt(unitToTakeTurn);
                }

            }
            else
            {
                //the first unit takes it's turn
                yield return StartCoroutine(firstUnit.ExecuteTurn());
                //adds to the turn history - mainly for GUI purposes
                turnHistory.Add(firstUnit);
            }
            //subtracts the currentTime from all units' timers
            for (int i = 0; i < unitCount; i++)
            {
                units[i].TimeLeftToTurn -= currentTime;
            }
        }
        
        //yield return null;
    }

    void Init()
    {
        units = new List<TurnBasedUnit>();
        turnHistory = new List<TurnBasedUnit>();
        //predictedTurnOrder = new List<TurnBasedUnit>();

        //adds all player ships to the units list
        units.AddRange(FindObjectsOfType<PlayerShip>().ToList<TurnBasedUnit>());
        //adds ai ships
        units.AddRange(FindObjectsOfType<AIShip>().ToList<TurnBasedUnit>());
    }


}

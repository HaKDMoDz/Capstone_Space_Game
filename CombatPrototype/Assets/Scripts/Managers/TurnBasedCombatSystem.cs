using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TurnBasedCombatSystem : SingletonComponent<TurnBasedCombatSystem>
{

    List<TurnBasedUnit> units;

    float currentTime = 0f;

    List<TurnBasedUnit> turnHistory;
    List<TurnBasedUnit> predictedTurnOrder;

    public bool combatOn = true;

    void Start()
    {
        StartCoroutine(ExecuteCombat());
    }
    public IEnumerator ExecuteCombat()
    {
        Init();

        int unitCount = units.Count; //caching list count

        TurnBasedUnit firstUnit; //the first unit to move

        while (combatOn && unitCount > 0)
        {
            //gets the unit with the lowest turn delay
            firstUnit = units.Aggregate((current, next) =>
                           current.TimeLeftToTurn < next.TimeLeftToTurn ?
                           current : next);

            //saves the current turnDelay for the unit that will execute it's turn, to subtract from all units
            currentTime = firstUnit.TimeLeftToTurn;

            //the first unit takes it's turn
            yield return StartCoroutine(firstUnit.ExecuteTurn());

            //adds to the turn history - mainly for GUI purposes
            turnHistory.Add(firstUnit);

            //subtracts the currentTime from all units' timers
            for (int i = 0; i < unitCount; i++)
            {
                units[i].TimeLeftToTurn -= currentTime;
            }
        }


        yield return null;
    }

    void Init()
    {
        units = new List<TurnBasedUnit>();
        turnHistory = new List<TurnBasedUnit>();
        predictedTurnOrder = new List<TurnBasedUnit>();

        //adds all player ships to the units list
        units.AddRange(FindObjectsOfType<PlayerShip>().ToList<TurnBasedUnit>());
        //adds ai ships
        units.AddRange(FindObjectsOfType<AIShip>().ToList<TurnBasedUnit>());

    }


}

#region Usings
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
#endregion Usings

public class CombatSystemTester : MonoBehaviour
{

#if UNITY_EDITOR
    #region Fields

    #endregion Fields

    #region Methods

    #region PrivateMethods

    #region UnityCallbacks

    private IEnumerator Start()
    {
        yield return StartCoroutine(RunTests());
        Debug.LogWarning("Tests Complete");
    }

    #endregion UnityCallbacks
    #region Tests
    private IEnumerator RunTests()
    {
        Debug.LogWarning("Running tests");

        Debug.Log("Running Turn Order Logic Test");
        yield return StartCoroutine(TestTurnDelayCalculations(10000));
    }
    private IEnumerator TestTurnDelayCalculations(int numCycles)
    {
        //add ships to list
        //calculate turn delay

        MethodInfo prepareForCombatMethod = typeof(TurnBasedCombatSystem).GetMethod("PrepareForCombat", BindingFlags.NonPublic | BindingFlags.Instance);
        MethodInfo sortUnitsMethod = typeof(TurnBasedCombatSystem).GetMethod("SortUnitsByTurnDelay", BindingFlags.NonPublic | BindingFlags.Instance);
        MethodInfo postTurnMethod = typeof(TurnBasedCombatSystem).GetMethod("PostTurnAction", BindingFlags.NonPublic | BindingFlags.Instance);


        prepareForCombatMethod.Invoke(TurnBasedCombatSystem.Instance, null);
        //sort units
        Dictionary<TurnBasedUnit, int> unit_turnCount_table = TurnBasedCombatSystem.Instance.units.ToDictionary(s => s, s => 0);
        //get 1st unit
        for (int i = 0; i < numCycles; i++)
        {
            sortUnitsMethod.Invoke(TurnBasedCombatSystem.Instance, null);
            TurnBasedUnit firstUnit = TurnBasedCombatSystem.Instance.units[0];
            unit_turnCount_table[firstUnit]++;
            //Debug.Log(firstUnit.shipBPMetaData.blueprintName + " takes it's turn");
            postTurnMethod.Invoke(TurnBasedCombatSystem.Instance, null);
        }

        Debug.Log("Num turns for unit: ");
        foreach (var unit_turnCount in unit_turnCount_table)
        {
            Debug.Log(unit_turnCount.Key.shipBPMetaData.blueprintName + " : " + unit_turnCount.Value + " turns");
        }
        TurnBasedUnit unitWithLeastTurns = unit_turnCount_table.Aggregate((current, next) =>
                                                current.Value < next.Value ?
                                                current : next)
                                                .Key;
        Debug.Log("Slowest unit: " + unitWithLeastTurns.shipBPMetaData.blueprintName);
        Debug.Log("Turn ratios: ");
        foreach (var unit_turnCount in unit_turnCount_table)
        {
            Debug.Log(unit_turnCount.Key.shipBPMetaData.blueprintName + ": " + (float)unit_turnCount.Value / (float)unit_turnCount_table[unitWithLeastTurns]);
        }

        yield return null;
    }
    #endregion Tests
    #endregion PrivateMethods

    #endregion Methods
#endif

}

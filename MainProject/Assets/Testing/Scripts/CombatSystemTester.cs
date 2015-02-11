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
        yield return StartCoroutine(TestTurnDelayCalculations(1000));
        yield return null;
    }
    /// <summary>
    /// tests turn delay calculations to make sure the ratios for turns that each unit takes is as expected
    /// </summary>
    /// <param name="numCycles"></param>
    /// <returns></returns>
    private IEnumerator TestTurnDelayCalculations(int numCycles)
    {
        //add ships to list
        //calculate turn delay

        MethodInfo PrepareForCombatMethod = typeof(TurnBasedCombatSystem).GetMethod("PrepareForCombat", BindingFlags.NonPublic | BindingFlags.Instance);
        MethodInfo PreTurnMethod = typeof(TurnBasedCombatSystem).GetMethod("PreTurnActions", BindingFlags.NonPublic | BindingFlags.Instance);
        MethodInfo PostTurnMethod = typeof(TurnBasedCombatSystem).GetMethod("PostTurnActions", BindingFlags.NonPublic | BindingFlags.Instance);

        TurnBasedCombatSystem combatSystem = TurnBasedCombatSystem.Instance;

        PrepareForCombatMethod.Invoke(combatSystem, null);
        //sort units
        Dictionary<TurnBasedUnit, int> unit_turnCount_table = combatSystem.units.ToDictionary(s => s, s => 0);
        //get 1st unit
        for (int i = 0; i < numCycles; i++)
        {
            Debug.Log("Cycle # " + i);
            PreTurnMethod.Invoke(combatSystem, null);
            unit_turnCount_table[combatSystem.firstUnit]++;
            //Debug.Log(firstUnit.shipBPMetaData.blueprintName + " takes it's turn");
            PostTurnMethod.Invoke(combatSystem, null);
        }

        Debug.Log("Num turns for unit: ");
        foreach (var unit_turnCount in unit_turnCount_table)
        {
            Debug.Log(unit_turnCount.Key.shipBPMetaData.BlueprintName + " : " + unit_turnCount.Value + " turns");
        }
        TurnBasedUnit unitWithLeastTurns = unit_turnCount_table.Aggregate((current, next) =>
                                                current.Value < next.Value ?
                                                current : next)
                                                .Key;
        Debug.Log("Slowest unit: " + unitWithLeastTurns.shipBPMetaData.BlueprintName);
        Debug.Log("Turn ratios: ");
        foreach (var unit_turnCount in unit_turnCount_table)
        {
            Debug.Log(unit_turnCount.Key.shipBPMetaData.BlueprintName + ": " + (float)unit_turnCount.Value / (float)unit_turnCount_table[unitWithLeastTurns]);
        }

        yield return null;
    }
    #endregion Tests
    #endregion PrivateMethods

    #endregion Methods
#endif

}

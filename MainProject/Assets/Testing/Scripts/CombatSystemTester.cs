using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

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
        yield return StartCoroutine(TestTurnDelayCalculations(100));
    }
    private IEnumerator TestTurnDelayCalculations(int numCycles)
    {
        //add ships to list
        //calculate turn delay
        MethodInfo calculateTurnDelayMethod = typeof(TurnBasedCombatSystem).GetMethod("CalculateTurnDelay", BindingFlags.NonPublic | BindingFlags.Instance);
        calculateTurnDelayMethod.Invoke(TurnBasedCombatSystem.Instance, null);
        MethodInfo sortUnitsMethod = typeof(TurnBasedCombatSystem).GetMethod("SortUnitsByTurnDelay", BindingFlags.NonPublic | BindingFlags.Instance);
        sortUnitsMethod.Invoke(TurnBasedCombatSystem.Instance, null);
        //sort units
        //get 1st unit
            //counter++
        //post turn action


        yield return null;
    }
    #endregion Tests
    #endregion PrivateMethods

    #endregion Methods
#endif

}

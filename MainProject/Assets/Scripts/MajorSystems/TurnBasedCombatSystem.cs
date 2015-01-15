using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TurnBasedCombatSystem : Singleton<TurnBasedCombatSystem>
{
    #region Fields
    #region EditorExposed
    [SerializeField]
    private float turnDelayFactor = 200; //lower means higher penalty for having high power
    #endregion EditorExposed
    #region Internal
    public List<TurnBasedUnit> ships { get; private set; }
    #endregion Internal
    #endregion Fields

    #region Methods

    #region PublicMethods
    public void Init()
    {
        ships = new List<TurnBasedUnit>();
    }
    public IEnumerator StartCombat()
    {
        yield return null;
    }
    public void AddShip(TurnBasedUnit unit)
    {
        #if FULL_DEBUG
        if(ships.Contains(unit))
        {
            Debug.LogError("Unit already exists in list");
            return;
        }
        Debug.Log("Adding unit " + unit.shipBPMetaData.blueprintName + " with excess power: " + unit.shipBPMetaData.excessPower);
        ships.Add(unit);
        #else
        #endif
    }
    #endregion PublicMethods

    #region PrivateMethods
    private void CalculateTurnDelay()
    {
        float minPower = ships.Min(s => s.shipBPMetaData.excessPower);
        foreach (TurnBasedUnit unit in ships)
        {
            float shipPower = unit.shipBPMetaData.excessPower;
            unit.TurnDelay = shipPower / minPower - (shipPower - minPower) / turnDelayFactor;
        }
    }
    #endregion PrivateMethods

    #endregion Methods


}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TurnBasedCombatSystem : Singleton<TurnBasedCombatSystem>
{
    #region Fields
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
    #endregion PrivateMethods

    #endregion Methods


}

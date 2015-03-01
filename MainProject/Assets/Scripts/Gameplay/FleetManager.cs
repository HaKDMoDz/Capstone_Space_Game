using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FleetManager : Singleton<FleetManager>
{

    #region Fields
    //EditorExposed
    [SerializeField]
    private int maxFleetStrength;
    private int currentFleetStrength;
    public int CurrentFleetStrength
    {
        get { return currentFleetStrength; }
    }

    private List<string> currentFleet;
    public List<string> CurrentFleet
    {
        get { return currentFleet; }
        set
        {
            currentFleet = value;
            foreach (string blueprintName in currentFleet)
            {
                ShipDesignInterface.Instance.AddCurrentFleetButton(blueprintName);
            }
        }
    }

    

    #endregion Fields

    #region Methods
    #region Public
    #region GUIAccess
    private void AddShipToFleet(string blueprintName)
    {
        currentFleet.Add(blueprintName);
    }
    public bool TryAddShipToFleet(string blueprintName, int shipCost)
    {
        if(currentFleetStrength + shipCost > maxFleetStrength)
        {
            return false;
        }
        else
        {
            AddShipToFleet(blueprintName);
            return true;
        }
    }
    public void RemoveShipFromFleet(string blueprintName)
    {
#if !NO_DEBUG
        if (currentFleet.Contains(blueprintName))
        {
            currentFleet.Remove(blueprintName);
        }
        else
        {
            Debug.LogError("Ship does not exist in fleet");
        }
#else //NO_DEBUG
        currentFleet.Remove(shipBP);
#endif
    }
    public bool CurrentFleetContains(string blueprintName)
    {
        return currentFleet.Contains(blueprintName);
    }
    #endregion GUIAccess
    #endregion Public

    #region Private
    #region UnityCallbacks
    private void Awake()
    {
        currentFleet = new List<string>();
    }
    
    #endregion UnityCallbacks
    #endregion Private
    #endregion Methods
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CombatSceneController : Singleton<CombatSceneController>
{
    #region Fields

    #region EditorExposted
    [SerializeField]
    private HullTable hullTableSO;
    [SerializeField]
    private ComponentTable compTableSO;

    #endregion EditorExposted

    #region Internal

    private List<GameObject> playerShips;

    //references
    private PlayerFleetData playerFleetData;
    private ShipBuilder shipBuilder;
    #endregion Internal

    #endregion Fields

    #region Methods

    #region PublicMethods

    public void SetupScene()
    {
        #if FULL_DEBUG
        Debug.Log("Setup Combat Scene");
        #endif

        Init();

        //setup background objects, etc.

        //build player fleet
        playerFleetData = GameController.Instance.GameData.playerFleetData;
        
        #if FULL_DEBUG
        if(playerFleetData.currentFleet_BlueprintNames.Count==0)
        {
            Debug.LogError("Empty player fleet");
        }
	    #endif

        Vector3 spawnPos = Vector3.zero;
        int numShips = playerFleetData.currentFleet_BlueprintNames.Count;
        int spawnSpacing = 50;
        spawnPos.x -= spawnSpacing * numShips / 2;

        foreach (string blueprintName in playerFleetData.currentFleet_BlueprintNames)
        {
            playerShips.Add(shipBuilder.BuildShip(ShipType.PlayerShip, blueprintName, spawnPos, Quaternion.identity));
            spawnPos.x += spawnSpacing;
        }

        //build AI fleet

    }

    #endregion PublicMethods

    #region Private
    private void Init()
    {

        playerShips = new List<GameObject>();
        shipBuilder = new ShipBuilder();
        //shipBuilder = ShipBuilder.Instance;
        //hullTableSO.Init();
        //compTableSO.Init();
    }

    #region UnityCallbacks
    
    //private void Awake()
    //{
    //    #if FULL_DEBUG
    //    Debug.Log("Combat Controller Awake");
    //    #endif
    //}
    private void Start()
    {
        SetupScene();
    }
    
    #endregion UnityCallbacks

    #region InternalCallbacks

    #endregion InternalCallbacks

    #endregion Private

    #endregion Methods
}

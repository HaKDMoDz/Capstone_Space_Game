#region Usings
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
#endregion Usings

public class ShipDesignSystem : Singleton<ShipDesignSystem>
{
    #region Fields

    //Internal
    public PlayerFleetData playerFleetData { get; private set; }

    //References
    ShipBlueprintSaveSystem saveSystem; //Handles saving ship blueprints

    //these vars keep track of various factors regarding building ships
    public bool buildingShip { get; private set; } //Whether a ship is actively being built
    [SerializeField] private ShipBlueprint blueprintBeingBuilt; //the blueprint representing the ship currently being built
    private Hull hullBeingBuilt; //the intantiated Hull GameObject
    private List<ShipComponent> componentsBeingBuilt; //the instantiated ShipComponent GameObjects
    private Dictionary<ComponentSlot, ShipComponent> slot_compsBeingBuilt_table; //The instantiated components corresponding to the ComponentSlots
    
    #endregion Fields

    #region Methods
    #region Public
    /// <summary>
    /// Starts building a ship using the hull corresponding to the hull_ID. 
    /// Starts a blueprint to keep track of the actual hull and components and instantiates the ship on screen.
    /// Nothing happens if a ship is already being built
    /// </summary>
    /// <param name="hull_ID">
    /// ID of the hull to start building the ship with
    /// </param>
    public void BuildHull(int hull_ID)
    {
        if (!buildingShip)
        {
            blueprintBeingBuilt.Clear();
            blueprintBeingBuilt.Hull = HullTable.GetHull(hull_ID);
            #if FULL_DEBUG
            Debug.Log("Building hull: " + blueprintBeingBuilt.Hull.hullName);
            #endif
            TutorialSystem.Instance.ShowNextTutorial(TutorialSystem.TutorialType.BuildHull);
            AddHullToScene(blueprintBeingBuilt.Hull);
            buildingShip = true;
        }
        else
        {
            #if FULL_DEBUG
            Debug.Log("Already building a ship");
            #endif
        }
    }//BuildHull

    /// <summary>
    /// Installs a component onto the specified component slot on the blueprint being built and instantiates it onto the ship in the scene
    /// </summary>
    /// <param name="slot">
    /// The component slot to built the component on
    /// </param>
    /// <param name="component">
    /// The component to build
    /// </param>
    public void BuildComponent(ComponentSlot slot, ShipComponent component)
    {

#if !NO_DEBUG
        if(buildingShip)
        {
            if(slot.InstalledComponent)
            {
                if (ComponentTable.GetID(slot.InstalledComponent) == ComponentTable.GetID(component))
                {
                    return;
                }

                //if a component is already present, delete it from the scene and blueprint
                Debug.Log("Replacing component");
                ShipComponent otherComp = slot_compsBeingBuilt_table[slot];
                componentsBeingBuilt.Remove(otherComp);
                Destroy(otherComp.gameObject);
                blueprintBeingBuilt.RemoveComponent(slot.index);
            }
            #if FULL_DEBUG
            Debug.Log("Building component " + component.componentName + " on slot " + slot.index);
            #endif
            TutorialSystem.Instance.ShowNextTutorial(TutorialSystem.TutorialType.BuildComponent);
            AddComponentToScene(slot, component);
            blueprintBeingBuilt.AddComponent(slot, component);
            //blueprintBeingBuilt.GenerateMetaData();
            if (blueprintBeingBuilt.IsValid())
            {
                ShipDesignInterface.Instance.AllowSaving(true);
                TutorialSystem.Instance.ShowTutorial(TutorialSystem.TutorialType.SaveShip, true);
            }
            ShipDesignInterface.Instance.UpdateStatsPanel(blueprintBeingBuilt.MetaData);
            
        }
        else
        {
            #if FULL_DEBUG || LOW_DEBUG
            Debug.LogError("No ship being built");
            #endif
        }
#else //if NO_DEBUG
        AddComponentToScene(slot, component);
        blueprintBeingBuilt.AddComponent(slot, component);
#endif
    }//BuildComponent


    /// <summary>
    /// Removes everything from the scene, destroying all instantiated objects
    /// </summary>
    public void ClearScreen()
    {
        #if FULL_DEBUG
        Debug.Log("Clear Screen");
        #endif

        buildingShip = false;
        blueprintBeingBuilt.Clear();
        if (hullBeingBuilt)
        {
            Destroy(hullBeingBuilt.gameObject);
        }
        hullBeingBuilt = null;
        if (componentsBeingBuilt.Count > 0)
        {
            foreach (ShipComponent comp in componentsBeingBuilt)
            {
                Destroy(comp.gameObject);
            }
        }
        componentsBeingBuilt.Clear();
        slot_compsBeingBuilt_table.Clear();
    }//ClearScreen

    public void SaveFleet()
    {
        #if FULL_DEBUG
        Debug.Log("Saving fleet");
        #endif
        playerFleetData.currentFleet_meta_list = FleetManager.Instance.CurrentFleet;
        GameController.Instance.GameData.playerFleetData = playerFleetData;
        GameController.Instance.QuickSave();
    }

    #region SaveSystemInterface 
    //These methods provide other scripts access to the Save System

    /// <summary>
    /// Saves the current blueprint being built
    /// </summary>
    /// <param name="fileName">
    /// The name of the blueprint to save as
    /// </param>
    public void SaveBlueprint(string fileName)
    {
        blueprintBeingBuilt.GenerateMetaData(fileName);
        ShipDesignInterface.Instance.UpdateStatsPanel(blueprintBeingBuilt.MetaData);
        saveSystem.SaveBlueprint(blueprintBeingBuilt, fileName);
        TutorialSystem.Instance.ShowNextTutorial(TutorialSystem.TutorialType.SaveShip);
    }
    /// <summary>
    /// Loads a saved blueprint and adds it to the scene; clearing any unsaved changes to the current ship being built
    /// </summary>
    /// <param name="fileName">
    /// Name of the blueprint to load
    /// </param>
    public void LoadBlueprint(string fileName)
    {
        ClearScreen();
#if !NO_DEBUG
        if(saveSystem.LoadBlueprint(out blueprintBeingBuilt, fileName))
        {
            AddHullToScene(blueprintBeingBuilt.Hull);
            //loop through all components listed in the blueprint and add them to the scene
            foreach (var slot_component in blueprintBeingBuilt.Slot_component_table)
            {
                //need to get the actual component slot that corresponds to the slot index in the blueprint
                int slotIndex = slot_component.Key.index;
                ComponentSlot slotToBuildOn = hullBeingBuilt.index_slot_table[slotIndex];
                AddComponentToScene(slotToBuildOn, slot_component.Value);
                slotToBuildOn.InstalledComponent = slot_component.Value;
            }
            buildingShip = true;
        }
        else
        {
            Debug.LogError("ShipBlueprint named " + fileName + " could not be found");
        }
#else //NO_DEBUG
        saveSystem.LoadBlueprint(out blueprintBeingBuilt, fileName);
        AddHullToScene(blueprintBeingBuilt.hull);
        foreach (var slot_component in blueprintBeingBuilt.slot_component_table)
        {
            AddComponentToScene(slot_component.Key, slot_component.Value);
        }
        buildingShip = true;
#endif
    }//LoadBlueprint

    /// <summary>
    /// Deletes the specified blueprint
    /// </summary>
    /// <param name="fileName"></param>
    public void DeleteBlueprint(string fileName)
    {
        #if !NO_DEBUG
        Debug.LogWarning("Deleting Blueprint " + fileName);
        #endif
        saveSystem.DeleteBlueprint(fileName);
    }
    /// <summary>
    /// Deletes all saved blueprints 
    /// </summary>
    public void DeleteAllBlueprints()
    {
        #if !NO_DEBUG
        Debug.LogWarning("Deleting all ship blueprints");
        #endif
        saveSystem.DeleteAllBlueprints();
    }
    /// <summary>
    /// returns a list of filenames for all saved blueprints
    /// </summary>
    /// <returns>
    /// List of filenames for all saved blueprints
    /// </returns>
    public List<string> GetSaveFileList()
    {
        return saveSystem.savedBPList.blueprintMetaDataList.Select(b=>b.BlueprintName).ToList();
    }
    /// <summary>
    /// Returns whether a file with the name already exists
    /// </summary>
    /// <param name="filename"></param>
    /// <returns></returns>
    public bool FileExists(string filename)
    {
        return saveSystem.savedBPList.Contains(filename);
    }
    public ShipBlueprintMetaData GetMetaData(string blueprintName)
    {
        return saveSystem.savedBPList.GetMetaData(blueprintName);
    }
    #endregion SaveSystemInterface

    #endregion Public

    #region Private
    /// <summary>
    /// Instantiates the hull prefab (rotated 90 Deg) and initializes it.
    /// Also adjusts the camera so the ship fits on screen properly
    /// </summary>
    /// <param name="hull">
    /// The hull prefab to instantiate
    /// </param>
    private void AddHullToScene(Hull hull)
    {
        hullBeingBuilt = Instantiate(hull, Vector3.zero, Quaternion.Euler(0.0f, 90.0f, 0.0f)) as Hull;
        ShipDesignCamera.Instance.SetCameraForHull(hull);
        hullBeingBuilt.Init();
        //turn off cameras in hull prefab
        foreach (GameObject obj in hullBeingBuilt.GetComponentsInChildren<Camera>().Select(c=>c.gameObject))
        {
            obj.SetActive(false);
        }
        blueprintBeingBuilt.GenerateMetaData();
        ShipDesignInterface.Instance.UpdateStatsPanel(blueprintBeingBuilt.MetaData);
        //camera
    }
    /// <summary>
    /// Intantiates a component on the specified slot
    /// </summary>
    /// <param name="slot">
    /// The component slot to build the component on
    /// </param>
    /// <param name="component">
    /// The component prefab to instantiate
    /// </param>
    private void AddComponentToScene(ComponentSlot slot, ShipComponent component)
    {
        ShipComponent builtComp = Instantiate(component, slot.transform.position, slot.transform.rotation) as ShipComponent;
        //turn off GUI in component prefab
        builtComp.GetComponentInChildren<Canvas>().gameObject.SetActive(false);
        //tracking components that are built for deletion
        componentsBeingBuilt.Add(builtComp);
        if(slot_compsBeingBuilt_table.ContainsKey(slot))
        {
            slot_compsBeingBuilt_table[slot] = builtComp;
        }
        else
        {
            slot_compsBeingBuilt_table.Add(slot, builtComp);
        }
        
    }

    #region UnityCallBacks
    private void Awake()
    {
        //hullTableScriptableObject.Init();
        //compTableScriptableObject.Init();

        componentsBeingBuilt = new List<ShipComponent>();
        blueprintBeingBuilt = new ShipBlueprint();
        slot_compsBeingBuilt_table = new Dictionary<ComponentSlot, ShipComponent>();
        //name_savedBP_table = new Dictionary<string, ShipBlueprint>();
        saveSystem = new ShipBlueprintSaveSystem();//saveFields.fileExtension_ShipBP, saveFields.saveDirectory_ShipBP, saveFields.fileName_SaveList);
        playerFleetData = new PlayerFleetData();
    }
    private void Start()
    {
        GameController.Instance.OnPreSceneChange += PreSceneChange;
        playerFleetData = GameController.Instance.GameData.playerFleetData;
        if (ValidateFleet())
        {
            FleetManager.Instance.CurrentFleet = playerFleetData.currentFleet_meta_list;
        }
        else
        {
            FleetManager.Instance.CurrentFleet.Clear();
        }
        ShipDesignInterface.Instance.Init();
    }
    #endregion UnityCallBacks

    #region InternalCallbacks
    private void PreSceneChange(SceneChangeArgs args)
    {
        SaveFleet();
    }
    #endregion InternalCallbacks

    #region Helper
    private bool ValidateFleet()
    {
        foreach (string blueprintName in playerFleetData.currentFleet_meta_list.Select(meta=>meta.BlueprintName))
        {
            if(!saveSystem.savedBPList.Contains(blueprintName))
            {
                return false;
            }
        }
        return true;
    }
    #endregion Helper
    #endregion Private

    #endregion Methods

}

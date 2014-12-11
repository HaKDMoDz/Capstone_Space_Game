using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

#region AdditionalData
public enum GameScene { MainMenu, GalaxyMap, CombatScene, ShipDesignScene }

[Serializable]
public struct SceneNameEntry
{
    public GameScene gameScene;
    public string sceneName;
}
//for a prettier inspector window
[Serializable]
public struct SaveFields
{
    public string fileExtension;
    public string saveDirectory;
    public string fileName_SavesList;
    public string autoSaveFileName;
    public int numAutoSaves;
    public string quickSaveName;
    public int numQuickSaves;
    public int numNormalSaves;
}

#endregion//Additional Data


public class GameController : Singleton<GameController>
{
    #region Fields
    
    #region EditorExposed
    [SerializeField]
    private GameScene defaultStartScene = GameScene.MainMenu;
    [SerializeField]
    private List<SceneNameEntry> sceneEntryList;
    [SerializeField]
    SaveFields saveFields;
    #endregion //EditorExposed

    #region References
    private GameSaveSystem saveSystem;

    #endregion References

    #region InternalFields
    //private GameScene currentScene;
    private Dictionary<GameScene, string> sceneEnumToNameTable;
    //need this for now - until Button's onClick event can pass in enums
    private Dictionary<string, GameScene> sceneNameToEnumTable;
    private GameData gameData; //will hold the current game state
    #endregion //Internal

    #region Events
    public delegate void PreSceneChange(SceneChangeArgs args);
    public event PreSceneChange OnPreSceneChange = new PreSceneChange((SceneChangeArgs) => { });

    public delegate void PostSceneChange(SceneChangeArgs args);
    public event PostSceneChange OnPostSceneChange = new PostSceneChange((SceneChangeArgs) => { });
    #endregion //Events

    #endregion //Fields

    #region Methods
    #region Public

    /// <summary>
    /// Called by the change scene button 
    /// need this for now - until Button's onClick event can pass in enums
    /// </summary>
    /// <param name="sceneName">
    /// verify the string using the GameController inspector
    /// </param>
    public void ChangeScene(string sceneName)
    {
        //Debug.Log("Change Scene Button");
        ChangeScene(sceneNameToEnumTable[sceneName]);
    }
    /// <summary>
    /// Call this method to change the scene
    /// Raises a PreSceneChange event, autosaves, and then triggers a Unity scene change
    /// Once a new scene is loaded, the last autosave is loaded and a PostSceneChange event is raised
    /// </summary>
    /// <param name="nextScene">
    /// pass in the enum representing the scene to change to
    /// </param>
    public void ChangeScene(GameScene nextScene)
    {

        #if !NO_DEBUG
        Debug.LogWarning("Scene changing from " + gameData.currentScene + " to " + nextScene);
        #endif
        #if FULL_DEBUG
        Debug.Log("PreSceneChange event raised: " + gameData.currentScene + " to " + nextScene);
        #endif

        //notifies all other systems that the scene is about to change
        //they should all update their "data" structures so they can be persisted
        OnPreSceneChange(new SceneChangeArgs(gameData.currentScene, nextScene));
        saveSystem.AutoSave(gameData);
        Application.LoadLevel(sceneEnumToNameTable[nextScene]);
    }//ChangeScene

    /// <summary>
    /// Will return true if any saves exist - essentially if a game has been started and can be continued/loaded
    /// </summary>
    /// <returns></returns>
    public bool AnySavesExist()
    {
        return saveSystem.AnySavesExist();
    }

    #endregion //Public

    #region Private
    private void Awake()
    {
        #if FULL_DEBUG
        //Debug.Log("GameController Awake");
        #endif

        #if !NO_DEBUG
        if (sceneEntryList.Count == 0)
        {
            Debug.LogError("No Scene Name Entries provided");
            return;
        }
        #endif
        //create scene name table
        sceneEnumToNameTable = sceneEntryList.ToDictionary(s => s.gameScene, s => s.sceneName);
        //need this for now - until Button's onClick event can pass in enums
        sceneNameToEnumTable = sceneEntryList.ToDictionary(s => s.sceneName, s => s.gameScene);

        saveSystem = new GameSaveSystem(
            saveFields.fileExtension, saveFields.saveDirectory, saveFields.fileName_SavesList, 
            saveFields.autoSaveFileName,saveFields.quickSaveName,
            saveFields.numAutoSaves, saveFields.numQuickSaves, saveFields.numNormalSaves);

        //empty game data - will be filled up upon loading an autosave
        gameData = new GameData();

        //Debug.Log("autosave: " + autosaveFileName);
        
        //attempt to load the latest autosave
        if(saveSystem.LoadAutoSave(ref gameData))
        {
            #if FULL_DEBUG
            Debug.Log("Game Data loaded successfully");
            #endif
        }
        else
        {
            #if !NO_DEBUG
            Debug.LogWarning("No AutoSave found, default GameData created");
            #endif
            gameData = new GameData(defaultStartScene);
        }

    }//Awake

    /// <summary>
    /// Unity call back after a scene is loaded
    /// Raises the PostSceneChange event
    /// </summary>
    /// <param name="levelID"></param>
    private void OnLevelWasLoaded(int levelID)
    {
        #if !NO_DEBUG
        Debug.Log("Post Scene Change: from " + gameData.currentScene + " to " + sceneNameToEnumTable[Application.loadedLevelName]);
        #endif

        //notifies all systems that a new scene has loaded
        OnPostSceneChange(new SceneChangeArgs(gameData.currentScene, sceneNameToEnumTable[Application.loadedLevelName]));
        gameData.currentScene = sceneNameToEnumTable[Application.loadedLevelName];
    }

    #endregion //Private methods

    #endregion //Methods
}

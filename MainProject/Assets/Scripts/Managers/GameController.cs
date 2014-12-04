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
#endregion//Additional Data


public class GameController : Singleton<GameController>
{
    #region Fields
    #region EditorExposed
    [SerializeField]
    private GameScene defaultStartScene = GameScene.MainMenu;
    [SerializeField]
    private List<SceneNameEntry> sceneEntryList;

    //save game related
    [SerializeField]
    private string fileExtension, saveDirectory, fileName_SavesList;

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

    //static int count = 0;

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
    //need this for now - until Button's onClick event can pass in enums
    public void ChangeScene(string sceneName)
    {
        //Debug.Log("Change Scene Button");
        ChangeScene(sceneNameToEnumTable[sceneName]);
    }

    public void ChangeScene(GameScene nextScene)
    {

        #if !NO_DEBUG
        Debug.LogWarning("Scene changing from " + gameData.currentScene + " to " + nextScene);
        #endif
        #if FULL_DEBUG
        Debug.Log("PreSceneChange event raised: " + gameData.currentScene + " to " + nextScene);
        #endif

        OnPreSceneChange(new SceneChangeArgs(gameData.currentScene, nextScene));
        saveSystem.Save(gameData, "save1");
        Application.LoadLevel(sceneEnumToNameTable[nextScene]);

        //OnPostSceneChange(new SceneChangeArgs(currentScene, nextScene));
    }
    #endregion //Public

    #region Private
    void Awake()
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

        //currentScene = defaultStartScene;

        saveSystem = new GameSaveSystem(fileExtension, saveDirectory, fileName_SavesList);

        if(saveSystem.Load(out gameData, "save1"))
        {
            #if FULL_DEBUG
            Debug.Log("Game Data loaded successfully");
            #endif
        }
        else
        {
            #if !NO_DEBUG
            Debug.LogError("No Save game \"save1\" found, new game data created");
            #endif
            gameData = new GameData(defaultStartScene);
        }
    }
    void OnLevelWasLoaded(int levelID)
    {
        #if !NO_DEBUG          
        Debug.Log("Post Scene Change: from " + gameData.currentScene + " to " + sceneNameToEnumTable[Application.loadedLevelName]);
        #endif
        OnPostSceneChange(new SceneChangeArgs(gameData.currentScene, sceneNameToEnumTable[Application.loadedLevelName]));
        gameData.currentScene = sceneNameToEnumTable[Application.loadedLevelName];
    }

    #endregion //Private methods

    #endregion //Methods
}

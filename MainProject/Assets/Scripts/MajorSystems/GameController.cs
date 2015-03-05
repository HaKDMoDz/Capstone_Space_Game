using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;


//The scenes that will be in the release build and are managed by the game controller
public enum GameScene {MainMenu, GalaxyMap, CombatScene, ShipDesignScene }


public class GameController : Singleton<GameController>
{
    #region Fields
    
    //EditorExposed
    [SerializeField]
    private LoadingScreen loadingScreen;

    //References
    private GameSaveSystem saveSystem;

    // InternalFields
    private GameScene defaultStartScene = GameScene.MainMenu;
    //private GameScene currentScene;
    //private Dictionary<GameScene, string> sceneEnumToNameTable;
    ////need this for now - until Button's onClick event can pass in enums
    //private Dictionary<string, GameScene> sceneNameToEnumTable;
    private GameData gameData;//will hold the current game state
    public GameData GameData
    {
        get { return gameData; }
    }
    
    //private GameScene currentScene;

    //Events
    //raised before a Unity scene change is triggered - a hint for all systems to prepare to save whatever they are doing and prepare to be shut down
    public delegate void PreSceneChange(SceneChangeArgs args);
    public event PreSceneChange OnPreSceneChange = new PreSceneChange((SceneChangeArgs) => { });
    //raised after Unity has completed loading a new scene - systems should load up the latest save file and prepare to resume activity
    public delegate void PostSceneChange(SceneChangeArgs args);
    public event PostSceneChange OnPostSceneChange = new PostSceneChange((SceneChangeArgs) => { });
    public delegate void QuitEvent();
    public event QuitEvent OnQuit = new QuitEvent(() => { });
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
        ChangeScene(GameConfig.GetSceneEnum(sceneName));
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
        gameData.nextScene = nextScene;
        #if !NO_DEBUG
        Debug.LogWarning("Scene changing from " + gameData.prevScene + " to " + nextScene);
        #endif

        if (gameData.prevScene == GameScene.MainMenu || gameData.nextScene == GameScene.MainMenu)
        {
            #if FULL_DEBUG
            Debug.Log("Loading from/to main menu - no SceneChangeEvents");
            #endif
        }
        else if(gameData.prevScene == gameData.nextScene)
        {
            #if FULL_DEBUG
            Debug.Log("Scene did not change - no SceneChangeEvents");
            #endif
        }
        else
        {
            #if FULL_DEBUG
            Debug.Log("PreSceneChange event raised: " + gameData.prevScene + " to " + nextScene);
            #endif
            //notifies all other systems that the scene is about to change
            //they should all update their "data" structures so they can be persisted
            OnPreSceneChange(new SceneChangeArgs(gameData.prevScene, nextScene));
        }
        saveSystem.AutoSave(gameData);
        loadingScreen.gameObject.SetActive(true);
        loadingScreen.LoadLevel(GameConfig.GetSceneName(nextScene));
        //Application.LoadLevel(GameConfig.GetSceneName(nextScene));

    }//ChangeScene

    /// <summary>
    /// Called by the Main Menu - transitions to the galaxy map to start a new game
    /// </summary>
    public void StartNewGame()
    {
        ChangeScene(GameScene.GalaxyMap);
    }


    #region SaveSystemInterface
    /// <summary>
    /// Will return true if any saves exist - essentially if a game has been started and can be continued/loaded
    /// </summary>
    /// <returns></returns>
    public bool AnySavesExist()
    {
        return saveSystem.AnySavesExist();
    }
    /// <summary>
    /// Loads the latest savegame
    /// </summary>
    public void LoadLatestSave()
    {
        Debug.Log("Load latest save");
        saveSystem.LoadLatestSave(ref gameData);
        ChangeScene(gameData.prevScene);
    }
    #endregion SaveSystemInterface
    #endregion //Public

    #region Private
    #region UnityCallbacks
    private void Awake()
    {
        #if FULL_DEBUG
        //Debug.Log("GameController Awake");
        #endif

        saveSystem = new GameSaveSystem();
            //saveFields.fileExtension, saveFields.saveDirectory, saveFields.fileName_SavesList, 
            //saveFields.autoSaveFileName,saveFields.quickSaveName,
            //saveFields.numAutoSaves, saveFields.numQuickSaves, saveFields.numNormalSaves);

        //empty game data - will be filled up upon loading an autosave
        gameData = new GameData();
        gameData.prevScene = GameConfig.GetSceneEnum(Application.loadedLevelName);
        //Debug.Log("autosave: " + autosaveFileName);

        if (gameData.prevScene == GameScene.MainMenu)
        {
            #if FULL_DEBUG
            Debug.Log("In Main Menu - not loading");
            #endif
        }
        else
        {
            //attempt to load the latest autosave
            if (saveSystem.LoadLatestSave(ref gameData))
            {
                #if FULL_DEBUG
                Debug.Log("Game Data loaded successfully");
                #endif
            }
            else
            {
                #if !NO_DEBUG
                Debug.LogWarning("No AutoSave found, new GameData created");
                #endif
                gameData = new GameData(gameData.prevScene, gameData.prevScene);
            }
        }
    }//Awake

    /// <summary>
    /// Unity call back after a scene is loaded
    /// Raises the PostSceneChange event
    /// </summary>
    /// <param name="levelID"></param>
    private void OnLevelWasLoaded(int levelID)
    {
        #if FULL_DEBUG
        //Debug.Log("GameController level loaded");
        #endif

        if (gameData.prevScene == GameScene.MainMenu)
        {
            #if FULL_DEBUG
            Debug.Log("Transitioning from MainMenu - no SceneChange events");
            #endif
        }
        else if(gameData.prevScene == gameData.nextScene)
        {
            #if FULL_DEBUG
            Debug.Log("Scene did not change - no SceneChangeEvents");
            #endif
        }
        else
        {
            #if !NO_DEBUG
            Debug.Log("Post Scene Change: from " + gameData.prevScene + " to " + gameData.nextScene);
            #endif

            //notifies all systems that a new scene has loaded
            OnPostSceneChange(new SceneChangeArgs(gameData.prevScene, gameData.nextScene));
        }
        gameData.prevScene = gameData.nextScene;
    }
    private void OnApplicationQuit()
    {
        #if !NO_DEBUG
        Debug.Log("Application Quit");
        #endif
        //if (gameData.prevScene != GameScene.MainMenu)
        {
            OnQuit();
            saveSystem.AutoSave(gameData); 
        }
    }
    private void Start()
    {
        #if FULL_DEBUG
        //Debug.Log("GameController Start");
        #endif
        //InputManager.Instance.RegisterKeysDown(KeyDown, KeyCode.F5, KeyCode.F9);
    }
    #endregion UnityCallbacks

    private void KeyDown(KeyCode key)
    {
        switch (key)
        {
            case KeyCode.F5:
                QuickSave();
                break;
            case KeyCode.F9:
                QuickLoad();
                break;
        }
    }

    /// <summary>
    /// Performs a quicksave of the current game state
    /// </summary>
    public void QuickSave()
    {
        #if FULL_DEBUG
        Debug.Log("quick save");
        #endif
        saveSystem.QuickSave(gameData);
    }
    private void QuickLoad()
    {
        #if FULL_DEBUG
        Debug.Log("quick load");
        #endif
        if(saveSystem.LoadQuickSave(ref gameData))
        {
            
        }
        else
        {
            #if FULL_DEBUG
            Debug.Log("no quick saves found");
            #endif
        }
    }
    #endregion //Private methods

    #endregion //Methods
}

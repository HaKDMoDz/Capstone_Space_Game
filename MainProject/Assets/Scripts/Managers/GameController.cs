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

    #endregion //EditorExposed

    #region InternalFields
    private GameScene currentScene;
    public GameScene CurrentScene
    {
        get { return currentScene; }
    }
    private Dictionary<GameScene, string> sceneEnumToNameTable;
    //need this for now - until Button's onClick event can pass in enums
    private Dictionary<string, GameScene> sceneNameToEnumTable;

    static int count = 0;
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
        ChangeScene(sceneNameToEnumTable[sceneName]);
    }

    public void ChangeScene(GameScene nextScene)
    {

        #if FULL_DEBUG || LOW_DEBUG || RELEASE
        Debug.Log("Scene changing from " + currentScene + " to " + nextScene);
        #endif
        #if FULL_DEBUG || LOW_DEBUG
        Debug.Log("PreSceneChange event raised");
        #endif

        OnPreSceneChange(new SceneChangeArgs(currentScene, nextScene));

        Application.LoadLevel(sceneEnumToNameTable[nextScene]);

        //OnPostSceneChange(new SceneChangeArgs(currentScene, nextScene));
    }
    #endregion //Public

    #region Private
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        count++;
        if(count > 1)
        {
            #if FULL_DEBUG
            Debug.Log("More than 1 Game Controller found - destroying");
            #endif
            Destroy(gameObject);
        }
        #if FULL_DEBUG
        Debug.Log("GameController Awake");
        #endif
        #if FULL_DEBUG || LOW_DEBUG || RELEASE
        if (sceneEntryList.Count == 0)
        {
            Debug.LogError("No Scene Name Entries provided");
            return;
        }
        #endif
        currentScene = defaultStartScene;
        sceneEnumToNameTable = sceneEntryList.ToDictionary(s => s.gameScene, s => s.sceneName);
        //need this for now - until Button's onClick event can pass in enums
        sceneNameToEnumTable = sceneEntryList.ToDictionary(s => s.sceneName, s => s.gameScene);
    }
    void OnLevelWasLoaded(int levelID)
    {
        #if FULL_DEBUG          
        Debug.Log("Level loaded " + Application.loadedLevelName);
        #endif
    }

    #endregion //Private methods

    #endregion //Methods
}

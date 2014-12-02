using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

#region AdditionalStructures
public enum GameScene { MainMenu, GalaxyMap, CombatScene, ShipDesignScene }

[Serializable]
public struct SceneNameEntry
{
    public GameScene gameScene;
    public string sceneName;
}
#endregion//Additional Structures

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
    #endregion //Internal

    #region Events
    public delegate void PreSceneChange(SceneChangeArgs args);
    public event PreSceneChange OnPreSceneChange = new PreSceneChange((SceneChangeArgs) => { });

    public delegate void PostSceneChange(SceneChangeArgs args);
    public event PostSceneChange OnPostSceneChange = new PostSceneChange((SceneChangeArgs) => { });
    #endregion //Events

    #endregion //Fields

    #region Methods
    void Awake()
    {
#if FULL_DEBUG
        Debug.Log("GameController Awake");
#endif
#if FULL_DEBUG || LOW_DEBUG || RELEASE
        if(sceneEntryList.Count == 0)
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

    #endregion //Methods
}

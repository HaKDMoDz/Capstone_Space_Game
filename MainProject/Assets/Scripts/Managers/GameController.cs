using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;


public enum GameScene { MainMenu, GalaxyMap, CombatScene, ShipDesignScene }
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
    private Dictionary<GameScene, string> sceneNameTable;
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
        sceneNameTable = sceneEntryList.ToDictionary(s => s.gameScene, s => s.sceneName);

    }

    public void ChangeScene(GameScene nextScene)
    {
#if FULL_DEBUG
        Debug.Log("Scene changing from " + currentScene + " to " + nextScene);
#endif
        OnPreSceneChange(new SceneChangeArgs(currentScene, nextScene));

        Application.LoadLevel(sceneNameTable[nextScene]);

        //OnPostSceneChange(new SceneChangeArgs(currentScene, nextScene));
    }

    #endregion //Methods
}
[Serializable]
public class SceneNameEntry
{
    public GameScene gameScene;
    public string sceneName;
}
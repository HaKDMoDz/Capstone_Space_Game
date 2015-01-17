using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class GameConfig : ScriptableObject
{
    [SerializeField]
    private List<SceneNameEntry> sceneEntryList;

    public static Dictionary<GameScene, string> sceneEnumToNameTable { get; private set; }
    //need this for now - until Button's onClick event can pass in enums
    public static Dictionary<string, GameScene> sceneNameToEnumTable { get; private set; }

    public static GameScene GetSceneEnum(string sceneName)
    {
        return sceneNameToEnumTable[sceneName];
    }
    public static string GetSceneName(GameScene gameScene)
    {
        return sceneEnumToNameTable[gameScene];
    }

    private void OnEnable()
    {
        if(sceneEntryList.Count==0)
        {
            Debug.LogError("No scene entries found");
        }
        sceneEnumToNameTable = sceneEntryList.ToDictionary(s => s.gameScene, s => s.sceneName);
        sceneNameToEnumTable = sceneEntryList.ToDictionary(s => s.sceneName, s => s.gameScene);
    }
	
}
[Serializable]
public struct SceneNameEntry
{
    public GameScene gameScene;
    public string sceneName;
}
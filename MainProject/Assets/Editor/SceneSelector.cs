using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
public class SceneSelector : EditorWindow 
{
    private const string menuName = "Open Scene";
    
    private string scenesFolder = Application.dataPath + "/Assets/Scenes";

    private IEnumerable<string> sceneNames;
    private Vector2 scroll;

    [MenuItem(menuName + "/Main Menu")]
    public static void OpenMainMenu()
    {
        OpenScene("MainMenu");
    }
    [MenuItem(menuName + "/Galaxy Map")]
    public static void OpenGalaxyMap()
    {
        OpenScene("GalaxyMap");
    }
    [MenuItem(menuName + "/Ship Design Scene")]
    public static void OpenShipDesignScene()
    {
        OpenScene("ShipDesignScene");
    }
    [MenuItem(menuName + "/Combat Scene")]
    public static void OpenCombatScene()
    {
        OpenScene("CombatScene");
    }
    [MenuItem(menuName + "/Test Scene")]
    public static void OpenTestScene()
    {
        OpenScene("TestScene");
    }
    [MenuItem(menuName + "/Particle Workshop")]
    public static void ParticleWorkshop()
    {
        OpenScene("ParticleWorkshop");
    }
    static void OpenScene(string name)
    {
        if(EditorApplication.SaveCurrentSceneIfUserWantsTo())
        {
            EditorApplication.OpenScene("Assets/Scenes/" + name + ".unity");
        }
    }
    [MenuItem(menuName+"/Scene Selector")]
    private static void OpenSceneSelector()
    {
        EditorWindow.GetWindow(typeof(SceneSelector));
    }
    private void FindInScenesDir()
    {
        string dir = scenesFolder;
        Debug.Log("Searching for scenes in directory " + dir);
        var info = new DirectoryInfo(dir);
        sceneNames = info.GetFiles()
            .Select(f => f.Name)
            .Where(f => f.Contains(".unity") && !f.Contains(".meta"));
    }
    private void OnGUI()
    {
        if(sceneNames==null || sceneNames.Count()==0)
        {
            FindInScenesDir();
        }

        scroll = GUILayout.BeginScrollView(scroll);

        if (sceneNames == null || sceneNames.Count() == 0)
        {
            Debug.LogError("No scenes found");
        }
        else
        {
            foreach (string scene in sceneNames)
            {
                string sceneName = scene.Replace(Application.dataPath, "");
                sceneName = scene.Replace(".unity", "");
                if (GUILayout.Button(sceneName))
                {
                    if (EditorApplication.SaveCurrentSceneIfUserWantsTo())
                    {
                        EditorApplication.OpenScene(scenesFolder + "/" + scene);
                    }
                }
            }
        }
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        if(GUILayout.Button("Search in Scenes folder"))
        {
            Debug.Log("Searching for scene files in " + scenesFolder);
            FindInScenesDir();
        }

        GUILayout.EndScrollView();
    }
    private void Awake()
    {
        scenesFolder = Application.dataPath + "/Scenes";
    }
    
}

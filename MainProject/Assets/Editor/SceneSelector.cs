using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class SceneSelector : Editor 
{
    const string menuName = "Open Scene";

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

    static void OpenScene(string name)
    {
        if(EditorApplication.SaveCurrentSceneIfUserWantsTo())
        {
            EditorApplication.OpenScene("Assets/Scenes/" + name + ".unity");
        }
    }
}

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class SceneSelector : Editor 
{
    const string menuName = "Open Scene";

    [MenuItem(menuName + "/Ship Design Scene")]
    public static void OpenShipDesignScene()
    {
        OpenScene("ShipDesignScene");
    }

    [MenuItem(menuName + "/Test Scene")]
    public static void OpenTestScene()
    {
        OpenScene("TestScene");
    }

    static void OpenScene(string name)
    {
        if(EditorApplication.SaveCurrentSceneIfUserWantsTo())
        {
            EditorApplication.OpenScene("Assets/Scenes/" + name + ".unity");
        }
    }
}

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;

public class ScriptableObjectCreator : EditorWindow
{
    private List<Type> soTypes;
    private Vector2 scroll;

    [MenuItem("Assets/Create/ScriptableObject")]
    private static void CreateScriptableObject()
    {
        EditorWindow.GetWindow(typeof(ScriptableObjectCreator));
    }

    private static bool IsValidType(Type type)
    {
        return !type.IsAbstract &&
            type.IsSubclassOf(typeof(ScriptableObject)) &&
            !type.IsSubclassOf(typeof(Editor));
            
    }

    private void Awake()
    {
        soTypes = new List<Type>();
        Assembly currentAssembly = Assembly.GetAssembly(typeof(HullTable));
        foreach (Type type in currentAssembly.GetTypes())
        {
            if (IsValidType(type))
                soTypes.Add(type);
        }
    }

    private void OnGUI()
    {
        if(soTypes == null || soTypes.Count==0)
        {
            return;
        }
        scroll = GUILayout.BeginScrollView(scroll);
        
        foreach (Type type in soTypes)
        {
            if(GUILayout.Button(type.Name))
            {
                ScriptableObject asset = ScriptableObject.CreateInstance(type);
                string path = AssetDatabase.GenerateUniqueAssetPath("Assets/Database/" + type.Name + ".asset");
                AssetDatabase.CreateAsset(asset, path);
                EditorGUIUtility.PingObject(asset);
            }
        }
        GUILayout.EndScrollView();

    }
	
}

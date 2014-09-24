using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;

[CustomEditor(typeof(HullTable))]
public class HullTableEditor : Editor
{
    int id;
    GameObject hull;

    [MenuItem("Data/Create Hull Table")]
    static void CreateHullTable()
    {
        string path = EditorUtility.SaveFilePanel("Create Hull Tabl", "Assets/", "default.asset", "asset");
        if (path == "")
        {
            return;
        }
        path = FileUtil.GetProjectRelativePath(path);

        HullTable hullTable = CreateInstance<HullTable>();
        AssetDatabase.CreateAsset(hullTable, path);
        AssetDatabase.SaveAssets();
    }


    public override void OnInspectorGUI()
    {
        //if(Event.current.type==EventType.Layout)
        //{
        //    return;
        //}

        DrawDefaultInspector();

        HullTable hullTable = target as HullTable;

        //EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("Add new entry to Hull Table");
        
        id = EditorGUILayout.IntField("ID", id);
        if (hullTable.IDExists(id))
        {
            EditorGUILayout.HelpBox("ID already exists in table", MessageType.Error, true);     
        }
        EditorGUILayout.LabelField("Hull Prefab");
        hull = EditorGUILayout.ObjectField(hull, typeof(GameObject), true) as GameObject;
        
        if(!hull)
        {
            EditorGUILayout.HelpBox("Please assign a hull", MessageType.Info, true);
        }
        else if(hullTable.PrefabExists(hull))
        {
            EditorGUILayout.HelpBox("Hull already exists in table", MessageType.Warning, true);     
        }
        //EditorGUILayout.EndHorizontal();
        if(GUILayout.Button("Add Entry"))
        {
            if (hull)
            {
                hullTable.AddEntry(id, hull);
            }
            else
            {
                EditorGUILayout.HelpBox("No hull assigned", MessageType.Error, true);
                Debug.LogError("no hull assigned", this);
            }
            
        }
        if(GUILayout.Button("Display Table"))
        {
            hullTable.DisplayTable();
        }

        if(GUILayout.Button("Wipe Table"))
        {
            if(EditorUtility.DisplayDialog("Confirm Wipe", "Are you sure you want to wipe the hull table?", "Wipe", "Cancel"))
            {
                hullTable.WipeTable();
            }
        }


    }
}

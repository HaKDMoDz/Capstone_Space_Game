using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(HullTable))]
public class HullTableEditor : Editor
{
    int id;
    Hull hull;

    [MenuItem("Data/Create Hull Table")]
    static void CreateHullTable()
    {
        string path = EditorUtility.SaveFilePanel("Create Hull Table", "Assets/", "HullTable.asset", "asset");
        if (path == "")
        {
            return;
        }
        path = FileUtil.GetProjectRelativePath(path);

        HullTable hullTable = CreateInstance<HullTable>();
        AssetDatabase.CreateAsset(hullTable, path);
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = hullTable;
    }


    public override void OnInspectorGUI()
    {
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
        hull = EditorGUILayout.ObjectField(hull, typeof(Hull), true) as Hull;
        
        if(!hull)
        {
            EditorGUILayout.HelpBox("Please assign a hull", MessageType.Info, true);
        }
        else if(hullTable.HullExists(hull))
        {
            EditorGUILayout.HelpBox("Hull already exists in table", MessageType.Warning, true);     
        }
        //EditorGUILayout.EndHorizontal();
        if(GUILayout.Button("Add Entry"))
        {
            if (hull)
            {
                hullTable.AddEntry(id, hull);
                EditorUtility.SetDirty(hullTable);
            }
            else
            {
                EditorGUILayout.HelpBox("No hull assigned", MessageType.Error, true);
                Debug.LogError("no hull assigned", this);
            }
            
        }
        //if(GUILayout.Button("Display Table"))
        //{
        //    hullTable.DisplayTable();
        //}

        if(GUILayout.Button("Wipe Table"))
        {
            if(EditorUtility.DisplayDialog("Confirm Wipe", "Are you sure you want to wipe the hull table?", "Wipe", "Cancel"))
            {
                hullTable.WipeTable();
                EditorUtility.SetDirty(hullTable);
            }
        }


    }
}

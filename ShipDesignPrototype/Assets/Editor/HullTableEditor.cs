using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;

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
        //DrawDefaultInspector();

        HullTable hullTable = target as HullTable;
        float PosY = 50f;
        
        EditorGUI.LabelField(new Rect(0f, PosY, Screen.width * .25f, EditorGUIUtility.singleLineHeight), "ID");
        EditorGUI.LabelField(new Rect(Screen.width * .26f, PosY, Screen.width * .7f, EditorGUIUtility.singleLineHeight), "Hull");
        foreach (HullTableEntry entry in hullTable.HullTableProp)
        {
            PosY += EditorGUIUtility.singleLineHeight ;
            EditorGUI.IntField(new Rect(0f, PosY, Screen.width * .25f, EditorGUIUtility.singleLineHeight), entry.ID);
            EditorGUI.ObjectField(new Rect(Screen.width * .26f, PosY, Screen.width * .70f, EditorGUIUtility.singleLineHeight), entry.hull, typeof(Hull));
            

        }
        
        for (int i = 0; i < 5+hullTable.HullTableProp.Count * EditorGUIUtility.singleLineHeight/6; i++)
        {
            EditorGUILayout.Space();
        }
        
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
        
        if (GUILayout.Button("Auto Generate ID and Add"))
        {
            if (hull)
            {
                hullTable.AutoGenIDAndAdd(hull);
                EditorUtility.SetDirty(hullTable);
                Clear();
            }
            else
            {
                EditorGUILayout.HelpBox("No hull assigned", MessageType.Error, true);
                Debug.LogError("No hull assigned", this);
            }
        }
        if(GUILayout.Button("Add Entry"))
        {
            if (hull)
            {
                hullTable.AddEntry(id, hull);
                EditorUtility.SetDirty(hullTable);
                Clear();
            }
            else
            {
                EditorGUILayout.HelpBox("No hull assigned", MessageType.Error, true);
                Debug.LogError("No hull assigned", this);
            }
            
        }
        
        if(GUILayout.Button("Wipe Table"))
        {
            if(EditorUtility.DisplayDialog("Confirm Wipe", "Are you sure you want to wipe the hull table?", "Wipe", "Cancel"))
            {
                hullTable.WipeTable();
                EditorUtility.SetDirty(hullTable);
                Clear();
            }
        }


    }
    void Clear()
    {
        HullTable hullTable = target as HullTable;
        id = hullTable.GenNextID(); ;
        hull = null;
    }
}

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

    Vector3 rectXPos = new Vector3(0.09f, 0.19f, 0.75f);
    float spacing = 0.02f;
    [MenuItem("Custom/Database/Create Hull Table")]
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

        if (!hullTable)
        { return; }

        float PosY = 50f;

        EditorGUI.LabelField(new Rect(0f, PosY, Screen.width * .25f, EditorGUIUtility.singleLineHeight), "ID");
        EditorGUI.LabelField(new Rect(Screen.width * .26f, PosY, Screen.width * .7f, EditorGUIUtility.singleLineHeight), "Hull");

        if(hullTable.Hull_id_List==null)
        {
            Debug.Log("hull table null");
        }

        for (int i = 0; i < hullTable.Hull_id_List.Count; i++)
        {
            HullTableEntry entry = hullTable.Hull_id_List[i];
            PosY += EditorGUIUtility.singleLineHeight ;
            if (GUI.Button(new Rect(0.0f, PosY, Screen.width * rectXPos.x, EditorGUIUtility.singleLineHeight), "X"))
            {
                hullTable.RemoveEntry(entry.ID);
                EditorUtility.SetDirty(hullTable);
            }
            EditorGUI.IntField(new Rect(Screen.width * (rectXPos.x + spacing), PosY, Screen.width * rectXPos.y, EditorGUIUtility.singleLineHeight), entry.ID);
            EditorGUI.ObjectField(new Rect(Screen.width * (rectXPos.y + spacing), PosY, Screen.width * rectXPos.z, EditorGUIUtility.singleLineHeight), entry.hull, typeof(Hull), true);
            
        }

        for (int i = 0; i < 5 + hullTable.Hull_id_List.Count * EditorGUIUtility.singleLineHeight / 6; i++)
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
        hull = EditorGUILayout.ObjectField(hull, typeof(Hull), false) as Hull;
        
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
                id = hullTable.GenNextID();
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
                id = hullTable.GenNextID();
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

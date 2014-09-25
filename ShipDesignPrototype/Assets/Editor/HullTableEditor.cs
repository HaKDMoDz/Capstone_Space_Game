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

    //ReorderableList orderedList;
    //SerializedProperty hullTable;
    //private static readonly GUIContent HULL_LIST_HEADER = new GUIContent("Hull List", "List of hulls");


    //void OnEnable()
    //{
    //    hullTable = serializedObject.FindProperty("hullTable");
    //    orderedList = new ReorderableList(serializedObject, hullTable, true, true, true, true);
    //    orderedList.drawHeaderCallback += rect => GUI.Label(rect, HULL_LIST_HEADER);
    //    orderedList.drawElementCallback += (rect, index, active, focused) =>
    //        {
    //            rect.height = 16;
    //            rect.y += 2;
    //            if (index >= hullTable.arraySize) return;
    //            var item = hullTable.GetArrayElementAtIndex(index).objectReferenceValue as HullTableEntry;
    //            if (item == null)
    //            {
    //                EditorGUI.LabelField(rect, "null");
    //                return;
    //            }
    //            EditorGUI.LabelField(rect, item.hull.name);
    //        };
    //}

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
        //serializedObject.Update();
        //orderedList.DoLayoutList();
        //serializedObject.ApplyModifiedProperties();

        HullTable hullTable = target as HullTable;

        //EditorGUILayout.BeginHorizontal();
        for (int i = 0; i < 5; i++)
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
        //EditorGUILayout.EndHorizontal();
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
        id = 0;
        hull = null;
    }
}

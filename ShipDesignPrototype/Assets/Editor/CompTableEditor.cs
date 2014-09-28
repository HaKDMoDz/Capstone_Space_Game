using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(ComponentTable))]
public class CompTableEditor : Editor
{
    int id;
    ShipComponent component;

    [MenuItem("Data/Create Component Table")]
    static void CreateCompTable()
    {
        string path = EditorUtility.SaveFilePanel("Create Component Table", "Assets/", "CompTable.asset", "asset");
        if (path == "")
        {
            return;
        }
        path = FileUtil.GetProjectRelativePath(path);
        ComponentTable compTable = CreateInstance<ComponentTable>();
        AssetDatabase.CreateAsset(compTable, path);
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = compTable;
    }

    public override void OnInspectorGUI()
    {
        //DrawDefaultInspector();
        ComponentTable compTable = target as ComponentTable;
        float PosY = 50f;

        EditorGUI.LabelField(new Rect(0f, PosY, Screen.width * .25f, EditorGUIUtility.singleLineHeight), "ID");
        EditorGUI.LabelField(new Rect(Screen.width * .26f, PosY, Screen.width * .7f, EditorGUIUtility.singleLineHeight), "Hull");
        foreach (CompTableEntry entry in compTable.CompTable)
        {
            PosY += EditorGUIUtility.singleLineHeight;
            EditorGUI.IntField(new Rect(0f, PosY, Screen.width * .25f, EditorGUIUtility.singleLineHeight), entry.ID);
            EditorGUI.ObjectField(new Rect(Screen.width * .26f, PosY, Screen.width * .70f, EditorGUIUtility.singleLineHeight), entry.component, typeof(ShipComponent));


        }

        for (int i = 0; i < 5 + compTable.CompTable.Count * EditorGUIUtility.singleLineHeight / 6; i++)
        {
            EditorGUILayout.Space();
        }
        

        EditorGUILayout.LabelField("Add new entry");
        id = EditorGUILayout.IntField("ID", id);
        if(compTable.IDExists(id))
        {
            EditorGUILayout.HelpBox("ID already exists in table", MessageType.Error, true);
        }
        EditorGUILayout.LabelField("Component Prefab");
        component = EditorGUILayout.ObjectField(component, typeof(ShipComponent), true) as ShipComponent;

        if(!component)
        {
            EditorGUILayout.HelpBox("Please assign a component", MessageType.Info, true);
        }
        else if (compTable.ComponentExists(component))
        {
            EditorGUILayout.HelpBox("Component already exists in table", MessageType.Warning, true);
        }

        if (GUILayout.Button("Auto Generate ID and Add"))
        {
            if (component)
            {
                compTable.AutoGenIDAndAdd(component);
                EditorUtility.SetDirty(compTable);
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
            if(component)
            {
                compTable.AddEntry(id, component);
                EditorUtility.SetDirty(compTable);
                Clear();
            }
            else
            {
                EditorGUILayout.HelpBox("No component Assigned", MessageType.Error, true);
                Debug.LogError("No component Assigned", this);
            }
        }

        if(GUILayout.Button("Wipe Table"))
        {
            if(EditorUtility.DisplayDialog("Confirm Wipe", "Are you sure you want to wipe the component table?", "Wipe","Cancel"))
            {
                Clear();
                compTable.WipeTable();
                EditorUtility.SetDirty(compTable);
            }
        }

    }

    void Clear()
    {
        ComponentTable compTable = target as ComponentTable;
        id = compTable.GenNextID(); ;
        component = null;
    }
}

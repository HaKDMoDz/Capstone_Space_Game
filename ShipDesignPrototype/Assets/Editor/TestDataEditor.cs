using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(ComponentTable))]
public class ComponentTableEditor : Editor 
{
    int id;
    ShipComponent comp;
    
    [MenuItem("Data/Create Component Table")]
    static void CreateTestData()
    {
        string path = EditorUtility.SaveFilePanel("Create Component Table", "Assets/", "ComponentTable.asset", "asset");
        if(path=="")
        {
            return;
        }
        path = FileUtil.GetProjectRelativePath(path);
        ComponentTable testData = CreateInstance<ComponentTable>();
        AssetDatabase.CreateAsset(testData, path);
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = testData;

    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        ComponentTable compTable = target as ComponentTable;

        EditorGUILayout.LabelField("Add entry to Component Table");

        id = EditorGUILayout.IntField("num", id);
        if(compTable.IDExists(id))
        {
            EditorGUILayout.HelpBox("ID already exists in table", MessageType.Error, true);
        }
        comp = EditorGUILayout.ObjectField("Component", comp, typeof(ShipComponent), true) as ShipComponent;
        if(!comp)
        {
            EditorGUILayout.HelpBox("Please assign a component", MessageType.Info, true);
        }
        else if(compTable.ComponentExists(comp))
        {
            EditorGUILayout.HelpBox("Component already exists in table", MessageType.Warning, true);
        }
        if(GUILayout.Button("Auto Generate ID and Add"))
        {
            if(comp)
            {
                compTable.AutoGenIDandAdd(comp);
                EditorUtility.SetDirty(compTable);
                Clear();            
            }
            else
            {
                EditorGUILayout.HelpBox("No Component assigned", MessageType.Error, true);
                Debug.LogError("No Component Assigned", this);
            }
        }

        if(GUILayout.Button("Add Entry"))
        {
            if (comp)
            {
                compTable.AddEntry(id, comp);
                EditorUtility.SetDirty(compTable);
                Clear();
            }
            else
            {
                EditorGUILayout.HelpBox("No Component assigned", MessageType.Error, true);
                Debug.LogError("No Component Assigned", this);
            }
        }
        if(GUILayout.Button("Wipe Table"))
        {
            if(EditorUtility.DisplayDialog("Confirm Wipe", "Are you sure you want to wipe the Component Table?","Wipe","Cancel"))
            {
                compTable.WipeTable();
                EditorUtility.SetDirty(compTable);
                Clear();
            }
        }

    }
    void Clear()
    {
        ComponentTable compTable = target as ComponentTable;
        id = compTable.GenID();
        comp = null;
    }
}

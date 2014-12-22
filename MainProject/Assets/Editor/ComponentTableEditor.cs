using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;

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
        //DrawDefaultInspector();

        ComponentTable compTable = target as ComponentTable;

        float posY = 50f;
        if (compTable.Comp_id_List != null)
        {
            EditorGUI.LabelField(new Rect(0f, posY, Screen.width * 0.25f, EditorGUIUtility.singleLineHeight), "ID");
            EditorGUI.LabelField(new Rect(Screen.width * 0.26f, posY, Screen.width * 0.7f, EditorGUIUtility.singleLineHeight), "Component");

            //if(compTable.ComponentList.Where(entry=>entry.component is Component_Weapon).Count()>0)
            if (compTable.Comp_id_List.Where(entry => entry.component.CompType == ComponentType.Weapon).Count() > 0)
            {
                posY += EditorGUIUtility.singleLineHeight;
                EditorGUI.LabelField(new Rect(0f, posY, Screen.width, EditorGUIUtility.singleLineHeight), "Weapons");
                foreach (ComponentTableEntry entry in compTable.Comp_id_List.Where(entry => entry.component.CompType == ComponentType.Weapon))
                {
                    posY+=EditorGUIUtility.singleLineHeight;
                    EditorGUI.IntField(new Rect(0f, posY, Screen.width*0.25f, EditorGUIUtility.singleLineHeight),entry.ID);
                    EditorGUI.ObjectField(new Rect(Screen.width*.26f,posY, Screen.width,EditorGUIUtility.singleLineHeight),entry.component,typeof(ShipComponent),true);
                }
            }

            if (compTable.Comp_id_List.Where(entry => entry.component.CompType == ComponentType.Defense).Count() > 0)
            {
                posY += EditorGUIUtility.singleLineHeight;
                EditorGUI.LabelField(new Rect(0f, posY, Screen.width, EditorGUIUtility.singleLineHeight), "Defenses");
                foreach (ComponentTableEntry entry in compTable.Comp_id_List.Where(entry => entry.component.CompType == ComponentType.Defense))
                {
                    posY += EditorGUIUtility.singleLineHeight;
                    EditorGUI.IntField(new Rect(0f, posY, Screen.width * 0.25f, EditorGUIUtility.singleLineHeight), entry.ID);
                    EditorGUI.ObjectField(new Rect(Screen.width * .26f, posY, Screen.width, EditorGUIUtility.singleLineHeight), entry.component, typeof(ShipComponent), true);
                }
            }
            if (compTable.Comp_id_List.Where(entry => entry.component.CompType == ComponentType.Power).Count() > 0)
            {
                posY += EditorGUIUtility.singleLineHeight;
                EditorGUI.LabelField(new Rect(0f, posY, Screen.width, EditorGUIUtility.singleLineHeight), "Power");
                foreach (ComponentTableEntry entry in compTable.Comp_id_List.Where(entry => entry.component.CompType == ComponentType.Power))
                {
                    posY += EditorGUIUtility.singleLineHeight;
                    EditorGUI.IntField(new Rect(0f, posY, Screen.width * 0.25f, EditorGUIUtility.singleLineHeight), entry.ID);
                    EditorGUI.ObjectField(new Rect(Screen.width * .26f, posY, Screen.width, EditorGUIUtility.singleLineHeight), entry.component, typeof(ShipComponent), true);
                }
            }
            if (compTable.Comp_id_List.Where(entry => entry.component.CompType == ComponentType.Support).Count() > 0)
            {
                posY += EditorGUIUtility.singleLineHeight;
                EditorGUI.LabelField(new Rect(0f, posY, Screen.width, EditorGUIUtility.singleLineHeight), "Support");
                foreach (ComponentTableEntry entry in compTable.Comp_id_List.Where(entry => entry.component.CompType == ComponentType.Support))
                {
                    posY += EditorGUIUtility.singleLineHeight;
                    EditorGUI.IntField(new Rect(0f, posY, Screen.width * 0.25f, EditorGUIUtility.singleLineHeight), entry.ID);
                    EditorGUI.ObjectField(new Rect(Screen.width * .26f, posY, Screen.width, EditorGUIUtility.singleLineHeight), entry.component, typeof(ShipComponent), true);
                }
            }


            for (int i = 0; i < 15 + compTable.Comp_id_List.Count * EditorGUIUtility.singleLineHeight / 6; i++)
            {
                EditorGUILayout.Space();
            }
        }



        EditorGUILayout.LabelField("Add entry to Component Table");

        id = EditorGUILayout.IntField("num", id);
        if(compTable.IDExists(id))
        {
            EditorGUILayout.HelpBox("ID already exists in table", MessageType.Error, true);
        }
        comp = EditorGUILayout.ObjectField("Component", comp, typeof(ShipComponent), false) as ShipComponent;
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

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
    IEnumerable<ComponentTableEntry> weapons;
    IEnumerable<ComponentTableEntry> defenses;
    IEnumerable<ComponentTableEntry> engineering;
    IEnumerable<ComponentTableEntry> supports;

    Vector3 rectXPos = new Vector3(0.09f, 0.19f, 0.75f);
    float spacing = 0.02f;

    [MenuItem("Custom/Database/Create Component Table")]
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

            weapons = compTable.Comp_id_List.Where(entry => entry.component.CompType == ComponentType.Weapon);
            defenses = compTable.Comp_id_List.Where(entry => entry.component.CompType == ComponentType.Defense);
            engineering = compTable.Comp_id_List.Where(entry => entry.component.CompType == ComponentType.Engineering);
            supports = compTable.Comp_id_List.Where(entry => entry.component.CompType == ComponentType.Support);

            if (weapons.Count() > 0)
            {
                posY += EditorGUIUtility.singleLineHeight;
                EditorGUI.LabelField(new Rect(0f, posY, Screen.width, EditorGUIUtility.singleLineHeight), "Weapons");
                for (int i = 0; i < weapons.Count(); i++)
                {
                    ComponentTableEntry entry = weapons.ElementAt(i) ;
                    posY+=EditorGUIUtility.singleLineHeight;
                    if(GUI.Button(new Rect(0.0f, posY, Screen.width*rectXPos.x, EditorGUIUtility.singleLineHeight),"X"))
                    {
                        compTable.RemoveEntry(entry.ID);
                        EditorUtility.SetDirty(compTable);
                    }
                    EditorGUI.IntField(new Rect(Screen.width*(rectXPos.x+spacing), posY, Screen.width*rectXPos.y, EditorGUIUtility.singleLineHeight),entry.ID);
                    EditorGUI.ObjectField(new Rect(Screen.width*(rectXPos.y+spacing),posY, Screen.width*rectXPos.z,EditorGUIUtility.singleLineHeight),entry.component,typeof(ShipComponent),true);
                }
            }

            if (defenses.Count() > 0)
            {
                posY += EditorGUIUtility.singleLineHeight;
                EditorGUI.LabelField(new Rect(0f, posY, Screen.width, EditorGUIUtility.singleLineHeight), "Defenses");
                for (int i = 0; i < defenses.Count(); i++)
                {
                    ComponentTableEntry entry = defenses.ElementAt(i);
                    posY += EditorGUIUtility.singleLineHeight;
                    if (GUI.Button(new Rect(0.0f, posY, Screen.width * rectXPos.x, EditorGUIUtility.singleLineHeight), "X"))
                    {
                        compTable.RemoveEntry(entry.ID);
                    }
                    EditorGUI.IntField(new Rect(Screen.width * (rectXPos.x + spacing), posY, Screen.width * rectXPos.y, EditorGUIUtility.singleLineHeight), entry.ID);
                    EditorGUI.ObjectField(new Rect(Screen.width * (rectXPos.y + spacing), posY, Screen.width * rectXPos.z, EditorGUIUtility.singleLineHeight), entry.component, typeof(ShipComponent), true);
                }
            }
            if (engineering.Count() > 0)
            {
                posY += EditorGUIUtility.singleLineHeight;
                EditorGUI.LabelField(new Rect(0f, posY, Screen.width, EditorGUIUtility.singleLineHeight), "Engineering");
                for (int i = 0; i < engineering.Count(); i++)
                {
                    ComponentTableEntry entry = engineering.ElementAt(i);
                    posY += EditorGUIUtility.singleLineHeight;
                    if (GUI.Button(new Rect(0.0f, posY, Screen.width * rectXPos.x, EditorGUIUtility.singleLineHeight), "X"))
                    {
                        compTable.RemoveEntry(entry.ID);
                    }
                    EditorGUI.IntField(new Rect(Screen.width * (rectXPos.x + spacing), posY, Screen.width * rectXPos.y, EditorGUIUtility.singleLineHeight), entry.ID);
                    EditorGUI.ObjectField(new Rect(Screen.width * (rectXPos.y + spacing), posY, Screen.width * rectXPos.z, EditorGUIUtility.singleLineHeight), entry.component, typeof(ShipComponent), true);
                }
            }
            if (supports.Count() > 0)
            {
                posY += EditorGUIUtility.singleLineHeight;
                EditorGUI.LabelField(new Rect(0f, posY, Screen.width, EditorGUIUtility.singleLineHeight), "Support");
                for (int i = 0; i < supports.Count(); i++)
                {
                    ComponentTableEntry entry = supports.ElementAt(i);
                    posY += EditorGUIUtility.singleLineHeight;
                    if (GUI.Button(new Rect(0.0f, posY, Screen.width * rectXPos.x, EditorGUIUtility.singleLineHeight), "X"))
                    {
                        compTable.RemoveEntry(entry.ID);
                    }
                    EditorGUI.IntField(new Rect(Screen.width * (rectXPos.x + spacing), posY, Screen.width * rectXPos.y, EditorGUIUtility.singleLineHeight), entry.ID);
                    EditorGUI.ObjectField(new Rect(Screen.width * (rectXPos.y + spacing), posY, Screen.width * rectXPos.z, EditorGUIUtility.singleLineHeight), entry.component, typeof(ShipComponent), true);
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

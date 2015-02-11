using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(BlueprintTemplates))]
public class BPTemplatesEditor : Editor
{
    string bpName;
    [SerializeField]
    private List<bool> foldouts = new List<bool>();

    public override void OnInspectorGUI()
    {
        //DrawDefaultInspector();

        BlueprintTemplates bpTemplates = target as BlueprintTemplates;

        if (!bpTemplates)
        {
            Debug.LogError("Target is null or not BlueprintTemplates");
            return;
        }

        //Display Data
        //float posY = 50.0f;

        EditorGUILayout.BeginVertical();

        for (int i = 0; i < bpTemplates.BpTemplateList.Count; i++)
        {
            BlueprintTemplate bp = bpTemplates.BpTemplateList[i];
            //posY += EditorGUIUtility.singleLineHeight;

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("X"))
            {
                bpTemplates.RemoveBlueprint(bp.MetaData.BlueprintName);
                foldouts.RemoveAt(i);
            }
            EditorGUILayout.TextField(bp.MetaData.BlueprintName);
            EditorGUILayout.ObjectField(bp.Hull, typeof(Hull));

            EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel += 2;
            //Debug.Log("index " + i);
            //Debug.Log("foldouts.Count " + foldouts.Count);
            foldouts[i] = EditorGUILayout.Foldout(foldouts[i], "Blueprint Info");
            if (foldouts[i])
            {
                //EditorGUILayout.BeginVertical();
                EditorGUILayout.Space();
                EditorGUILayout.TextField("Excess Power: ", bp.MetaData.ExcessPower.ToString());
                EditorGUILayout.LabelField("Component List:");
                EditorGUILayout.LabelField("Slot Index          Component");
                foreach (var slotIndex_comp in bp.SlotIndex_Comp_List)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.TextArea(slotIndex_comp.slotIndex.ToString());
                    EditorGUILayout.ObjectField(slotIndex_comp.component, typeof(ShipComponent));
                    EditorGUILayout.EndHorizontal();
                }
                //EditorGUILayout.EndVertical();
            }
            EditorGUI.indentLevel-=2;
            EditorGUILayout.Space();
        }//for bps

        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Add New Blueprints: ");
        
        //Modification
        bpName = EditorGUILayout.TextField("Blueprint Name", bpName);

        if (bpName == "")
        {
            EditorGUILayout.HelpBox("Enter name of blueprint", MessageType.Info);
        }
        else if (bpTemplates.BlueprintExists(bpName))
        {
            EditorGUILayout.HelpBox("Duplicate name", MessageType.Error);
        }
        if (GUILayout.Button("Add Blueprint Template"))
        {
            if (bpName==null || bpName == "" )
            {
                Debug.LogWarning("Enter name of blueprint");
                return;
            }
            if (bpTemplates.BlueprintExists(bpName))
            {
                Debug.LogWarning("Duplicate name");
                return;
            }
            bpTemplates.AddBlueprint(bpName);
            foldouts.Add(false);
            bpName = "";
            EditorUtility.SetDirty(bpTemplates);
        }
        if (GUILayout.Button("Wipe Database"))
        {
            if (EditorUtility.DisplayDialog("Confirm Wipe", "Are you sure you want to wipe the hull table?", "Wipe", "Cancel"))
            {
                bpTemplates.Wipe();
                foldouts.Clear();
                EditorUtility.SetDirty(bpTemplates);
            }
        }

    }//OnInspectorGUI

    void OnEnable()
    {
        for (int i = 0; i < BlueprintTemplates.BlueprintTemplateList.Count; i++)
        {
            foldouts.Add(false);
        }    
    }
    

}

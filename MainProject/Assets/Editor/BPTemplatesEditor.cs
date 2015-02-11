using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(BlueprintTemplates))]
public class BPTemplatesEditor : Editor
{
    string bpName;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        BlueprintTemplates bpTemplates = target as BlueprintTemplates;

        if (!bpTemplates)
        {
            Debug.LogError("Target is null or not BlueprintTemplates");
            return;
        }

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
            bpName = "";
            EditorUtility.SetDirty(bpTemplates);
        }
        if (GUILayout.Button("Wipe Database"))
        {
            if (EditorUtility.DisplayDialog("Confirm Wipe", "Are you sure you want to wipe the hull table?", "Wipe", "Cancel"))
            {
                bpTemplates.Wipe();
                EditorUtility.SetDirty(bpTemplates);
            }
        }

    }

}

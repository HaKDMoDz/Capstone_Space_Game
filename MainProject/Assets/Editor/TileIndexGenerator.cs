using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class TileIndexGenerator : EditorWindow
{
    List<ComponentSlot> slots;

    [MenuItem("Custom/ShipHexGrid/Generate Tile Indices")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow<TileIndexGenerator>();
    }
    public void OnGUI()
    {
        if (GUILayout.Button("Assign Hex Tile Indices"))
        {
            AssignTileIndicesSelectedObjs();
        }
    }
    void AssignTileIndicesSelectedObjs()
    {
        GameObject[] selectedObjs= Selection.gameObjects;
        
        foreach (GameObject go in selectedObjs)
        {
            AssignIndices(go);
        }
    }

    void AssignIndices(GameObject go)
    {
        slots = new List<ComponentSlot>(go.GetComponentsInChildren<ComponentSlot>());

        if (slots == null || slots.Count < 1)
        {
            Debug.Log("No Component Slots found in " + go.name);
            return;
        }
        else
        {
            for (int i = 0; i < slots.Count; i++)
            {
                slots[i].index = i;
            }
            Debug.Log("Assigned "+slots.Count + " slot indices for " + go.name);
        }

    }

}

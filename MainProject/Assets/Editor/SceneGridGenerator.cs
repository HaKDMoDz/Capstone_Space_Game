using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class SceneGridGenerator : EditorWindow 
{
    public GameObject tilePrefab;
    public Vector2 gridSize;
    public Vector3 center;

    private List<GameObject> grid;
    private float tileSize;
    private GameObject gridParent;
    private Transform gridParentTrans;


    [MenuItem("SceneBuilder/Scene Grid Generator")]
    private static void ShowWindow()
    {
        EditorWindow.GetWindow<SceneGridGenerator>();
    }
    
    private void OnGUI()
    {
        tilePrefab = EditorGUILayout.ObjectField("Tile Prefab", tilePrefab, typeof(GameObject), false) as GameObject;
        gridSize = EditorGUILayout.Vector2Field("Grid Size", gridSize);
        center = EditorGUILayout.Vector3Field("Center", center);

        if(GUILayout.Button("Generate Grid"))
        {
            if(!tilePrefab)
            {
                Debug.LogError("Please assign a tile prefab");
                return;
            }
            if(tilePrefab.transform.localScale.x != tilePrefab.transform.localScale.y)
            {
                Debug.LogError("Tile should be square");
                return;
            }
            Init();
            GenerateGrid();
        }
        if(GUILayout.Button("Delete & Regenerate"))
        {
            if (!tilePrefab)
            {
                Debug.LogError("Please assign a tile prefab");
                return;
            }
            if (tilePrefab.transform.localScale.x != tilePrefab.transform.localScale.y)
            {
                Debug.LogError("Tile should be square");
                return;
            }
            Clear();
            Init();
            GenerateGrid();
        }
    }
    private void Init()
    {
        grid = new List<GameObject>();
        Mesh tileMesh = tilePrefab.GetComponent<MeshFilter>().sharedMesh;
        tileSize = tileMesh.bounds.size.x * tilePrefab.transform.localScale.x;
    }
    private void GenerateGrid()
    {
        if (!gridParent)
        {
            gridParent = new GameObject("Scene Grid");
            gridParentTrans = gridParent.transform;
        }
        Quaternion rotation = tilePrefab.transform.rotation;
        
        float gridWidth = gridSize.x * tileSize;
        float gridHeight = gridSize.y * tileSize;
        Vector3 startPos = Vector3.zero;
        startPos.x = center.x - gridWidth *0.5f;
        startPos.z = center.z - gridHeight * 0.5f;

        for (int i = 0; i < gridSize.x; i++)
        {
            for (int j = 0; j < gridSize.y; j++)
            {
                Vector3 tilePos = new Vector3();
                tilePos.x = startPos.x + i * tileSize;
                tilePos.z = startPos.z + j * tileSize;
                GameObject gridTile = Instantiate(tilePrefab, tilePos, rotation) as GameObject;
                gridTile.transform.SetParent(gridParentTrans, true);
                grid.Add(gridTile);
            }
        }
    }
    private void Clear()
    {
        for (int i = grid.Count-1; i >=0; i--)
        {
            DestroyImmediate(grid[i]);
        }
        grid.Clear();
    }
}

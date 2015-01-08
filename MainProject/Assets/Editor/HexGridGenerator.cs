using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class HexGridGenerator : EditorWindow 
{
    public GameObject tile;
    public Transform shipTransform;
    public Renderer renderer;
    public int shipLayer = 8;
    public bool deleteExtraTiles = true;

    private List<ComponentSlot> tiles;
    
    private Vector2 hexTileSize;
    private Vector3 shipSize;

    private Vector3 startPos;
    private float tileSpawnHeight;
    private float raycastHeight;
    private int tileGridWidth;
    private int tileGridHeight;

    [MenuItem("Custom/ShipHexGrid/Hex Grid Generator")]
    private static void ShowWindow()
    {
        EditorWindow.GetWindow<HexGridGenerator>();
    }

    private void Awake()
    {
        tiles = new List<ComponentSlot>();
    }
    public void OnGUI()
    {
        tile = EditorGUILayout.ObjectField("Tile Prefab", tile, typeof(GameObject),false) as GameObject;
        shipTransform = EditorGUILayout.ObjectField("Ship Transform", shipTransform, typeof(Transform), true) as Transform;
        renderer = EditorGUILayout.ObjectField("Ship Mesh Renderer", renderer, typeof(Renderer), true) as Renderer;
        shipLayer =  EditorGUILayout.IntField(new GUIContent("Ship Layer"), shipLayer);
        deleteExtraTiles =  EditorGUILayout.Toggle(new GUIContent("Delete Extra Tiles"), deleteExtraTiles);

        if (GUILayout.Button("Create Tile Grid"))
        {
            CreateTileGrid();
        }
    }

    private void Init()
    {

        tiles.Clear();
        hexTileSize.x = tile.renderer.bounds.size.x;
        hexTileSize.y = tile.renderer.bounds.size.z;

        shipSize.x = renderer.bounds.size.x;
        shipSize.y = renderer.bounds.size.y;
        shipSize.z = renderer.bounds.size.z;

        CalculateGridSize();

        tileSpawnHeight = shipTransform.transform.position.y - shipSize.y;
        raycastHeight = shipTransform.transform.position.y + shipSize.y * 4f;

        startPos = new Vector3(shipTransform.transform.position.x - shipSize.x / 2f, tileSpawnHeight,
                               shipTransform.transform.position.z + shipSize.z / 2f - hexTileSize.x / 2f);

    }
    private void CalculateGridSize()
    {
        //hexagon's side length is half the height
        float sideLength = hexTileSize.y / 2.0f;
        //the number of whole hex sides that fit inside inside ship's z length
        int numSides = Mathf.RoundToInt(shipSize.z / sideLength);

        tileGridHeight = Mathf.RoundToInt(numSides * 2f / 3f);
        tileGridWidth = Mathf.RoundToInt(shipSize.x / hexTileSize.x);

    }

    private void CreateTileGrid()
    {
        Init();

        GameObject shipTileMap = new GameObject("ComponentGrid");
        shipTileMap.transform.position = shipTransform.transform.position;
        Vector3 tilePos;
        for (int y = 0; y < tileGridHeight; y++)
        {
            for (int x = 0; x < tileGridWidth + y % 2; x++)
            {
                tilePos = GetWorldCoords(x, y);
                GameObject tileClone = Instantiate(tile, tilePos, tile.transform.rotation) as GameObject;
                tileClone.transform.SetParent(shipTileMap.transform, true);
                tiles.Add(tileClone.GetSafeComponent<ComponentSlot>());
                tilePos.x += hexTileSize.x * 2f;
            }
        }
        shipTileMap.transform.SetParent(shipTransform, true);

        if(deleteExtraTiles)
        {
            DeleteExtraTiles();
        }
        AssignSlotIndices();
    }
    private Vector3 GetWorldCoords(int xGridPos, int yGridPos)
    {
        float offset = 0f;
        if (yGridPos % 2 == 0)
        {
            offset = hexTileSize.x / 2f;
        }
        Vector3 tilePos = Vector3.zero;
        tilePos.x = startPos.x + xGridPos * hexTileSize.x + offset;
        tilePos.y = tileSpawnHeight;
        tilePos.z = startPos.z - yGridPos * hexTileSize.y * .75f;

        return tilePos;
    }
    private void DeleteExtraTiles()
    {
        Vector3 rayOrigin;
        Ray ray = new Ray();
        for (int i = tiles.Count - 1; i >= 0; i--)
        {
            rayOrigin = tiles[i].transform.position + Vector3.up * raycastHeight * 1.0f;
            ray.origin = rayOrigin;
            ray.direction = Vector3.down;
            if (!Physics.Raycast(ray, 500f, 1 << shipLayer))
            {
                DestroyImmediate(tiles[i].gameObject, false);
                tiles.RemoveAt(i);
            }
        }
    }
    private void AssignSlotIndices()
    {
        for (int i = 0; i < tiles.Count; i++)
        {
            tiles[i].index = i;
        }
    }
}

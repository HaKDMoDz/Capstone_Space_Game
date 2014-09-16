using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class ShipTileMapWizard : ScriptableWizard
{
    public GameObject tile;
    public MeshFilter shipMeshFilter;
    public Transform shipTransform;
    public int shipLayer;
    public float zRotation = 0f;
    

    Mesh shipMesh;
    Bounds shipBounds;
    List<GameObject> tiles;

    int tileGridWidth;
    int tileGridLength;
    float tileSpawnHeight;
    float raycastHeight;
    Vector3 startPos;
    int tileSize;

    [MenuItem("Custom/Ship Tilemap Wizard")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard<ShipTileMapWizard>("Ship Tilemap Wizard", "Create");

    }


    //called on clicking "Create" in the wizard
    void OnWizardCreate()
    {

        Init();
        CreateTileMap();
        DeleteExtraTiles();

    }

    //generates a tilemap for the entire ship mesh
    void CreateTileMap()
    {
        Debug.Log("Create Tile map");
        GameObject shipTileMap = new GameObject("ShipTileMap");
        shipTileMap.transform.position = shipTransform.position;

        for (int i = 0; i < tileGridWidth; i++)
        {
            for (int j = 0; j < tileGridLength; j ++)
            {
                GameObject tileClone = Instantiate(tile, startPos + new Vector3(i*tileSize, 0f, j*tileSize) + shipTransform.position, tile.transform.rotation) as GameObject;
                tileClone.transform.parent = shipTileMap.transform;
                tiles.Add(tileClone);
            }
        }
    }
    //deletes tiles that are outside the ship mesh
    void DeleteExtraTiles()
    {
        Debug.Log("DeleteExtraTiles");
        for (int i = tiles.Count - 1; i >= 0; i--)
        {
            Vector3 rayOrigin = tiles[i].transform.position + Vector3.up * raycastHeight * 2f;
            Ray ray = new Ray(rayOrigin, Vector3.down);
            if (!Physics.Raycast(ray, 1000f, 1 << shipLayer))
            {
                DestroyImmediate(tiles[i], false);
                tiles.RemoveAt(i);
            }
        }
    }
    //initializes vars
    void Init()
    {
        
        tiles = new List<GameObject>();

        shipMesh = shipMeshFilter.sharedMesh;
        shipBounds = shipMesh.bounds;

        tileSize = Mathf.RoundToInt(tile.transform.localScale.x);

        tileGridWidth = Mathf.RoundToInt(shipBounds.size.x / tile.transform.localScale.x);
        if (zRotation == 90f)
        {
            tileGridLength = Mathf.RoundToInt(shipBounds.size.y / tile.transform.localScale.x);
            tileSpawnHeight = shipTransform.position.y + shipBounds.center.z- shipBounds.size.z / 2;
            raycastHeight = shipBounds.center.z + shipBounds.size.z;
        }
        else
        {
            tileGridLength = Mathf.RoundToInt(shipBounds.size.z / tile.transform.localScale.x);
            tileSpawnHeight = shipTransform.position.y + shipBounds.center.y - shipBounds.size.y / 2;
            raycastHeight = shipBounds.center.y + shipBounds.size.y;
        }
        startPos = new Vector3((shipBounds.center.x - tileGridWidth*tileSize / 2), tileSpawnHeight, shipBounds.center.z - tileGridLength*tileSize / 2);

    }



    void OnWizardUpdate()
    {
        helpString = "Generates a tilemap based on the provided mesh filter";
        bool valid = true;
        //if (tile && shipMeshFilter)
        //{
        //    errorString = "";
        //    isValid = true;
        //    valid = false;
        //}

        if (shipLayer < 8)
        {
            errorString = "please assign the ship layer";
            valid = false;
        }
        if (!shipTransform)
        {
            errorString = "please assign a transform to use to position the tilemap";
            valid = false;
        }
        
        if (!shipMeshFilter)
        {
            errorString = "please assign a meshfilter";
            valid = false;
        }
        if (!tile)
        {
            errorString = "please assign a tile prefab";
            valid = false;
        }

        if(valid)
        {
            errorString = "";
        }
        isValid = valid;
    }
}

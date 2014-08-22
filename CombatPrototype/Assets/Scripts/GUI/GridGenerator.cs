using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridGenerator : MonoBehaviour 
{
    public int gridWidth;
    public int gridLength;

    public GameObject tilePrefab;
    public Vector3 startPos;

    List<GameObject> grid;
    float tileSize;

    void Start()
    {
        
        if (tilePrefab.transform.localScale.x != tilePrefab.transform.localScale.y)
        {
            Debug.LogError("Tile should be a square", this);
        }
        Mesh tileMesh = tilePrefab.GetComponent<MeshFilter>().sharedMesh;
        tileSize = tileMesh.bounds.size.x * tilePrefab.transform.localScale.x;

        grid = new List<GameObject>();
        GenerateGrid(gridWidth, gridLength, startPos);
        
    }
    /// <summary>
    /// Generates a grid
    /// </summary>
    /// <param name="gridWidth"></param>
    /// <param name="gridLength"></param>
    /// <param name="startPos">Starts at the bottom left corner - closest z, left most x</param>
    void GenerateGrid(int gridWidth, int gridLength, Vector3 startPos)
    {
        GameObject gridTile;
        Vector3 tilePos=Vector3.zero;
        GameObject gridParent = new GameObject("Grid");
        for (int i = 0; i < gridWidth; i++)
        {
            for (int j = 0; j < gridLength; j++)
            {
                tilePos.x=startPos.x+i*tileSize;
                tilePos.z = startPos.z+j*tileSize;

                gridTile = Instantiate(tilePrefab, tilePos, tilePrefab.transform.rotation) as GameObject;
                gridTile.transform.parent = gridParent.transform;
                grid.Add(gridTile);
            }
        }
    }

}

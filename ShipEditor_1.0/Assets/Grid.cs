using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/*
Programer: Wesley Allard
Summerization: This class generates a grid of gameObjects to be used for selection 
*/
public class Grid : MonoBehaviour {

	public GameObject GridTile;
	public int Width = 10;
	public int Height = 10;
	public float TileSpacing = 1.0f;
	public List<GameObject> GridTiles;

	void Start () {
		GridTiles = new List<GameObject>();
		GenerateGrid(Height, Width);
	}	
	// Update is called once per frame
	void Update () {
	
	}
	public void SetGridDimensions(int _Height, int _Width)
	{
		Height = _Height;
		Width = _Width;
		GenerateGrid(Height, Width);
	}
	void GenerateGrid(int _Height, int _Width)
	{
		foreach(GameObject t in GridTiles)
		{
			Destroy(t);
		}
		GridTiles.Clear();
			
		Vector3 startingPos = new Vector3(((1-_Width)/2.0f)* TileSpacing , ((1-_Height)/2.0f)* TileSpacing, this.transform.position.z); 

		for(int x = 0; x < _Width; x++)
		{
			for( int y = 0; y < _Height; y++)
			{
				Vector3 tilePos = new Vector3(startingPos.x + x * TileSpacing,this.transform.position.y , startingPos.y + y * TileSpacing);
				GameObject gTile; 
				gTile = Instantiate(GridTile, tilePos, GridTile.transform.rotation) as GameObject; 
				gTile.transform.parent = this.transform;
				GridTiles.Add (gTile);
			}
		}
	}
}













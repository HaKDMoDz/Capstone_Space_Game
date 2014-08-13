using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VoxelSystem : MonoBehaviour {
	//Notes
	/* Voxel systems must have a layer around the outside that is empty.
	 * 
	*/
	public  int XSize = 20;
	public  int YSize = 20;
	public  int ZSize = 20;
	public  float VoxelSpacing = 4f;
	public float x_offset;
	public float y_offset;
	public float z_offset;

	Voxel[,,] blocks;

	// Use this for initialization
	void Start () {
		//Shift the game oject over so the center is always the origin.
		this.transform.Translate (-VoxelSpacing / 2.0f, -VoxelSpacing / 2.0f, -VoxelSpacing / 2.0f);
	
		x_offset = - XSize*VoxelSpacing/2.0f;//Starting position of vertices
		y_offset = - YSize*VoxelSpacing/2.0f;
		z_offset = - ZSize*VoxelSpacing/2.0f;
		// - Size/2.0f is to offset the Vertices by half of the Size so that they are centered around the origin


		//Subscribe to Voxel Events
		VoxelEvents.onVoxelAdded += AddVoxel;
		VoxelEvents.onVoxelRemoved += RemoveVoxel;

		//Initializing Voxel/Vertex array
		blocks = new Voxel[XSize,YSize,ZSize];


		for (int x = 0; x < XSize; ++x) {
			for (int y = 0; y < YSize; ++y) {
				for (int z = 0; z < ZSize; ++z) {

					blocks[x,y,z] = new Voxel();
					if(x==0||y==0||z==0||x==XSize-1||y==YSize-1||z==ZSize-1)
					{
						blocks[x,y,z].locked = true;
					}
					else{ blocks[x,y,z].locked = false;}
				}				
			}
		}
		blocks [XSize/2,YSize/2,ZSize/2].filled = true;

		UpdateMesh ();


	}
	public void AddVoxel(VoxelPos voxel)
	{
		if (!blocks [voxel.x, voxel.y, voxel.z].locked) {
			blocks [voxel.x, voxel.y, voxel.z].filled = true;
			UpdateMesh ();
		} else {
			Debug.Log(".");
		}	
	}
	public void RemoveVoxel(VoxelPos voxel)
	{
		blocks [voxel.x, voxel.y, voxel.z].filled = false;
		UpdateMesh ();
	}

	
	// Update is called once per frame
	void Update () {
	
	}

	public void UpdateMesh()
	{
		Mesh SystemMesh = GenerateMesh ();		
		gameObject.GetComponent<MeshFilter> ().mesh = SystemMesh;

		//Nullify the mesh so it initiates a reset;
		MeshCollider meshCollider = GetComponent<MeshCollider> ();
		meshCollider.sharedMesh = null;
		meshCollider.sharedMesh = SystemMesh;
	}

	Mesh GenerateMesh ()
	{
		Mesh mesh = new Mesh ();

		List<int> Triangles = new List<int> ();
		List<Vector2> UVs = new List<Vector2> ();
		List<Vector3> Verts = new List<Vector3> ();
		int faceCount = 0;
		for (int x = 1; x < XSize-1; ++x) {
			for (int y = 1; y < YSize-1; ++y) {
				for (int z = 1; z < ZSize-1; ++z) {
					if(blocks[x,y,z].filled)
					{
						if(!blocks[x+1,y,z].filled)
						{
							Verts.Add (new Vector3(x_offset + (x+1) * VoxelSpacing, y_offset + y * VoxelSpacing, z_offset + z * VoxelSpacing));
							Verts.Add (new Vector3(x_offset + (x+1) * VoxelSpacing, y_offset + y * VoxelSpacing, z_offset + (z+1) * VoxelSpacing));
							Verts.Add (new Vector3(x_offset + (x+1) * VoxelSpacing, y_offset + (y+1) * VoxelSpacing, z_offset + (z+1) * VoxelSpacing));
							Verts.Add (new Vector3(x_offset + (x+1) * VoxelSpacing, y_offset + (y+1) * VoxelSpacing, z_offset + z * VoxelSpacing));


							Triangles.Add((faceCount*4)+1);
							Triangles.Add(faceCount*4);	
							Triangles.Add((faceCount*4)+3);
							Triangles.Add((faceCount*4)+2);
							Triangles.Add((faceCount*4)+1);
							Triangles.Add((faceCount*4)+3);

							UVs.Add(new Vector2(0,0));
					        UVs.Add(new Vector2(1,0));
							UVs.Add(new Vector2(1,1));
							UVs.Add(new Vector2(0,1));

							faceCount++;       

						}
						if(!blocks[x-1,y,z].filled)
						{
							Verts.Add (new Vector3(x_offset + (x) * VoxelSpacing, y_offset + y * VoxelSpacing, z_offset + z * VoxelSpacing));
							Verts.Add (new Vector3(x_offset + (x) * VoxelSpacing, y_offset + y * VoxelSpacing, z_offset + (z+1) * VoxelSpacing));
							Verts.Add (new Vector3(x_offset + (x) * VoxelSpacing, y_offset + (y+1) * VoxelSpacing, z_offset + (z+1) * VoxelSpacing));
							Verts.Add (new Vector3(x_offset + (x) * VoxelSpacing, y_offset + (y+1) * VoxelSpacing, z_offset + z * VoxelSpacing));

							Triangles.Add((faceCount*4)+3);
							Triangles.Add(faceCount*4);
							Triangles.Add((faceCount*4)+1);

							Triangles.Add((faceCount*4)+1);
							Triangles.Add((faceCount*4)+2);
							Triangles.Add((faceCount*4)+3);

							UVs.Add(new Vector2(0,0));
							UVs.Add(new Vector2(1,0));
							UVs.Add(new Vector2(1,1));
							UVs.Add(new Vector2(0,1));

							faceCount++;    
						}
						if(!blocks[x,y+1,z].filled)
						{
							Verts.Add (new Vector3(x_offset + (x) * VoxelSpacing, y_offset + (y+1) * VoxelSpacing, z_offset + (z) * VoxelSpacing));
							Verts.Add (new Vector3(x_offset + (x) * VoxelSpacing, y_offset + (y+1) * VoxelSpacing, z_offset + (z+1) * VoxelSpacing));
							Verts.Add (new Vector3(x_offset + (x+1) * VoxelSpacing, y_offset + (y+1) * VoxelSpacing, z_offset + (z+1) * VoxelSpacing));
							Verts.Add (new Vector3(x_offset + (x+1) * VoxelSpacing, y_offset + (y+1) * VoxelSpacing, z_offset + (z) * VoxelSpacing));
							
							Triangles.Add((faceCount*4)+3);
							Triangles.Add(faceCount*4);
							Triangles.Add((faceCount*4)+1);
							Triangles.Add((faceCount*4)+1);
							Triangles.Add((faceCount*4)+2);
							Triangles.Add((faceCount*4)+3);

							UVs.Add(new Vector2(0,0));	
							UVs.Add(new Vector2(1,0));
							UVs.Add(new Vector2(1,1));
							UVs.Add(new Vector2(0,1));

							faceCount++;  
						}
						if(!blocks[x,y-1,z].filled)
						{
							Verts.Add (new Vector3(x_offset + (x) * VoxelSpacing, y_offset + (y) * VoxelSpacing, z_offset + (z) * VoxelSpacing));
							Verts.Add (new Vector3(x_offset + (x) * VoxelSpacing, y_offset + (y) * VoxelSpacing, z_offset + (z+1) * VoxelSpacing));
							Verts.Add (new Vector3(x_offset + (x+1) * VoxelSpacing, y_offset + (y) * VoxelSpacing, z_offset + (z+1) * VoxelSpacing));
							Verts.Add (new Vector3(x_offset + (x+1) * VoxelSpacing, y_offset + (y) * VoxelSpacing, z_offset + (z) * VoxelSpacing));
							

							Triangles.Add(faceCount*4);
							Triangles.Add((faceCount*4)+3);
							Triangles.Add((faceCount*4)+1);
							Triangles.Add((faceCount*4)+2);
							Triangles.Add((faceCount*4)+1);
							Triangles.Add((faceCount*4)+3);

							UVs.Add(new Vector2(0,0));							
							UVs.Add(new Vector2(1,0));							
							UVs.Add(new Vector2(1,1));
							UVs.Add(new Vector2(0,1));

							faceCount++;  
						}
						if(!blocks[x,y,z+1].filled)
						{
							Verts.Add (new Vector3(x_offset + (x) * VoxelSpacing, y_offset + (y) * VoxelSpacing, z_offset + (z+1) * VoxelSpacing));
							Verts.Add (new Vector3(x_offset + (x) * VoxelSpacing, y_offset + (y+1) * VoxelSpacing, z_offset + (z+1) * VoxelSpacing));
							Verts.Add (new Vector3(x_offset + (x+1) * VoxelSpacing, y_offset + (y+1) * VoxelSpacing, z_offset + (z+1) * VoxelSpacing));
							Verts.Add (new Vector3(x_offset + (x+1) * VoxelSpacing, y_offset + (y) * VoxelSpacing, z_offset + (z+1) * VoxelSpacing));
							
							
							Triangles.Add(faceCount*4);
							Triangles.Add((faceCount*4)+3);
							Triangles.Add((faceCount*4)+1);				
							Triangles.Add((faceCount*4)+2);
							Triangles.Add((faceCount*4)+1);
							Triangles.Add((faceCount*4)+3);

							UVs.Add(new Vector2(0,0));							
							UVs.Add(new Vector2(1,0));							
							UVs.Add(new Vector2(1,1));
							UVs.Add(new Vector2(0,1));

							faceCount++;  
						}
						if(!blocks[x,y,z-1].filled)
						{
							Verts.Add (new Vector3(x_offset + (x) * VoxelSpacing, y_offset + (y) * VoxelSpacing, z_offset + (z) * VoxelSpacing));
							Verts.Add (new Vector3(x_offset + (x) * VoxelSpacing, y_offset + (y+1) * VoxelSpacing, z_offset + (z) * VoxelSpacing));
							Verts.Add (new Vector3(x_offset + (x+1) * VoxelSpacing, y_offset + (y+1) * VoxelSpacing, z_offset + (z) * VoxelSpacing));
							Verts.Add (new Vector3(x_offset + (x+1) * VoxelSpacing, y_offset + (y) * VoxelSpacing, z_offset + (z) * VoxelSpacing));						

							Triangles.Add((faceCount*4)+3);
							Triangles.Add(faceCount*4);
							Triangles.Add((faceCount*4)+1);
							Triangles.Add((faceCount*4)+1);
							Triangles.Add((faceCount*4)+2);
							Triangles.Add((faceCount*4)+3);

							UVs.Add(new Vector2(0,0));	
							UVs.Add(new Vector2(1,0));
							UVs.Add(new Vector2(1,1));
							UVs.Add(new Vector2(0,1));

							faceCount++;  
						}
					}
				}				
			}
		}


		mesh.vertices = Verts.ToArray();
		mesh.triangles = Triangles.ToArray();
		mesh.uv = UVs.ToArray ();
		mesh.Optimize ();
		mesh.RecalculateNormals ();

		return mesh;
	}	
}

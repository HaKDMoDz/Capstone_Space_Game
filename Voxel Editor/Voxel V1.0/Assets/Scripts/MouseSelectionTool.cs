using UnityEngine;
using System.Collections;

public class MouseSelectionTool : MonoBehaviour {

	VoxelSystem VoxelMesh;
	public GameObject SystemObject;


	// Use this for initialization
	void Start () {
		Debug.developerConsoleVisible = true;
	}
	
	// Update is called once per frame
	void Update () {

		VoxelMesh = SystemObject.GetComponent<VoxelSystem> ();
		Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

		if(Input.GetMouseButtonDown(0))
		{
			RaycastHit hit;
			if(Physics.Raycast(mouseRay,out hit,100.0f))
			{
				Debug.DrawLine(mouseRay.origin,mouseRay.direction*100, Color.red);

				if(Input.GetKey(KeyCode.LeftShift))
				{
					RemoveVoxel(hit);
				}else AddVoxel(hit);

				
			}else Debug.DrawLine(mouseRay.origin,mouseRay.direction*100,Color.green);
		}
		else Debug.DrawLine(mouseRay.origin,mouseRay.direction*100,Color.green);

	
	}
	void AddVoxel (RaycastHit hitPos)
	{

		Vector3 vtest = new Vector3();

		vtest = new Vector3 (hitPos.point.x / VoxelMesh.VoxelSpacing,
		                     hitPos.point.y / VoxelMesh.VoxelSpacing,
		                     hitPos.point.z / VoxelMesh.VoxelSpacing);
//		vtest = hitPos.point;

		Debug.Log ("pre normal "+vtest);
		//Apply an offset in the direction of the normal
		vtest += new Vector3 (hitPos.normal.x * (VoxelMesh.VoxelSpacing / 2.0f),
		                      hitPos.normal.y * (VoxelMesh.VoxelSpacing / 2.0f),
		                      hitPos.normal.z * (VoxelMesh.VoxelSpacing / 2.0f));

		Debug.Log ("normal "+hitPos.normal);

		vtest.x -= VoxelMesh.x_offset/VoxelMesh.VoxelSpacing;
		vtest.y -= VoxelMesh.y_offset/VoxelMesh.VoxelSpacing;
		vtest.z -= VoxelMesh.z_offset/VoxelMesh.VoxelSpacing;

		Debug.Log ("offset "+vtest);

		Debug.Log ("pre-round "+vtest);
		
		vtest.x =  (float)System.Math.Round(vtest.x,System.MidpointRounding.AwayFromZero);
		vtest.y =  (float)System.Math.Round(vtest.y,System.MidpointRounding.AwayFromZero);
		vtest.z =  (float)System.Math.Round(vtest.z,System.MidpointRounding.AwayFromZero);
		
		Debug.Log ("post-round "+vtest);

		VoxelPos voxel = new VoxelPos (vtest.x, vtest.y, vtest.z);

		VoxelEvents.VoxelAdded(voxel);

		Debug.Log ("Voxel Added:" + voxel.ToString ());

	}
	void RemoveVoxel(RaycastHit hitPos)
	{

		
		Vector3 vtest = new Vector3();
		
		vtest = new Vector3 (hitPos.point.x / VoxelMesh.VoxelSpacing,
		                     hitPos.point.y / VoxelMesh.VoxelSpacing,
		                     hitPos.point.z / VoxelMesh.VoxelSpacing);
		//		vtest = hitPos.point;
		
		Debug.Log ("pre normal "+vtest);
		//Apply an offset in the direction of the normal
		vtest -= new Vector3 (hitPos.normal.x * (VoxelMesh.VoxelSpacing / 2.0f),
		                      hitPos.normal.y * (VoxelMesh.VoxelSpacing / 2.0f),
		                      hitPos.normal.z * (VoxelMesh.VoxelSpacing / 2.0f));
		
		Debug.Log ("normal "+hitPos.normal);
		
		vtest.x -= VoxelMesh.x_offset/VoxelMesh.VoxelSpacing;
		vtest.y -= VoxelMesh.y_offset/VoxelMesh.VoxelSpacing;
		vtest.z -= VoxelMesh.z_offset/VoxelMesh.VoxelSpacing;
		
		Debug.Log ("offset "+vtest);
		
		Debug.Log ("pre-round "+vtest);
		
		vtest.x =  (float)System.Math.Round(vtest.x,System.MidpointRounding.AwayFromZero);
		vtest.y =  (float)System.Math.Round(vtest.y,System.MidpointRounding.AwayFromZero);
		vtest.z =  (float)System.Math.Round(vtest.z,System.MidpointRounding.AwayFromZero);
		
		Debug.Log ("post-round "+vtest);
		
		VoxelPos voxel = new VoxelPos (vtest.x, vtest.y, vtest.z);
		
		VoxelEvents.VoxelAdded(voxel);
		
		Debug.Log ("Voxel Added:" + voxel.ToString ());
			


		VoxelEvents.VoxelRemoved(voxel);
	}


}

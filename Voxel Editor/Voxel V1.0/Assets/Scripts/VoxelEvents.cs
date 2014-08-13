using UnityEngine;
using System.Collections;

public class VoxelEvents : MonoBehaviour {

	public delegate void VoxelEventHandler(VoxelPos voxel);

	public static event VoxelEventHandler onVoxelAdded;
	public static event VoxelEventHandler onVoxelRemoved;

	public static void VoxelAdded(VoxelPos voxel)
	{
		if (onVoxelAdded != null) 
						onVoxelAdded (voxel);
				
	}
	public static void VoxelRemoved(VoxelPos voxel)
	{
		if (onVoxelRemoved != null)
						onVoxelRemoved (voxel);
	}
}

using UnityEngine;
using System.Collections;

public class VoxelPos {
	public int x, y, z;
	public VoxelPos()
	{
	}
	public VoxelPos(int _x, int _y, int _z)
	{
		x = _x;
		y = _y;
		z = _z;
	}
	public VoxelPos(float _x, float _y, float _z)
	{
		x = (int)_x;
		y = (int)_y;
		z = (int)_z;
	}
	public static VoxelPos operator - (VoxelPos v1, VoxelPos v2)
	{
		VoxelPos newVox = new VoxelPos (v1.x - v2.x, v1.y - v2.y, v1.z - v2.z);
		return newVox;
	}
	public override string ToString ()
	{
		return string.Format ("X:"+x+" Y:"+y+" Z:"+z);
	}

}

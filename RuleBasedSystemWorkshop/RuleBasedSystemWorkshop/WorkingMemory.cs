using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// This is to handle Unity Vector3s. DELETE during import to Unity
/// </summary>
public class Vector3
{
    private float x, y, z;

    public static Vector3 zero = new Vector3(0, 0, 0);

    Vector3(float _x, float _y, float _z)
    {
        x = _x;
        y = _y;
        z = _z;
    }
}

class WorkingMemory
{
    List<Vector3> enemyPositions;
    List<Vector3> playerPositions;

    public WorkingMemory()
    {
        Debug.Log("Working Memory Created");

        enemyPositions = new List<Vector3>();
        playerPositions = new List<Vector3>();
    }

    /// <summary>
    /// This function will collect "facts" from the units in combat for use by the Rule Interpreter
    /// </summary>
    public void PopulateWorkingMemory()
    {
        Vector3 someVector = Vector3.zero;
    }
}

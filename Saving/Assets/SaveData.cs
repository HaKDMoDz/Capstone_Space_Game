using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class SaveData
{

    int health;
    public int Health
    {
        get { return health; }
        set { health = value; }
    }

    
    List<SerializableVector3> positions;
    public List<Vector3> Positions
    {
        get 
        {
            List<Vector3> vecList = new List<Vector3>();
            for (int i = 0; i < positions.Count; i++)
            {
                vecList.Add(positions[i].Vec3);
            }
            return vecList; 
        }
        set 
        {
            positions.Clear();
            for (int i = 0; i < value.Count; i++)
            {
                SerializableVector3 vecSerialized = new SerializableVector3(value[i]);
                positions.Add(vecSerialized);
            }
        }
    }

    Dictionary<int, string> table;

    public Dictionary<int, string> Table
    {
        get { return table; }
        set { table = value; }
    }

    Player.State currentState;
    public Player.State CurrentState
    {
        get { return currentState; }
        set { currentState = value; }
    }

    //int currentState;
    //public int CurrentState
    //{
    //    get
    //    {
    //        return (Player.State)currentState;
    //    }
    //    set
    //    {
    //        currentState = value;
    //    }
    //}
    
    public SaveData(int _health, List<Vector3> _pos, Dictionary<int, string> _table, Player.State _currentState)
    {
        positions = new List<SerializableVector3>();
        Health = _health;
        Positions = _pos;
        table = _table;
        currentState = _currentState;
    }
}

[Serializable]
public class SerializableVector3
{
    public float x, y, z;

    public SerializableVector3() { }
    public SerializableVector3(Vector3 vec3)
    {
        x = vec3.x;
        y = vec3.y;
        z = vec3.z;
    }
    
    
    public void Fill(Vector3 vec3)
    {
        x = vec3.x;
        y = vec3.y;
        z = vec3.z;
    }

    public Vector3 Vec3
    {
        get
        {
            return new Vector3(x, y, z);
        }
        set
        {
            Fill(value);
        }
    }

}

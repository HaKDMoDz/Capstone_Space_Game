using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PersistentData : SingletonComponent<PersistentData>
{

    int maxHealth = 100;
    public int MaxHealth
    {
        get { return maxHealth; }
    }

    int health;
    public int Health
    {
        get { return health; }
        set { health = value; }
    }

    List<Vector3> positions;
    public List<Vector3> Positions
    {
        get { return positions; }
        set { positions = value; }
    }
    List<Vector3> initialPositions;

    Dictionary<int, string> table;
    public Dictionary<int, string> Table
    {
        get { return table; }
        set { table = value; }
    }

    Dictionary<int, string> defaultTable;

    Player.State currentState;
    public Player.State CurrentState
    {
        get { return currentState; }
        set { currentState = value; }
    }



    public void Save()
    {
        SaveData saveData = new SaveData(Health, Positions, Table, CurrentState);
        SaveManager.Instance.Save(saveData);
    }
    public void Load()
    {
        SaveData saveData;

        if (!SaveManager.Instance.Load(out saveData))
        {
            Debug.Log("Save data does not exist");
            defaultTable = new Dictionary<int, string>();
            defaultTable.Add(1, "One");
            saveData = new SaveData(maxHealth, GenerateNewPositions(), defaultTable, Player.State.Idle);
        }
        health = saveData.Health;
        positions = saveData.Positions;
        table = saveData.Table;
        currentState = saveData.CurrentState;
    }

    public List<Vector3> GenerateNewPositions()
    {
        int numPos = 2;
        List<Vector3> positions = new List<Vector3>();
        for (int i = 0; i < numPos; i++)
        {
            Vector3 newPos = Random.insideUnitSphere * 10f;
            positions.Add(newPos);
        }
        return positions;
    }


}

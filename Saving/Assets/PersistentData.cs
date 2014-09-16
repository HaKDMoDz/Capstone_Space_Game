using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PersistentData : SingletonComponent<PersistentData> {

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





    public void Save()
    {
        SaveData saveData = new SaveData(Health,Positions);
        SaveManager.Instance.Save(saveData);
    }
    public void Load()
    {
        SaveData saveData;

         if(!SaveManager.Instance.Load(out saveData))
         {
             Debug.Log("Save data does not exist");
             saveData = new SaveData(maxHealth, GenerateNewPositions());
         }
        health = saveData.Health;
        positions = saveData.Positions;
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

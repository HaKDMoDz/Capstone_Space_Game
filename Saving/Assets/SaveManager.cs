using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SaveManager : SingletonComponent<SaveManager>
{

    protected override void Awake()
    {
        base.Awake();
        if (Instance != this && Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    public void Save(SaveData saveData)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/SaveData.dat");
        bf.Serialize(file, saveData);
        file.Close();
    }

    public bool Load(out SaveData saveData)
    {
        if (File.Exists(Application.persistentDataPath + "/SaveData.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/SaveData.dat", FileMode.Open);
            Debug.Log(Application.persistentDataPath + "/SaveData.dat");
            saveData = (SaveData)bf.Deserialize(file);
            Debug.Log(saveData.CurrentState);
            file.Close();
            return true;
        }
        else
        {
            saveData = null;
            return false;
        }
    }



}



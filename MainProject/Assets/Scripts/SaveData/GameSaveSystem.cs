#region Usings
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;
using System.Text;
#endregion //usings

public class GameSaveSystem
{
    #region Fields

    public SaveGameList savesList { get; private set; }

    #region Internal
    private string fileExtension, saveDirectory, fileName_SavesList, autosaveFileName, quickSaveName;
    private int maxAutoSaves, maxQuickSaves, maxNormalSaves;

    private BinaryFormatter binFormatter;
    private FileStream fileStream;
    private string path;

    //GameData gameData;
    private SerializedGameData sz_gameData;
    private DateTime timeStamp;

    #endregion//internal
    #endregion Fields

    #region Methods

    #region Public
    public GameSaveSystem(string fileExtension, string saveDirectory, string fileName_SavesList,
        string autosaveFileName, string quickSaveName,
        int numAutoSaves, int numQuickSaves, int numNormalSaves)
    {
        this.fileExtension = fileExtension;
        this.saveDirectory = saveDirectory;
        this.fileName_SavesList = fileName_SavesList;
        this.autosaveFileName = autosaveFileName;
        this.quickSaveName = quickSaveName;
        this.maxAutoSaves = numAutoSaves;
        this.maxQuickSaves = numQuickSaves;
        this.maxNormalSaves = numNormalSaves;

        binFormatter = new BinaryFormatter();
        sz_gameData = new SerializedGameData();

        CreateSaveGameDirectory();
        LoadSavesList();
    }
    public void AutoSave(GameData gameData)
    {
        timeStamp = DateTime.Now;

        string fileName;
        int autoSaveCounter = savesList.autoSaveCount;
        
        //not at max
        if(autoSaveCounter < maxAutoSaves)
        {
            autoSaveCounter++;
            fileName = autosaveFileName + autoSaveCounter;
        }
        else //at max limit
        {
            //delete first autoSave, then add
            fileName = savesList.autoSaves.Dequeue().fileName;
            File.Delete(fileName);
        }
        //autoSaveCounter = ++autoSaveCounter % maxAutoSaves;
        
        Save(gameData, fileName);

        //update file list
        savesList.AddSave(SaveType.AutoSave, fileName, timeStamp);
        SaveSavesList();
    }

    private void Save(GameData gameData, string fileName)
    {
        path = BuildPathString(fileName);

        #if FULL_DEBUG
        Debug.Log("Saving GameData to " + path + " at time: "+timeStamp.ToString());
        #endif

        fileStream = File.Create(path);
        //SerializeGameData(gameData, out sz_gameData);
        gameData.Serialize(ref sz_gameData);
        binFormatter.Serialize(fileStream, sz_gameData);
        fileStream.Close();


    }
    public bool LoadAutoSave(ref GameData gameData)
    {
        if(savesList.autoSaveCount <= 0)
        {
            return false;
        }
        string lastAutoSaveName = savesList.autoSaves.Last().fileName;
        return (Load(ref gameData, lastAutoSaveName));
    }

    private bool Load(ref GameData gameData, string fileName)
    {
        path = BuildPathString(fileName);

#if FULL_DEBUG
        //Debug.Log("filename: "+fileName);
        Debug.Log("Loading game data from " + path);
#endif

        if (File.Exists(path))
        {
            fileStream = File.Open(path, FileMode.Open);
            sz_gameData = binFormatter.Deserialize(fileStream) as SerializedGameData;
            sz_gameData.DeSerialize(ref gameData);
            fileStream.Close();
            return true;
        }
        else
        {
            gameData = null;
            return false;
        }
    }

    #endregion Public

    #region Private

    private void LoadSavesList()
    {
        path = BuildPathString(fileName_SavesList);
        if (File.Exists(path))
        {
            fileStream = File.Open(path, FileMode.Open);
            savesList = binFormatter.Deserialize(fileStream) as SaveGameList;
            fileStream.Close();
        }
        else
        {
            savesList = new SaveGameList(maxAutoSaves, maxQuickSaves, maxNormalSaves);
        }
    }
    private void SaveSavesList()
    {
        path = BuildPathString(fileName_SavesList);
        fileStream = File.Create(path);
        binFormatter.Serialize(fileStream, savesList);
        fileStream.Close();
    }

    #region Helper
    private void CreateSaveGameDirectory()
    {
        if (!Directory.Exists(Application.persistentDataPath + '/' + saveDirectory))
        {
            Directory.CreateDirectory(Application.persistentDataPath + '/' + saveDirectory);

#if FULL_DEBUG
            //Debug.Log("Create Directory: " + Application.persistentDataPath + '/' + saveDirectory);
#endif
        }
#if FULL_DEBUG
        //Debug.Log("Directory exists: " + Application.persistentDataPath + '/' + saveDirectory);
#endif
    }
    private string BuildPathString(string fileName)
    {
        return Application.persistentDataPath + '/' + saveDirectory + '/' + fileName + '.' + fileExtension;
    }
    #endregion Helper
    #endregion Private
    #endregion Methods
}

#region AdditionalData

public enum SaveType { AutoSave, QuickSave, NormalSave }

[Serializable]
public class SaveGameList
{
    //autoSaves
    public int maxAutoSaves { get; private set; }
    public int autoSaveCount { get; private set; }
    public Queue<SaveGame> autoSaves { get; private set; }

    //quickSaves
    public int maxQuickSaves { get; private set; }
    public int quickSaveCount { get; private set; }
    public Queue<SaveGame> quickSaves { get; private set; }

    //normal Saves
    public int maxNormalSaves { get; private set; }
    public int normalSaveCount { get; private set; }
    public Queue<SaveGame> normalSaves { get; private set; }

    public SaveGameList(int maxAutoSaves, int maxQuickSaves, int maxNormalSaves)
    {
        autoSaves = new Queue<SaveGame>();
        quickSaves = new Queue<SaveGame>();
        normalSaves = new Queue<SaveGame>();
        this.maxAutoSaves = maxAutoSaves;
        this.maxQuickSaves = maxQuickSaves;
        this.maxNormalSaves = maxNormalSaves;   
    }
    
    public void AddSave(SaveType saveType, string fileName, DateTime saveTime)
    {
        switch (saveType)
        {
            case SaveType.AutoSave:

                if (autoSaveCount > maxAutoSaves)
                {
                    #if !NO_DEBUG
                    Debug.LogError("Beyond max autosaves -- NumAutosaves: " +autoSaveCount + " max: "+ maxAutoSaves);
                    #endif
                }
                else if(autoSaveCount < maxAutoSaves)
                {
                    autoSaveCount++;
                }
                autoSaves.Enqueue(new SaveGame(saveType, fileName, saveTime));
                break;
            case SaveType.QuickSave:
                quickSaveCount++;
                break;
            case SaveType.NormalSave:
                normalSaveCount++;
                break;
            default:
                break;
        }
    }
}

[Serializable]
public class SaveGame
{
    public SaveType saveType;
    public string fileName;
    public DateTime saveTime;

    public SaveGame(SaveType saveType, string fileName, DateTime saveTime)
    {
        this.saveType = saveType;
        this.fileName = fileName;
        this.saveTime = saveTime;
    }
}

#endregion AdditionalData
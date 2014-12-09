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

    private SaveGameList gameSavesList;
    public SaveGameList GameSavesList
    {
        get { return gameSavesList; }
    }

    #region Internal
    private string fileExtension, saveDirectory, fileName_SavesList, autosaveFileName, quickSaveName;
    private int numAutoSaves, numQuickSaves, numNormalSaves;
    
    BinaryFormatter binFormatter;
    FileStream fileStream;
    string path;

    //GameData gameData;
    SerializedGameData sz_gameData;

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
        this.numAutoSaves = numAutoSaves;
        this.numQuickSaves = numQuickSaves;
        this.numNormalSaves = numNormalSaves;

        binFormatter = new BinaryFormatter();

        CreateSaveGameDirectory();
        LoadSavesList();
    }

    public void Save(GameData gameData, string fileName)
    {
        path = BuildPathString(fileName);

        #if FULL_DEBUG
        Debug.Log("Saving GameData to " + path);
        #endif

        fileStream = File.Create(path);
        //SerializeGameData(gameData, out sz_gameData);
        gameData.Serialize(ref sz_gameData);
        binFormatter.Serialize(fileStream, sz_gameData);
        fileStream.Close();

        //Update file list
        //gameSavesList.Add(fileName);
        SaveSavesList();
    }
    public bool Load(ref GameData gameData, string fileName)
    {
        path = BuildPathString(fileName);
        
        #if FULL_DEBUG
        //Debug.Log("filename: "+fileName);
        Debug.Log("Loading game data from "+ path);
        #endif

        if(File.Exists(path))
        {
            fileStream = File.Open(path, FileMode.Open);
            sz_gameData = binFormatter.Deserialize(fileStream) as SerializedGameData;
            //DeSerializeGameData(sz_gameData, out gameData);
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

    }
    private void SaveSavesList()
    {

    }

    #region Helper
    private void CreateSaveGameDirectory()
    {
        
        if(!Directory.Exists(Application.persistentDataPath +'/'+saveDirectory))
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

public enum SaveType { AutoSave, QuickSave, NormalSave}

[Serializable]
public class SaveGameList
{
    private Queue<SaveGame> autoSaves;
    public Queue<SaveGame> AutoSaves
    {
        get { return autoSaves; }
    }

    private Queue<SaveGame> quickSaves;
    public Queue<SaveGame> QuickSaves
    {
        get { return quickSaves; }
    }

    private Queue<SaveGame> normalSaves;
    public Queue<SaveGame> NormalSaves
    {
        get { return normalSaves; }
    }
    
    public SaveGameList()
    {
        autoSaves = new Queue<SaveGame>();
        quickSaves = new Queue<SaveGame>();
        normalSaves = new Queue<SaveGame>();
    }
    //public void Add(string fileName)
    //{
    //    count++;
    //    fileNameList.Add(fileName);
    //}
    
    //public bool FileExists(string fileName)
    //{
    //    //return fileNameList.Contains(fileName);
    //}
    public void AddSave(SaveType saveType, string fileName, string saveTime)
    {

    }
}

[Serializable]
public class SaveGame
{
    public SaveType saveType;
    public string fileName;
    public string saveTime;

    public SaveGame(SaveType saveType, string fileName, string saveTime)
    {
        this.saveType = saveType;
        this.fileName = fileName;
        this.saveTime = saveTime;
    }
}

#endregion AdditionalData
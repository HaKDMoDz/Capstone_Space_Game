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

    

    private GameSavesList gameSavesList;
    public GameSavesList GameSavesList
    {
        get { return gameSavesList; }
    }

    #region Internal
    private string fileExtension, saveDirectory, fileName_SavesList, autosaveFileName;

    BinaryFormatter binFormatter;
    FileStream fileStream;
    string path;

    //GameData gameData;
    SerializedGameData sz_gameData;

    #endregion//internal
    #endregion Fields

    #region Methods

    #region Public
    public GameSaveSystem(string fileExtension, string saveDirectory, string fileName_SavesList, string autosaveFileName)
    {
        this.fileExtension = fileExtension;
        this.saveDirectory = saveDirectory;
        this.fileName_SavesList = fileName_SavesList;
        this.autosaveFileName = autosaveFileName;

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
        sz_gameData = gameData.Serialized();
        binFormatter.Serialize(fileStream, sz_gameData);
        fileStream.Close();

        //Update file list
        //gameSavesList.Add(fileName);
        SaveSavesList();
    }
    public bool Load(out GameData gameData, string fileName)
    {
        path = BuildPathString(fileName);
        
        #if FULL_DEBUG
        Debug.Log("filename: "+fileName);
        Debug.Log("Loading game data from "+ path);
        #endif

        if(File.Exists(path))
        {
            fileStream = File.Open(path, FileMode.Open);
            sz_gameData = binFormatter.Deserialize(fileStream) as SerializedGameData;
            //DeSerializeGameData(sz_gameData, out gameData);
            gameData = sz_gameData.DeSerialized();
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
[Serializable]
public class GameSavesList
{
    private int count = 0;
    public int Count
    {
        get { return count; }
    }

    private List<string> fileNameList;
    public List<string> FileNameList
    {
        get { return fileNameList; }
    }

    public GameSavesList()
    {
        count = 0;
        fileNameList = new List<string>();
    }
    public void Add(string fileName)
    {
        count++;
        fileNameList.Add(fileName);
    }
    public bool FileExists(string fileName)
    {
        return fileNameList.Contains(fileName);
    }

}
#endregion AdditionalData
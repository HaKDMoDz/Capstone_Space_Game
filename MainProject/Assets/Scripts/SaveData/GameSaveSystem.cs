#region Usings
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.IO;
using System.Linq;
using System.Text;
#endregion //usings

public class GameSaveSystem
{
    #region Fields

    public SaveGameList savesList { get; private set; }//keeps track of all saves created by the save system

    #region Internal
    private string fileExtension, saveDirectory, fileName_SavesList, autosaveFileName, quickSaveName;
    private int maxAutoSaves, maxQuickSaves, maxNormalSaves;

    //caching common objects
#if FULL_DEBUG || LOW_DEBUG
    private XmlSerializer serializer;
    private XmlSerializer saveListSerializer;
#else
    private BinaryFormatter serializer;
#endif
    private FileStream fileStream;
    private string path;
    private SerializedGameData sz_gameData;
    private DateTime timeStamp;

    #endregion//internal
    #endregion Fields

    #region Methods

    #region Public
    /// <summary>
    /// Initializes the save system. Will assign variables related to saves, create the save game directory, and attempt to load the saves list file.
    /// </summary>
    public GameSaveSystem()
        //string fileExtension, string saveDirectory, string fileName_SavesList,
        //string autosaveFileName, string quickSaveName,
        //int numAutoSaves, int numQuickSaves, int numNormalSaves)
    {
        fileExtension = SaveFilesConfig.FileExtension_GameSave;
        saveDirectory = SaveFilesConfig.Directory_GameSave;
        fileName_SavesList = SaveFilesConfig.FileName_GameSavesList;
        autosaveFileName = SaveFilesConfig.AutoSaveFileName;
        quickSaveName = SaveFilesConfig.QuickSaveName;
        maxAutoSaves =SaveFilesConfig.NumAutoSaves;
        maxQuickSaves = SaveFilesConfig.NumQuickSaves;
        maxNormalSaves = SaveFilesConfig.NumNormalSaves;
        
        #if FULL_DEBUG || LOW_DEBUG
        serializer = new XmlSerializer(typeof(SerializedGameData));
        saveListSerializer = new XmlSerializer(typeof(SaveGameList));
        #else
        serializer = new BinaryFormatter();
        #endif
        
        sz_gameData = new SerializedGameData();

        CreateSaveGameDirectory();
        LoadSavesList();
    }//Ctor

    /// <summary>
    /// Creates an autosave. For use during scene trasitions, etc.
    /// If the max number of autosaves is reached, the oldest one will be deleted
    /// </summary>
    /// <param name="gameData">
    /// the gameData to serialize and store for the quickSave
    /// </param>
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
        Save(gameData, fileName);
        //update file list
        savesList.AddSave(SaveType.AutoSave, fileName, timeStamp);
        SaveSavesList();
    }//AutoSave

    /// <summary>
    /// Attemps to loads the latest autosave and populates gameData if one is found
    /// </summary>
    /// <param name="gameData">
    /// will be populated based on the latest autosave, if found
    /// </param>
    /// <returns>
    /// true if an autosave is found
    /// </returns>
    public bool LoadAutoSave(ref GameData gameData)
    {
        if(savesList.autoSaveCount <= 0)
        {
            return false;
        }
        #if FULL_DEBUG
        //Debug.Log("AutoSave was timestamped at " + savesList.autoSaves.Last().saveTime.ToString());
        #endif
        string latestAutoSaveName = savesList.autoSaves.Last().fileName;
        return (Load(ref gameData, latestAutoSaveName));
    }

    /// <summary>
    /// Loads the latest save game.
    /// Should call AnySavesExist first, to validate
    /// </summary>
    /// <param name="gameData"></param>
    public void LoadLatestSave(ref GameData gameData)
    {
        #if FULL_DEBUG || LOW_DEBUG
        if(!AnySavesExist())
        {
            Debug.LogError("No saves exist");
            return;
        }
	    #endif
        Load(ref gameData, savesList.latestSaveGame.fileName);
    }

    /// <summary>
    /// Creates a quickSave that the user can use on demand
    /// If the max number of quickSaves is reached, the oldest one will be deleted
    /// </summary>
    /// <param name="gameData">
    /// the gameData to serialize and store for the quickSave
    /// </param>
    public void QuickSave(GameData gameData)
    {
        timeStamp = DateTime.Now;
        string fileName;
        int quickSaveCounter = savesList.quickSaveCount;
        //not at max
        if(quickSaveCounter < maxQuickSaves)
        {
            quickSaveCounter++;
            fileName = quickSaveName + quickSaveCounter;
        }
        else //at max limit
        {
            //delete first, and add a new one
            fileName = savesList.quickSaves.Dequeue().fileName;
            File.Delete(fileName);
        }
        Save(gameData, fileName);
        //update file list
        savesList.AddSave(SaveType.QuickSave, fileName, timeStamp);
        SaveSavesList();
    }
    /// <summary>
    /// Attemps to loads the latest quickSave and populates gameData if one is found
    /// </summary>
    /// <param name="gameData">
    /// will be populated based on the latest quickSave, if found
    /// </param>
    /// <returns>
    /// true if an autosave is found
    /// </returns>
    /// 
    public bool LoadQuickSave(ref GameData gameData)
    {
        if(savesList.quickSaveCount <=0)
        {
            return false;
        }
        string latestQuickSaveName = savesList.quickSaves.Last().fileName;
        return Load(ref gameData, latestQuickSaveName);
    }


    /// <summary>
    /// Will return true if any saves exist - essentially if a game has been started and can be continued/loaded.
    /// This method is accessible via the GameControllers AnySavesExist method
    /// </summary>
    /// <returns>
    /// true if any saves exist
    /// </returns>
    public bool AnySavesExist()
    {
        return (savesList.autoSaveCount > 0 || savesList.quickSaveCount > 0 || savesList.normalSaveCount > 0);
    }

    #endregion Public

    #region Private

    /// <summary>
    /// does the actual file writing
    /// </summary>
    /// <param name="gameData"></param>
    /// <param name="fileName"></param>
    private void Save(GameData gameData, string fileName)
    {
        path = BuildPathString(fileName);

        #if FULL_DEBUG
        Debug.Log("Saving GameData to " + path + " at time: " + timeStamp.ToString());
        #endif

        fileStream = File.Create(path);
        //SerializeGameData(gameData, out sz_gameData);
        gameData.Serialize(ref sz_gameData);
        serializer.Serialize(fileStream, sz_gameData);
        fileStream.Close();


    }//Save

    /// <summary>
    /// Attempts to load a file from HDD
    /// </summary>
    /// <param name="gameData">
    /// will be populated if the provided file is found
    /// </param>
    /// <param name="fileName">
    /// name of the file to load (do not include an extension nor a path - a full path will be generated within the method)
    /// </param>
    /// <returns>
    /// true if the provided filename is found within the standard directory
    /// </returns>
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
            sz_gameData = serializer.Deserialize(fileStream) as SerializedGameData;
            sz_gameData.DeSerialize(ref gameData);
            fileStream.Close();
            return true;
        }
        else
        {
            gameData = null;
            return false;
        }
    }//Load

    /// <summary>
    /// Loads the latest list of files - used to keep track of all saves being managed by the save system
    /// </summary>
    private void LoadSavesList()
    {
        path = BuildPathString(fileName_SavesList);
        if (File.Exists(path))
        {
            fileStream = File.Open(path, FileMode.Open);
            #if FULL_DEBUG || LOW_DEBUG
            savesList = saveListSerializer.Deserialize(fileStream) as SaveGameList;
            savesList.ConvertFromXMLCompatible();
            #else
            savesList = serializer.Deserialize(fileStream) as SaveGameList;
            #endif
            fileStream.Close();
        }
        else
        {
            savesList = new SaveGameList(maxAutoSaves, maxQuickSaves, maxNormalSaves);
        }
    }
    /// <summary>
    /// Save the current save file list
    /// </summary>
    private void SaveSavesList()
    {
        path = BuildPathString(fileName_SavesList);
        fileStream = File.Create(path);
        #if FULL_DEBUG || LOW_DEBUG
        savesList.MakeXMLCompatible();
        saveListSerializer.Serialize(fileStream, savesList);
        #else
        serializer.Serialize(fileStream, savesList);
        #endif
        fileStream.Close();
    }

    #region Helper
    /// <summary>
    /// Helper method to generate the save game directory within the application's data path
    /// </summary>
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
    //returns a string that represents that full path for a save file, including the save directory and extension
    private string BuildPathString(string fileName)
    {
        return Application.persistentDataPath + '/' + saveDirectory + '/' + fileName + '.' + fileExtension;
    }
    #endregion Helper
    #endregion Private
    #endregion Methods
}

#region AdditionalData

public enum SaveType { AutoSave, QuickSave, NormalSave } //the types of saves that the savesystem can handle


[Serializable]
public class SaveGameList //keeps track of the various save files being handled by the save system
{
    //the following is required for XML serialization
#if FULL_DEBUG || LOW_DEBUG
    public List<SaveGameMetaData> autoSavesList { get; private set; }
    public List<SaveGameMetaData> quickSavesList { get; private set; }
    public List<SaveGameMetaData> normalSavesList { get; private set; }
    public void MakeXMLCompatible()
    {
        autoSavesList = autoSaves.ToList();
        quickSavesList = autoSaves.ToList();
        normalSavesList = normalSaves.ToList();
    }
    public void ConvertFromXMLCompatible()
    {
        autoSaves = new Queue<SaveGameMetaData>(autoSavesList);
        quickSaves = new Queue<SaveGameMetaData>(quickSavesList);
        normalSaves = new Queue<SaveGameMetaData>(normalSavesList);
    }
#endif


    //autoSaves
    public int maxAutoSaves { get; private set; }
    public int autoSaveCount { get; private set; }
    [XmlIgnore]
    public Queue<SaveGameMetaData> autoSaves { get; private set; }

    //quickSaves
    public int maxQuickSaves { get; private set; }
    public int quickSaveCount { get; private set; }
    [XmlIgnore]
    public Queue<SaveGameMetaData> quickSaves { get; private set; }

    //normal Saves
    public int maxNormalSaves { get; private set; }
    public int normalSaveCount { get; private set; }
    [XmlIgnore]
    public Queue<SaveGameMetaData> normalSaves { get; private set; }

    //records the latest save game for quick retreival
    public SaveGameMetaData latestSaveGame { get; private set; }
    public SaveGameList()
    {
        autoSaves = new Queue<SaveGameMetaData>();
        quickSaves = new Queue<SaveGameMetaData>();
        normalSaves = new Queue<SaveGameMetaData>();
        latestSaveGame = null;
    }
    public SaveGameList(int maxAutoSaves, int maxQuickSaves, int maxNormalSaves)
    {
        autoSaves = new Queue<SaveGameMetaData>();
        quickSaves = new Queue<SaveGameMetaData>();
        normalSaves = new Queue<SaveGameMetaData>();
        this.maxAutoSaves = maxAutoSaves;
        this.maxQuickSaves = maxQuickSaves;
        this.maxNormalSaves = maxNormalSaves;
        latestSaveGame = null;
    }

    /// <summary>
    /// Adds a save file to be included in the file list
    /// </summary>
    /// <param name="saveType">
    /// the type of save (Auto / Quick / Normal)
    /// </param>
    /// <param name="fileName">
    /// the name of the file (not including path)
    /// </param>
    /// <param name="saveTime">
    /// a timestamp for when the save file was created
    /// </param>
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
                latestSaveGame = new SaveGameMetaData(saveType, fileName, saveTime);
                autoSaves.Enqueue(latestSaveGame);
                break;
            case SaveType.QuickSave:
                if(quickSaveCount > maxQuickSaves)
                {
                    #if !NO_DEBUG
                    Debug.LogError("Beyond max quickSaves -- NumQuicksaves: " + quickSaveCount + " max: " + maxQuickSaves);
                    #endif
                }
                else if(quickSaveCount < maxQuickSaves)
                {
                    quickSaveCount++;
                }
                latestSaveGame = new SaveGameMetaData(saveType, fileName, saveTime);
                quickSaves.Enqueue(latestSaveGame);
                break;
            case SaveType.NormalSave:
                normalSaveCount++;
                break;
            default:
                break;
        }
    }//AddSave

    
}

[Serializable]
public class SaveGameMetaData //represents MetaData regarding a save file
{
    public SaveType saveType;
    public string fileName;
    public DateTime saveTime;
    public SaveGameMetaData()
    {

    }
    public SaveGameMetaData(SaveType saveType, string fileName, DateTime saveTime)
    {
        this.saveType = saveType;
        this.fileName = fileName;
        this.saveTime = saveTime;
    }
}

#endregion AdditionalData
#region Usings
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.IO;
#endregion Usings

//pure C# class that deals with saving blueprints
public class ShipBlueprintSaveSystem 
{
    #region Fields
    public SavedShipBPList savedBPList { get; private set; } //keeps track of all saves blueprints

    #region Internal
    //saving info
    private string fileExtension_ShipBP = "sbp";
    private string saveDirectory_ShipBP = "ShipBlueprints";
    private string fileName_SaveList = "ShipBPFileList";

    //cached vars
#if FULL_DEBUG || LOW_DEBUG
    private XmlSerializer serializer;
    private XmlSerializer saveListSerializer;
#else    
    private BinaryFormatter serializer;
#endif
    private FileStream fileStream;
    private string path;
    private SerializedShipBlueprint sz_ShipBP;

    #endregion Internal
    #endregion Fields

    #region Methods
    #region Public
    public ShipBlueprintSaveSystem()
    {
        #if FULL_DEBUG || LOW_DEBUG
        serializer = new XmlSerializer(typeof(SerializedShipBlueprint));
        saveListSerializer = new XmlSerializer(typeof(SavedShipBPList));
        #else
        serializer = new BinaryFormatter();
        #endif
        sz_ShipBP = new SerializedShipBlueprint(0);

        fileExtension_ShipBP = SaveFilesConfig.FileExtension_ShipBP;
        saveDirectory_ShipBP = SaveFilesConfig.Directory_ShipBP;
        fileName_SaveList = SaveFilesConfig.FileName_ShipBP_SaveList;

        CreateShipBPDirectory();
        LoadSavesList();
    }
  

    /// <summary>
    /// Saves the ShipBlueprint object as a file named as specified
    /// </summary>
    /// <param name="shipBP">
    /// The ship blueprint object to save
    /// </param>
    /// <param name="fileName">
    /// The name of the blueprint to save as
    /// </param>
    public void SaveBlueprint(ShipBlueprint shipBP, string fileName)
    {
        SerializeShipBP(shipBP);
        path = BuildPathString(fileName); 
        #if FULL_DEBUG
        Debug.Log("Saving Ship Blueprint to " + path);
        #endif

        fileStream = File.Create(path);
        serializer.Serialize(fileStream, sz_ShipBP);
        fileStream.Close();
        //update saves list
        savedBPList.Add(shipBP.MetaData);
        //optimize
        SaveSavesList();
    }
    /// <summary>
    /// Loads the blueprint as per the specified fileName and populates the reference to the blueprint
    /// </summary>
    /// <param name="shipBP">
    /// The blueprint to populate upon loading
    /// </param>
    /// <param name="fileName">
    /// Name of the blueprint file to load
    /// </param>
    /// <returns>
    /// Whether the load was successful
    /// </returns>
    public bool LoadBlueprint(out ShipBlueprint shipBP, string fileName)
    {
        path = BuildPathString(fileName);
        if (File.Exists(path))
        {
            #if FULL_DEBUG
            Debug.Log("Loading Ship Blueprint from " + path);
            #endif
            fileStream = File.Open(path, FileMode.Open);
            sz_ShipBP = serializer.Deserialize(fileStream) as SerializedShipBlueprint;
            DeSerializeSipBP(sz_ShipBP, out shipBP);
            fileStream.Close();
            return true;
        }
        else
        {
            #if !NO_DEBUG
            Debug.LogError("No ShipBlueprint found at path " + path);
            #endif
            shipBP = null;
            return false;
        }
    }
    /// <summary>
    /// Deletes the blueprint file
    /// </summary>
    /// <param name="fileName"></param>
    public void DeleteBlueprint(string fileName)
    {
        #if !NO_DEBUG
        if (savedBPList.FileExists(fileName))
        {
            path = BuildPathString(fileName);
            File.Delete(path);
            savedBPList.Remove(fileName);
            //optimize
            SaveSavesList();
        }
        else
        {
            Debug.LogError("Blueprint " + fileName + " not found");
        }
        #else //NO_DEBUG
            path = BuildPathString(fileName);
            File.Delete(path);
            savedBPList.Remove(fileName);
            SaveSavesList();
        #endif
    }
    /// <summary>
    /// Deletes all saved blueprints
    /// </summary>
    public void DeleteAllBlueprints()
    {
        for (int i = savedBPList.count - 1; i >= 0; i--)
        {
            DeleteBlueprint(savedBPList.blueprintMetaDataList[i].BlueprintName);
        }
    }
    #endregion Public

    #region Private
    /// <summary>
    /// Coverts the ShipBlueprint into a format that .Net's binary serializer can serialize
    /// </summary>
    /// <param name="shipBP">
    /// The blueprint to serialize
    /// </param>
    private void SerializeShipBP(ShipBlueprint shipBP)
    {
        sz_ShipBP.Clear();
        sz_ShipBP.hull_ID = HullTable.GetID(shipBP.Hull);
        foreach (var slot_component in shipBP.Slot_component_table)
        {
            sz_ShipBP.AddComponent(slot_component.Key.index, ComponentTable.GetID(slot_component.Value));
        }
        sz_ShipBP.metaData = shipBP.MetaData;
    }//Serialize

    /// <summary>
    /// Deserializes the loaded serialized ship blueprint
    /// </summary>
    /// <param name="sz_ShipBP">
    /// the serialized ship blueprint to de-serialize
    /// </param>
    /// <param name="shipBP">
    /// the blueprint to populate the deserialized shipBP into
    /// </param>
    private void DeSerializeSipBP(SerializedShipBlueprint sz_ShipBP, out ShipBlueprint shipBP)
    {
        shipBP = new ShipBlueprint(HullTable.GetHull(sz_ShipBP.hull_ID));
        shipBP.Hull.Init();
        foreach (var slotIndex_CompID in sz_ShipBP.slotIndex_CompID_Table)
        {
            #if FULL_DEBUG || LOW_DEBUG
            ShipComponent component = ComponentTable.GetComp(slotIndex_CompID.compID);
            ComponentSlot slot = shipBP.Hull.index_slot_table[slotIndex_CompID.slotIndex];
            #else
            ShipComponent component = ComponentTable.GetComp(slotIndex_CompID.Value);
            ComponentSlot slot = shipBP.hull.index_slot_table[slotIndex_CompID.Key];
            #endif
            shipBP.AddComponent(slot, component);
        }
        shipBP.MetaData = sz_ShipBP.metaData;
    }//DeSerialize
    
    /// <summary>
    /// Saves the current list of saved blueprints
    /// </summary>
    private void SaveSavesList()
    {
        path = BuildPathString(fileName_SaveList);
        fileStream = File.Create(path);
        #if FULL_DEBUG || LOW_DEBUG
        saveListSerializer.Serialize(fileStream, savedBPList);
        #else
        serializer.Serialize(fileStream, savedBPList);
        #endif
        fileStream.Close();
    }
    /// <summary>
    /// Loads the list of saved blueprints from HDD
    /// </summary>
    private void LoadSavesList()
    {
        path = BuildPathString(fileName_SaveList);
        if (File.Exists(path))
        {
            fileStream = File.Open(path, FileMode.Open);
            #if FULL_DEBUG || LOW_DEBUG
            savedBPList = saveListSerializer.Deserialize(fileStream) as SavedShipBPList;
            #else
            savedBPList = serializer.Deserialize(fileStream) as SavedShipBPList;
            #endif
            fileStream.Close();
        }
        else
        {
            #if FULL_DEBUG
            Debug.Log("No ShipBlueprint list found - initializing");
            #endif
            savedBPList = new SavedShipBPList();
        }
    }

    #region Helper
    /// <summary>
    /// Creates the directory to save blueprints into, unless it exists already
    /// </summary>
    private void CreateShipBPDirectory()
    {
        if (!Directory.Exists(Application.persistentDataPath + '/' + saveDirectory_ShipBP))
        {
            Directory.CreateDirectory(Application.persistentDataPath + '/' + saveDirectory_ShipBP);
            #if FULL_DEBUG
            Debug.Log("Creating Directory for Ship Blueprints: " + Application.persistentDataPath + '/' + saveDirectory_ShipBP);
            #endif
        }
    }
    /// <summary>
    /// Returns the full path to save a file into
    /// </summary>
    /// <param name="fileName">
    /// Name of the file to save
    /// </param>
    /// <returns>
    /// The full path to save the file into
    /// </returns>
    private string BuildPathString(string fileName)
    {
        return Application.persistentDataPath + '/' + saveDirectory_ShipBP + '/' + fileName + '.' + fileExtension_ShipBP;
    }
    #endregion Helper

    #endregion Private

    #endregion Methods

}
#region AdditionalStructs
[Serializable]
public class SavedShipBPList //keeps track of all the saves ship blueprints
{
    public int count { get; private set; } //more efficient that list.count
    public List<ShipBlueprintMetaData> blueprintMetaDataList { get; private set; }
    //public List<string> fileNames { get; private set; } //Filenames of all saved blueprints

    public SavedShipBPList()
    {
        count = 0;
        //fileNames = new List<string>();
        blueprintMetaDataList = new List<ShipBlueprintMetaData>();
    }
    public void Add(ShipBlueprintMetaData metaData)
    {
        count++;
        //fileNames.Add(fileName);
        blueprintMetaDataList.Add(new ShipBlueprintMetaData(metaData));
    }
    public void Remove(string fileName)
    {
        count--;
        //fileNames.Remove(fileName);
        #if FULL_DEBUG || LOW_DEBUG
        if(FileExists(fileName))
        {
            blueprintMetaDataList.Remove(blueprintMetaDataList.FirstOrDefault(b => b.BlueprintName == fileName));
        }
        else
        {
            Debug.LogError("Blueprint " + fileName + " not found");
        }
        #else
        blueprintMetaDataList.Remove(blueprintMetaDataList.FirstOrDefault(b => b.blueprintName == fileName));
        #endif
    }
    public bool FileExists(string fileName)
    {
        return blueprintMetaDataList.Exists(b => b.BlueprintName == fileName);
    }
}

#endregion AdditionalStructs

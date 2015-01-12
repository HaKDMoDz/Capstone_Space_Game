#region Usings
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Runtime.Serialization.Formatters.Binary;
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
    private BinaryFormatter binaryFormatter;
    private FileStream fileStream;
    private string path;
    private SerializedShipBlueprint sz_ShipBP;

    #endregion Internal
    #endregion Fields

    #region Methods
    #region Public
    public ShipBlueprintSaveSystem()
    {
        Init();
    }
    public ShipBlueprintSaveSystem(string fileExtension_ShipBP, string saveDirectory_ShipBP, string fileName_SaveList)
    {
        this.fileExtension_ShipBP = fileExtension_ShipBP;
        this.saveDirectory_ShipBP = saveDirectory_ShipBP;
        this.fileName_SaveList = fileName_SaveList;
        Init();
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
        binaryFormatter.Serialize(fileStream, sz_ShipBP);
        fileStream.Close();
        //update saves list
        savedBPList.Add(fileName);
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
            sz_ShipBP = binaryFormatter.Deserialize(fileStream) as SerializedShipBlueprint;
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
        for (int i = savedBPList.fileNames.Count - 1; i >= 0; i--)
        {
            DeleteBlueprint(savedBPList.fileNames[i]);
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
        sz_ShipBP.hull_ID = HullTable.GetID(shipBP.hull);
        foreach (var slot_component in shipBP.slot_component_table)
        {
            sz_ShipBP.AddComponent(slot_component.Key.index, ComponentTable.GetID(slot_component.Value));
        }
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
        //shipBP = new ShipBlueprint(id_hull_table[sz_ShipBP.hull_ID]);
        shipBP.hull.Init();
        foreach (var slotIndex_CompID in sz_ShipBP.slotIndex_CompID_Table)
        {
            ShipComponent component = ComponentTable.GetComp(slotIndex_CompID.Value);
            //ShipComponent component = id_comp_table[slotIndex_CompID.Value];
            ComponentSlot slot = shipBP.hull.index_slot_table[slotIndex_CompID.Key];
            shipBP.AddComponent(slot, component);
        }
    }//DeSerialize
    
    /// <summary>
    /// Saves the current list of saved blueprints
    /// </summary>
    private void SaveSavesList()
    {
        path = BuildPathString(fileName_SaveList);
        fileStream = File.Create(path);
        binaryFormatter.Serialize(fileStream, savedBPList);
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
            savedBPList = binaryFormatter.Deserialize(fileStream) as SavedShipBPList;
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
    private void Init()
    {
        binaryFormatter = new BinaryFormatter();
        sz_ShipBP = new SerializedShipBlueprint(0);
        CreateShipBPDirectory();
        LoadSavesList();
    }
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
    public int count = 0; //more efficient that list.count
    public List<ShipBlueprintMetaData> blueprintMetaDataList { get; private set; }
    public List<string> fileNames { get; private set; } //Filenames of all saved blueprints

    public SavedShipBPList()
    {
        count = 0;
        fileNames = new List<string>();
    }
    public void Add(string fileName)
    {
        count++;
        fileNames.Add(fileName);
    }
    public void Remove(string fileName)
    {
        count--;
        fileNames.Remove(fileName);
    }
    public bool FileExists(string fileName)
    {
        return fileNames.Contains(fileName);
    }
}
[Serializable]
public class SerializedShipBlueprint //serializable version of the ShipBlueprint
{
    public int hull_ID;
    public Dictionary<int, int> slotIndex_CompID_Table;

    public SerializedShipBlueprint(int hull_ID)
    {
        this.hull_ID = hull_ID;
        slotIndex_CompID_Table = new Dictionary<int, int>();
    }
    public void AddComponent(int slotIndex, int compID)
    {
        slotIndex_CompID_Table.Add(slotIndex, compID);
    }
    public void RemoveComponent(int slotIndex)
    {
#if !NO_DEBUG
        if (slotIndex_CompID_Table.ContainsKey(slotIndex))
        {
            slotIndex_CompID_Table.Remove(slotIndex);
        }
        #if FULL_DEBUG
        else
        {
            Debug.Log("slot " + slotIndex + " is not populated in the blueprint");
        }
        #endif
#else //NO_DEBUG
        slotIndex_CompID_Table.Remove(slotIndex);
#endif
    }//RemoveComp

    public void Clear()
    {
        hull_ID = -1;
        slotIndex_CompID_Table.Clear();
    }

}
#endregion AdditionalStructs

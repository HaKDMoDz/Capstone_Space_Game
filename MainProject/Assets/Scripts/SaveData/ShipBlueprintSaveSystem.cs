using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class ShipBlueprintSaveSystem
{
    #region Fields
    public SavedShipBPList savedBPList { get; private set; }

    #region Internal
    //saving info
    private string fileExtension_ShipBP = "sbp";
    private string saveDirectory_ShipBP = "ShipBlueprints";
    private string fileName_SaveList = "ShipBPFileList";

    //database references
    //private Dictionary<int, Hull> id_hull_table;
    //private Dictionary<Hull, int> hull_id_table;
    //private Dictionary<int, ShipComponent> id_comp_table;
    //private Dictionary<ShipComponent, int> comp_id_table;

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
        //update list
        savedBPList.Add(fileName);
        //optimize
        SaveSavesList();
    }

    public bool LoadBlueprint(out ShipBlueprint shipBP, string fileName)
    {
        path = BuildPathString(fileName);
        if(File.Exists(path))
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
#else
            path = BuildPathString(fileName);
            File.Delete(path);
            savedBPList.Remove(fileName);
            SaveSavesList();
#endif

    }
    public void DeleteAllBlueprints()
    {
        #if !NO_DEBUG
        Debug.LogWarning("Deleting all ship blueprints");
	    #endif
        foreach (string fileName in savedBPList.fileNames)
        {
            DeleteBlueprint(fileName);
        }
    }
    #endregion Public
    #region Private

    private void SerializeShipBP(ShipBlueprint shipBP)
    {
        sz_ShipBP.Clear();
        sz_ShipBP.hull_ID = HullTable.GetID(shipBP.hull);
        //sz_ShipBP.hull_ID = hull_id_table[shipBP.hull];
        foreach (var slot_component in shipBP.slot_component_table)
        {
            sz_ShipBP.AddComponent(slot_component.Key.index, ComponentTable.GetID(slot_component.Value));
            //sz_ShipBP.AddComponent(slot_component.Key.index, comp_id_table[slot_component.Value]);
        }
    }//Serialize

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

    private void SaveSavesList()
    {
        path = BuildPathString(fileName_SaveList);
        fileStream = File.Create(path);
        binaryFormatter.Serialize(fileStream, savedBPList);
        fileStream.Close();
    }

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
        //id_hull_table = ShipDesignSystem.Instance.id_hull_table;
        //hull_id_table = ShipDesignSystem.Instance.hull_id_table;
        //id_comp_table = ShipDesignSystem.Instance.id_comp_table;
        //comp_id_table = ShipDesignSystem.Instance.comp_id_table;
        sz_ShipBP = new SerializedShipBlueprint(0);
        CreateShipBPDirectory();
        //load list
        LoadSavesList();

    }
    private void CreateShipBPDirectory()
    {
        if(!Directory.Exists(Application.persistentDataPath+'/'+saveDirectory_ShipBP))
        {
            Directory.CreateDirectory(Application.persistentDataPath + '/' + saveDirectory_ShipBP);
            #if FULL_DEBUG
            Debug.Log("Creating Directory for Ship Blueprints: " + Application.persistentDataPath + '/' + saveDirectory_ShipBP);
            #endif
        }
    }
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
public class SavedShipBPList
{
    public int count = 0;
    public List<string> fileNames { get; private set; }

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
public class SerializedShipBlueprint
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
            //slot.InstalledComponent = null;
        }
        else
        {
            #if FULL_DEBUG
            Debug.Log("slot " + slotIndex + " is not populated in the blueprint");
            #endif
        }

#else
        slotIndex_CompID_Table.Remove(slotIndex);
#endif
    }//RemoveComp

    public void Clear()
    {
        hull_ID = -1;
        slotIndex_CompID_Table.Clear();
    }

}
//[Serializable]
//public class SerializedComponent
//{
//    public int ID;

//    public SerializedComponent(int ID)
//    {
//        this.ID =ID;
//    }
//}
#endregion AdditionalStructs

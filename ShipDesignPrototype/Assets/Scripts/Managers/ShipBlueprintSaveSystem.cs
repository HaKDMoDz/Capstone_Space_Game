using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;
using System.Text;

public class ShipBlueprintSaveSystem : Singleton<ShipBlueprintSaveSystem>
{
    [SerializeField]
    HullTable hullTableObject;
    [SerializeField]
    ComponentTable compTableObject;
    [SerializeField]
    string fileExtension_shipBP = "sbp";
    [SerializeField]
    string fileName_default = "ShipBP";
    [SerializeField]
    string saveFolder_shipBP = "ShipBlueprints";
    [SerializeField]
    string fileName_SaveList = "ShipBPFileList";

    SavedShipBPList savedBPList;
    public SavedShipBPList SavedBPList
    {
        get
        {
            LoadSavesList();
            return savedBPList;
        }
    }

    Dictionary<int, Hull> hullTable;
    Dictionary<int, ShipComponent> compTable;
    Dictionary<ShipComponent, int> compIDTable;

    //book-keeping vars
    BinaryFormatter bf;
    FileStream file;
    StringBuilder sb;
    string path;

    void Start()
    {
        hullTable = hullTableObject.HullTableProp
            .ToDictionary(h => h.ID, h => h.hull);
        compTable = compTableObject.ComponentList
            .ToDictionary(c => c.ID, c => c.component);
        compIDTable = compTableObject.ComponentList
            .ToDictionary(c => c.component, c => c.ID);

        bf = new BinaryFormatter();
        sb = new StringBuilder();

        LoadSavesList();

        //Debug.Log("Component object table");
        //foreach (var item in compTableObject.ComponentList)
        //{
        //    Debug.Log(item.component.ID + " : " + item.component.componentName);
        //}
        //Debug.Log("Component ID table");
        //foreach (var item in compIDTable)
        //{
        //    Debug.Log(item.Key + " : " + item.Value);
        //}
        //Debug.Log("Comp dictionary");
        //foreach (var item in compTable)
        //{
        //    Debug.Log(item.Value.ID + " : " + item.Value.componentName);
        //}

    }

    public void Save(ShipBlueprint shipBP, string fileName)
    {
        SerializedShipBlueprint sz_shipBP = SerializeShipBP(shipBP);
        CreateShipBPDirectory();
        fileName = fileName == "" ? fileName_default : fileName;

        path = BuildPathString(fileName);
        Debug.Log("Saving file to: " + path);
        file = File.Create(path);

        //Debug.Log("Saving file to: " + Application.persistentDataPath + "/ShipBlueprints/ShipBP1.sbp");
        //file = File.Create(Application.persistentDataPath + "/ShipBlueprints/ShipBP1.sbp");
        bf.Serialize(file, sz_shipBP);
        file.Close();

        //update file list

        savedBPList.Add(fileName);
        //for (int i = 0; i < savedBPList.count; i++)
        //{
        //    Debug.Log(savedBPList.fileNames[i]);
        //}
        SaveSavesList();
    }

    public bool Load(out ShipBlueprint shipBP, string fileName)
    {
        path = BuildPathString(fileName);
        if (File.Exists(path))
        //if (File.Exists(Application.persistentDataPath + "/ShipBlueprints/ShipBP1.sbp"))
        {
            file = File.Open(path, FileMode.Open);
            SerializedShipBlueprint sz_shipBP = bf.Deserialize(file) as SerializedShipBlueprint;
            shipBP = DeserializeShipBP(sz_shipBP);
            file.Close();
            return true;
        }
        else
        {
            shipBP = null;
            return false;
        }

    }

    void SaveSavesList()
    {
        CreateShipBPDirectory();
        path = BuildPathString(fileName_SaveList);
        file = File.Create(path);
        bf.Serialize(file, savedBPList);
        file.Close();
    }
    void LoadSavesList()
    {
        path = BuildPathString(fileName_SaveList);
        if (File.Exists(path))
        {
            file = File.Open(path, FileMode.Open);
            savedBPList = bf.Deserialize(file) as SavedShipBPList;
            file.Close();
        }
        else
        {
            savedBPList = new SavedShipBPList();
        }
    }

    public void DeleteBlueprints()
    {
        LoadSavesList();
        for (int i = 0; i < savedBPList.count; i++)
        {
            path = BuildPathString(savedBPList.fileNames[i]);
            File.Delete(path);
        }
        savedBPList = new SavedShipBPList();
        SaveSavesList();
    }
    string BuildPathString(string fileName)
    {
        sb.Length = 0;
        sb.Append(Application.persistentDataPath);
        sb.Append('/');
        sb.Append(saveFolder_shipBP);
        sb.Append('/');
        sb.Append(fileName);
        sb.Append('.');
        sb.Append(fileExtension_shipBP);
        return sb.ToString();
    }

    void CreateShipBPDirectory()
    {
        if (!Directory.Exists(Application.persistentDataPath + "/ShipBlueprints"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/ShipBlueprints");
        }
    }

    SerializedShipBlueprint SerializeShipBP(ShipBlueprint ship)
    {

        SerializedShipBlueprint sz_shipBP = new SerializedShipBlueprint(ship.Hull.ID);
        sz_shipBP.componentTable = new Dictionary<int, SerializedComponent>();
        foreach (var item in ship.ComponentTable)
        {
            SerializedComponent sz_comp = new SerializedComponent(compIDTable[item.Value]);
            sz_shipBP.componentTable.Add(item.Key.index, sz_comp);
        }
        return sz_shipBP;
    }

    ShipBlueprint DeserializeShipBP(SerializedShipBlueprint sz_shipBP)
    {
        Hull hull = hullTable[sz_shipBP.hull_ID];
        hull.Init();
        //hull.OutputSlotTable();
        ShipBlueprint shipBP = new ShipBlueprint(hull);

        foreach (var item in sz_shipBP.componentTable)
        {

            ShipComponent comp = compTable[item.Value.ID];
            //Debug.Log("Deserialzing - adding comp to slot " + item.Key);
            shipBP.AddComponent(item.Key, comp);
        }
        return shipBP;
    }

}
[Serializable]
public class SavedShipBPList
{
    public int count;
    public List<string> fileNames;
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


}


[Serializable]
public class SerializedShipBlueprint
{
    public int hull_ID;
    public Dictionary<int, SerializedComponent> componentTable;

    public SerializedShipBlueprint(int _hullID)
    {
        hull_ID = _hullID;
    }
    public void OutputContents()
    {
        if (componentTable != null)
        {
            Debug.Log("Hull ID: " + hull_ID);
            foreach (var item in componentTable)
            {
                Debug.Log(item.Key + ": " + item.Value.ID);
            }
        }
        else
        {
            Debug.Log("Sz_Shipblueprint is null");
        }
    }
}
[Serializable]
public class SerializedComponent
{
    public int ID;

    public SerializedComponent(int _ID)
    {
        ID = _ID;
    }
}
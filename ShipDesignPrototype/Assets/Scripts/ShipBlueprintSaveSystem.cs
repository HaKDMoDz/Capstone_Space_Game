using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;

public class ShipBlueprintSaveSystem : SingletonComponent<ShipBlueprintSaveSystem>
{
    [SerializeField]
    HullTable hullTableObject;
    [SerializeField]
    ComponentTable compTableObject;

    Dictionary<int, Hull> hullTable;
    Dictionary<int, ShipComponent> compTable;

    void Start()
    {

        hullTable = hullTableObject.HullTableProp
            .ToDictionary(h => h.ID, h => h.hull);
        compTable = compTableObject.ComponentList
            .ToDictionary(c => c.ID, c => c.component);

    }

    public void Save(ShipBlueprint shipBP)
    {
        SerializedShipBlueprint sz_shipBP = SerializeShipBP(shipBP);
        BinaryFormatter bf = new BinaryFormatter();
        Debug.Log("Saving file to: " + Application.persistentDataPath);
        FileStream file = File.Create(Application.persistentDataPath + "/ShipBP1.sbp");
        bf.Serialize(file, sz_shipBP);
        file.Close();

    }
    public bool Load(out ShipBlueprint shipBP)
    {

        if (File.Exists(Application.persistentDataPath + "/ShipBP1.sbp"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/ShipBP1.sbp", FileMode.Open);
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

    SerializedShipBlueprint SerializeShipBP(ShipBlueprint ship)
    {

        SerializedShipBlueprint sz_shipBP = new SerializedShipBlueprint(ship.Hull.ID);
        sz_shipBP.componentTable = new Dictionary<int, SerializedComponent>();
        foreach (var item in ship.ComponentTable)
        {
            SerializedComponent sz_comp = new SerializedComponent(item.Value.ID);
            sz_shipBP.componentTable.Add(item.Key.index, sz_comp);
        }
        return sz_shipBP;
    }

    ShipBlueprint DeserializeShipBP(SerializedShipBlueprint sz_shipBP)
    {
        Hull hull = hullTable[sz_shipBP.hull_ID];
        ShipBlueprint shipBP = new ShipBlueprint( hull);
        foreach (var item in sz_shipBP.componentTable)
        {
            ShipComponent comp = compTable[item.Value.ID];
            shipBP.AddComponent(item.Key, comp);
        }
        return shipBP;
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
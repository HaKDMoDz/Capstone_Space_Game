using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class PlayerFleetData
{
    public Dictionary<int, ShipBlueprintMetaData> gridIndex_metaData_table = new Dictionary<int, ShipBlueprintMetaData>();

    public PlayerFleetData()
    {
    }

    public void Serialize(ref SerializedPlayerFleetData sz_playerFleetData)
    {
#if FULL_DEBUG
        sz_playerFleetData.gridIndex_metaData_List.Clear();
        foreach (var gridIndex_MetaData in gridIndex_metaData_table)
        {
            GridIndex_MetaData item = new GridIndex_MetaData(gridIndex_MetaData.Key, gridIndex_MetaData.Value);
            sz_playerFleetData.gridIndex_metaData_List.Add(item);
        }        
#else
        sz_playerFleetData.gridIndex_metaData_List = gridIndex_metaData_table;    
#endif
    }
}

[Serializable]
public class SerializedPlayerFleetData
{
#if FULL_DEBUG || LOW_DEBUG
    public List<GridIndex_MetaData> gridIndex_metaData_List = new List<GridIndex_MetaData>();
#else
    public Dictionary<int, ShipBlueprintMetaData> gridIndex_metaData_table = new Dictionary<int, ShipBlueprintMetaData>();
#endif

    public SerializedPlayerFleetData()
    {

    }
    public void DeSerialize(ref PlayerFleetData playerFleetData)
    {
#if FULL_DEBUG || LOW_DEBUG
        playerFleetData.gridIndex_metaData_table = gridIndex_metaData_List.ToDictionary(item => item.gridIndex, item => item.metaData);
#else
        playerFleetData.gridIndex_metaData_table = gridIndex_metaData_table;
#endif

    }
}
#if FULL_DEBUG || LOW_DEBUG
[Serializable]
public class GridIndex_MetaData
{
    public int gridIndex;
    public ShipBlueprintMetaData metaData;
    public GridIndex_MetaData()
    {
        gridIndex = -1;
        metaData = new ShipBlueprintMetaData();
    }
    public GridIndex_MetaData(int gridIndex, ShipBlueprintMetaData metaData)
    {
        this.gridIndex = gridIndex;
        this.metaData = metaData;
    }
}
#endif

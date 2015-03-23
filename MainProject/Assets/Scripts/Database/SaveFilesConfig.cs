/*
  SaveFilesConfig.cs
  Mission: Invasion
  Created by Rohun Banerji on Jan 16/2015
  Copyright (c) 2015 Rohun Banerji. All rights reserved.
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SaveFilesConfig : ScriptableObject
{
    //GameSaves
    [SerializeField]
    private string fileExtension_GameSave;
    [SerializeField]
    private string directory_GameSave;
    [SerializeField]
    private string fileName_GameSavesList;
    [SerializeField]
    private string autoSaveFileName;
    [SerializeField]
    private int numAutoSaves;
    [SerializeField]
    private string quickSaveName;
    [SerializeField]
    private int numQuickSaves;
    [SerializeField]
    private int numNormalSaves;

    public static string FileExtension_GameSave { get; private set; }
    public static string Directory_GameSave{ get; private set; }
    public static string FileName_GameSavesList { get; private set; }
    public static string AutoSaveFileName { get; private set; }
    public static int NumAutoSaves { get; private set; }
    public static string QuickSaveName { get; private set; }
    public static int NumQuickSaves { get; private set; }
    public static int NumNormalSaves { get; private set; }
    //GameSaves

    //BlueprintSaves
    [SerializeField]
    private string fileExtension_ShipBP;
    [SerializeField]
    private string directory_ShipBP;
    [SerializeField]
    private string fileName_ShipBP_SaveList;

    public static string FileExtension_ShipBP { get; private set; }
    public static string Directory_ShipBP { get; private set; }
    public static string FileName_ShipBP_SaveList { get; private set; }
    //BlueprintSaves

    //Settings
    [SerializeField]
    private string fileExtension_Settings = "ini";
    [SerializeField]
    private string directory_Settings = "Settings";
    [SerializeField]
    private string fileName_settings = "Settings";

    public static string FileExtension_Settings { get; private set; }
    public static string Directory_Settings { get; private set; }
    public static string FileName_settings { get; private set; }
    private void OnEnable()
    {
        FileExtension_GameSave = fileExtension_GameSave;
        Directory_GameSave = directory_GameSave;
        FileName_GameSavesList = fileName_GameSavesList;
        AutoSaveFileName = autoSaveFileName;
        NumAutoSaves = numAutoSaves;
        QuickSaveName = quickSaveName;
        NumQuickSaves = numQuickSaves;
        NumNormalSaves = numNormalSaves;

        FileExtension_ShipBP= fileExtension_ShipBP;
        Directory_ShipBP = directory_ShipBP;
        FileName_ShipBP_SaveList = fileName_ShipBP_SaveList;

        FileExtension_Settings = fileExtension_Settings;
        Directory_Settings = directory_Settings;
        FileName_settings = fileName_settings;
    }

}

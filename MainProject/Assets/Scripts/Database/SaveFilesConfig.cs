using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SaveFilesConfig : ScriptableObject
{
    #region GameSaves
    public string fileExtension_GameSave;
    public string directory_GameSave;
    public string fileName_GameSavesList;
    public string autoSaveFileName;
    public int numAutoSaves;
    public string quickSaveName;
    public int numQuickSaves;
    public int numNormalSaves;

    public static string FileExtension_GameSave;
    public static string Directory_GameSave;
    public static string FileName_GameSavesList;
    public static string AutoSaveFileName;
    public static int NumAutoSaves;
    public static string QuickSaveName;
    public static int NumQuickSaves;
    public static int NumNormalSaves;
    #endregion GameSaves

    #region BlueprintSaves
    public string fileExtension_ShipBP;
    public string directory_ShipBP;
    public string fileName_ShipBP_SaveList;

    public static string FileExtension_ShipBP;
    public static string Directory_ShipBP;
    public static string FileName_ShipBP_SaveList;
    #endregion BlueprintSaves

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
    }

}

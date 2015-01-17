using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SaveFilesConfig : ScriptableObject
{
    #region GameSaves
    public string gameSaveFileExtension;
    public string gameSaveDirectory;
    public string fileName_GameSavesList;
    public string autoSaveFileName;
    public int numAutoSaves;
    public string quickSaveName;
    public int numQuickSaves;
    public int numNormalSaves;

    public static string GameSaveFileExtension;
    public static string GameSaveDirectory;
    public static string FileName_GameSavesList;
    public static string AutoSaveFileName;
    public static int NumAutoSaves;
    public static string QuickSaveName;
    public static int NumQuickSaves;
    public static int NumNormalSaves;
    #endregion GameSaves

    #region BlueprintSaves
    public string fileExtension_ShipBP;
    public string saveDirectory_ShipBP;
    public string fileName_ShipBP_SaveList;

    public static string FileExtension_ShipBP;
    public static string SaveDirectory_ShipBP;
    public static string FileName_ShipBP_SaveList;
    #endregion BlueprintSaves

    private void OnEnable()
    {
        GameSaveFileExtension = gameSaveFileExtension;
        GameSaveDirectory = gameSaveDirectory;
        FileName_GameSavesList = fileName_GameSavesList;
        AutoSaveFileName = autoSaveFileName;
        NumAutoSaves = numAutoSaves;
        QuickSaveName = quickSaveName;
        NumQuickSaves = numQuickSaves;
        NumNormalSaves = numNormalSaves;

        FileExtension_ShipBP= fileExtension_ShipBP;
        SaveDirectory_ShipBP = saveDirectory_ShipBP;
        FileName_ShipBP_SaveList = fileName_ShipBP_SaveList;
    }

}

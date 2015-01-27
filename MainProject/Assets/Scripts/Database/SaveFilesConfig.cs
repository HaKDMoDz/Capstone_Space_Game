using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SaveFilesConfig : ScriptableObject
{
    #region GameSaves
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
    #endregion GameSaves

    #region BlueprintSaves
    [SerializeField]
    private string fileExtension_ShipBP;
    [SerializeField]
    private string directory_ShipBP;
    [SerializeField]
    private string fileName_ShipBP_SaveList;

    public static string FileExtension_ShipBP { get; private set; }
    public static string Directory_ShipBP { get; private set; }
    public static string FileName_ShipBP_SaveList { get; private set; }
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

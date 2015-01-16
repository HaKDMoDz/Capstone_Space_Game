using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class OpenSavesLocation  
{
    [MenuItem("Custom/Saves/Open Saves Location")]
    static void OpenSaveLocation()
    {
        string path = Application.persistentDataPath.Replace("/", "\\");
        System.Diagnostics.Process.Start("explorer.exe", "/select," + path);
    }
    [MenuItem("Custom/Saves/Delete All Saves")]
    static void DeleteAllSaves()
    {
        Debug.LogWarning("Deleting all saves");
        Directory.Delete(Application.persistentDataPath, true);
    }
}

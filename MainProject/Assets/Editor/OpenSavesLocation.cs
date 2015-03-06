/*
  OpenSavesLocation.cs
  Mission: Invasion
  Created by Rohun Banerji on Jan 12/2015
  Copyright (c) 2015 Rohun Banerji. All rights reserved.
*/

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
        if (EditorUtility.DisplayDialog("Confirm Saves Deletion", "Are you sure you want to delete all save files?", "Delete", "Cancel"))
        {
            Debug.LogWarning("Deleting all saves");
            Directory.Delete(Application.persistentDataPath, true);
        }
    }
}

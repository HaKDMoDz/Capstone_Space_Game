using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class OpenSavesLocation  
{
    [MenuItem("Custom/Open Saves Location")]
    static void OpenSaveLocation()
    {
        string path = Application.persistentDataPath.Replace("/", "\\");
        System.Diagnostics.Process.Start("explorer.exe", "/select," + path);
    }
	
}

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public class ScriptTemplateImporter : UnityEditor.AssetModificationProcessor 
{
    public static void OnWillCreateAsset(string path)
    {
        path = path.Replace(".meta", "");
        int index = path.LastIndexOf(".");
        string ext = path.Substring(index);
        if (ext != ".cs") return;
        index = Application.dataPath.LastIndexOf("Assets");
        path = Application.dataPath.Substring(0, index) + path;
        Debug.Log("path: " + path);
        string file = File.ReadAllText(path);
        file = file.Replace("#CREATIONDATE#", DateTime.Now.ToShortDateString());
        file = file.Replace("#PROJECTNAME#", PlayerSettings.productName);
        file = file.Replace("#YEAR#", DateTime.Now.Year.ToString());
        File.WriteAllText(path, file);
        AssetDatabase.Refresh();
    }

}

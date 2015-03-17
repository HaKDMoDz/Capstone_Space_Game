/*
  ScriptTemplateImporter.cs
  Mission: Invasion
  Created by Rohun Banerji on March 7/2015.
  Copyright (c) 2015 Rohun Banerji. All rights reserved.
*/

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
        //not a script
        if (!path.Contains(".cs")) return;

        path = path.Replace(".meta", "");
        int index = path.LastIndexOf(".");
        string ext = path.Substring(index);
        if (ext != ".cs") return;
        index = Application.dataPath.LastIndexOf("Assets");
        path = Application.dataPath.Substring(0, index) + path;
        Debug.Log("path: " + path);
        string file = File.ReadAllText(path);
        file = file.Replace("#CREATIONDATE#", DateTime.Now.ToString("MMMM dd, yyyy"));
        file = file.Replace("#PROJECTNAME#", PlayerSettings.productName);
        file = file.Replace("#YEAR#", DateTime.Now.Year.ToString());
        File.WriteAllText(path, file);
        AssetDatabase.Refresh();
    }

}

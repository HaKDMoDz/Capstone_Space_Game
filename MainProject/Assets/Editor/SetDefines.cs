/*
  SetDefines.cs
  Mission: Invasion
  Created by Rohun Banerji on Dec 18/2014
  Copyright (c) 2014 Rohun Banerji. All rights reserved.
*/

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;

public class SetDefines
{
    const string fullDebug = "FULL_DEBUG";
    const string lowDebug = "LOW_DEBUG";
    const string release = "RELEASE";
    const string noDebug = "NO_DEBUG";

    [MenuItem("Custom/Defines/Full Debug")]
    private static void SetFullDebug()
    {
        Debug.Log("Set "+fullDebug);
        SetDefinesForAllBuilds(fullDebug, lowDebug, release);
    }

    [MenuItem("Custom/Defines/Low Debug")]
    private static void SetLowDebug()
    {
        Debug.Log("Set "+lowDebug);
        SetDefinesForAllBuilds(lowDebug, release);
    }

    [MenuItem("Custom/Defines/Release")]
    private static void SetRelease()
    {
        Debug.Log("Set "+release);
        SetDefinesForAllBuilds(release);
    }

    [MenuItem("Custom/Defines/No Debug")]
    private static void SetNoDebug()
    {
        Debug.Log("Set "+noDebug);
        SetDefinesForAllBuilds(noDebug);
    }

    private static void SetDefinesForAllBuilds(params string[] defineNames)
    {
        string defines = string.Join(";", defineNames);
        foreach (BuildTargetGroup buildTarget in Enum.GetValues(typeof(BuildTargetGroup)))
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTarget, defines);
        }
    }

}

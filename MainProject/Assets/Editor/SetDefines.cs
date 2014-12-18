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

    [MenuItem("Defines/Full Debug")]
    private static void SetFullDebug()
    {
        Debug.Log("Set full debug");
        SetDefinesForAllBuilds(fullDebug, lowDebug, release);
    }

    [MenuItem("Defines/Low Debug")]
    private static void SetLowDebug()
    {
        Debug.Log("Set low debug");
        SetDefinesForAllBuilds(lowDebug, release);
    }

    [MenuItem("Defines/Release")]
    private static void SetRelease()
    {
        Debug.Log("Set release");
        SetDefinesForAllBuilds(release);
    }

    [MenuItem("Defines/No Debug")]
    private static void SetNoDebug()
    {
        Debug.Log("Set no debug");
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

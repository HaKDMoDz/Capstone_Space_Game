/*
  LoadingScreen.cs
  Mission: Invasion
  Created by Rohun Banerji on Mar 4/2015
  Copyright (c) 2015 Rohun Banerji. All rights reserved.
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LoadingScreen : MonoBehaviour 
{
    [SerializeField]
    private FillBar loadingbar;

    private AsyncOperation async;
    private string levelToLoad;
    private bool toLoadLevel = false;
    public void LoadLevel(string level)
    {
        levelToLoad = level;
        toLoadLevel = true;
    }

    private IEnumerator LoadLevelAsync()
    {
        Debug.LogWarning("Loading sync level " + levelToLoad);
        loadingbar.SetValue(0.0f);
        async = Application.LoadLevelAsync(levelToLoad);
        while (!async.isDone)
        {
            loadingbar.SetValue(async.progress);
            Debug.Log("Loading Progress: " + async.progress);
            yield return null;
        }
    }
    
    void Update()
    {
        if (toLoadLevel)
        {
            toLoadLevel = false;
            StartCoroutine(LoadLevelAsync());
        }
    }

}

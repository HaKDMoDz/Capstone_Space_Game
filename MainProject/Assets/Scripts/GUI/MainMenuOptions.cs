/*
  MainMenuOptions.cs
  Mission: Invasion
  Created by Rohun Banerji on March 20, 2015.
  Copyright (c) 2015 Rohun Banerji. All rights reserved.
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainMenuOptions : MonoBehaviour 
{

    public void SetQualityLevel(int index)
    {
        QualitySettings.SetQualityLevel(index);
        Debug.Log("Quality Level: " + QualitySettings.names[QualitySettings.GetQualityLevel()]);
    }
    public void SetAntiAliasing(int aaLevel)
    {
#if FULL_DEBUG
        if(aaLevel != 0 && aaLevel != 2 && aaLevel != 4 && aaLevel !=8)
        {
            Debug.LogError("AA level should be: 0, 2, 4 or 8 but is " + aaLevel);
            return;
        }
#endif
        QualitySettings.antiAliasing = aaLevel;
        Debug.Log("AA level: " + QualitySettings.antiAliasing);
    }
    public void SetTripleBuffering(bool set)
    {
        QualitySettings.maxQueuedFrames = set ? 3 : 0; 
        Debug.Log("Triple buffering: " + QualitySettings.maxQueuedFrames);
    }
    public void SetAnistropicFiltering(bool set)
    {
        QualitySettings.anisotropicFiltering = set ? AnisotropicFiltering.ForceEnable : AnisotropicFiltering.Disable;
        Debug.Log("Anistropic Filtering: " + QualitySettings.anisotropicFiltering);
    }
    public void SetResolution(int width, int height, int refreshRate)
    {
        Screen.SetResolution(width, height, Screen.fullScreen, refreshRate);
        Debug.Log("Resolution: " + Screen.currentResolution);
    }
    public void SetVSync(bool set)
    {
        QualitySettings.vSyncCount = set ? 1 : 0;
        Debug.Log("VSync: " + QualitySettings.vSyncCount);
    }
    public void SetFullScreen(bool set)
    {
        Screen.fullScreen = set;
        Debug.Log("Full screen: " + Screen.fullScreen);
    }

}

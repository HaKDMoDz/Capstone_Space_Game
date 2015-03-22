/*
  MainMenuOptions.cs
  Mission: Invasion
  Created by Rohun Banerji on March 20, 2015.
  Copyright (c) 2015 Rohun Banerji. All rights reserved.
*/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
public class MainMenuOptions : Singleton<MainMenuOptions>
{
    public enum TextureQuality { Full=0, Half=1, Quater=2, Eight=3}
    #region Fields
    //Editor Exposed
    [SerializeField]
    private ButtonWithContent dropDownButtonPrefab;
    [SerializeField]
    private Toggle muteMaster;
    [SerializeField]
    private Slider masterVolume;
    [SerializeField]
    private Toggle muteMusic;
    [SerializeField]
    private Slider musicVolume;
    [SerializeField]
    private Toggle muteEffects;
    [SerializeField]
    private Slider effectsVolume;
    [SerializeField]
    private ButtonWithContent currentResButton;
    [SerializeField]
    private RectTransform resButtonsParent;
    [SerializeField]
    private Toggle fullScreenToggle;
    [SerializeField]
    private ButtonWithContent currentQualButton;
    [SerializeField]
    private RectTransform qualButtonsParent;
    [SerializeField]
    private Toggle vsyncToggle;
    [SerializeField]
    private ButtonWithContent currentTexButton;
    [SerializeField]
    private RectTransform texButtonsParent;
    #endregion Fields

    #region UI Builder
    public void Init()
    {
        Debug.Log(QualitySettings.masterTextureLimit);
        SetupGUI();
    }
    private void SetupGUI()
    {
        SetupResolutionDropDown();
        SetupQualityDropDown();
    }
    private void SetupResolutionDropDown()
    {
        currentResButton.AddOnClickListener(()=>OpenResolutionDropDown(true));
        foreach (Resolution res in Screen.resolutions)
        {
            int width = res.width;
            int height = res.height;
            int refreshRate = res.refreshRate;
            ButtonWithContent buttonClone = (ButtonWithContent)Instantiate(dropDownButtonPrefab);
            buttonClone.transform.SetParent(resButtonsParent, false);
            buttonClone.SetText(width + "x" + height + "(" + refreshRate + ")");
            buttonClone.AddOnClickListener(()=>
                {
                    SetResolution(width, height, refreshRate);
                    currentResButton.SetText(width + "x" + height + "(" + refreshRate + ")");
                    currentResButton.RemoveOnClickListeners();
                    currentResButton.AddOnClickListener(()=>OpenResolutionDropDown(true));
                    OpenResolutionDropDown(false);
                });
            Debug.Log("Created res button: " + width + "x" + height + "(" + refreshRate + ")");
        }
    }
    private void OpenResolutionDropDown(bool open)
    {
        Debug.Log("open res drop down " + open);
        resButtonsParent.gameObject.SetActive(open);
        currentResButton.RemoveOnClickListeners();
        currentResButton.AddOnClickListener(() =>
            {
                OpenResolutionDropDown(!open);
            });
    }
    private void SetupQualityDropDown()
    {
        currentQualButton.AddOnClickListener(() => OpenQualityDropDown(true));
        string[] qualityNames = QualitySettings.names;
        for (int i = 0; i < qualityNames.Length; i++)
        {
            int qualIndex = i;
            ButtonWithContent buttonClone = (ButtonWithContent)Instantiate(dropDownButtonPrefab);
            buttonClone.transform.SetParent(qualButtonsParent, false);
            buttonClone.SetText(qualityNames[qualIndex]);
            buttonClone.AddOnClickListener(() =>
            {
                SetQualityLevel(qualIndex);
                currentQualButton.SetText(qualityNames[qualIndex]);
                OpenQualityDropDown(false);
            });
        }
    }
    private void OpenQualityDropDown(bool open)
    {
        Debug.Log("Open qual drop down: " + open);
        qualButtonsParent.gameObject.SetActive(open);
        currentQualButton.RemoveOnClickListeners();
        currentQualButton.AddOnClickListener(() =>
            {
                OpenQualityDropDown(!open);
            });
    }
    private void SetupTexQualDropDown()
    {
        currentTexButton.AddOnClickListener(()=>OpenTexQualityDropDown(true));
        foreach (TextureQuality textureQuality in Enum.GetValues(typeof(TextureQuality)))
        {
            TextureQuality texQual = textureQuality;
            ButtonWithContent buttonClone = (ButtonWithContent)Instantiate(dropDownButtonPrefab);
            buttonClone.transform.SetParent(texButtonsParent, false);
            buttonClone.SetText(texQual.ToString());
            buttonClone.AddOnClickListener(() =>
                {
                    SetTextureQuality(texQual);
                    currentResButton.SetText(texQual.ToString());
                    OpenTexQualityDropDown(false);
                });
        }
    }
    private void OpenTexQualityDropDown(bool open)
    {
        texButtonsParent.gameObject.SetActive(open);
        currentTexButton.RemoveOnClickListeners();
        currentTexButton.AddOnClickListener(()=>OpenTexQualityDropDown(!open));
    }
    #endregion UI Builder

    #region Audio
    public void MuteMaster()
    {
        Debug.Log("Mute master " + muteMaster.isOn);
        foreach (var item in Screen.resolutions)
        {
            Debug.Log(item.width + "x" + item.height + "(" + item.refreshRate + ")");
        }
    }
    public void SetMasterVolume()
    {
        Debug.Log("Master Volume: " + masterVolume.value);
    }
    public void MuteMusic()
    {
        Debug.Log("Mute music " + muteMusic.isOn);
    }
    public void SetMusicVol()
    {
        Debug.Log("Music vol: " + musicVolume.value);
    }
    public void MuteEffects()
    {
        Debug.Log("Mute Effects " + muteEffects.isOn);
    }
    public void SetEffectsVol()
    {
        Debug.Log("Effects Vol " + effectsVolume.value);
    }
    #endregion Audio

    #region Video

    public void SetQualityLevel(int index)
    {
        QualitySettings.SetQualityLevel(index);
        Debug.Log("Quality Level: " + QualitySettings.names[QualitySettings.GetQualityLevel()]);
    }
    public void SetAntiAliasing(int aaLevel)
    {
#if FULL_DEBUG
        if (aaLevel != 0 && aaLevel != 2 && aaLevel != 4 && aaLevel != 8)
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
        Debug.Log("Resolution: " + Screen.currentResolution.width + "x" + Screen.currentResolution.height);
    }
    public void SetVSync()
    {
        QualitySettings.vSyncCount = vsyncToggle.isOn ? 1 : 0;
        Debug.Log("VSync: " + QualitySettings.vSyncCount);
    }
    public void SetFullScreen()
    {
        Screen.fullScreen = fullScreenToggle.isOn;
        Debug.Log("Full screen: Button: "+fullScreenToggle.isOn+ " Screen: " + Screen.fullScreen);
    }
    public void SetTextureQuality(TextureQuality texQuality)
    {
        QualitySettings.masterTextureLimit = (int)texQuality;
        Debug.Log("Texture Quality: " + QualitySettings.masterTextureLimit);
    }
    #endregion Video


}

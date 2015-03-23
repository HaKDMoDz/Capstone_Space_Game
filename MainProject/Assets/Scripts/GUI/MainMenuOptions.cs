/*
  MainMenuOptions.cs
  Mission: Invasion
  Created by Rohun Banerji on March 20, 2015.
  Copyright (c) 2015 Rohun Banerji. All rights reserved.
*/
#region Usings
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
#endregion Usings
public enum TextureQuality { Full = 0, Half = 1, Quater = 2 }

public class MainMenuOptions : Singleton<MainMenuOptions>
{
    private int[] aaValues = { 8,4,2,0};
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
    [SerializeField]
    private ButtonWithContent currentAAButton;
    [SerializeField]
    private RectTransform aaButtonsParent;
    [SerializeField]
    private Toggle anisToggle;
    [SerializeField]
    private Toggle tripBuffToggle;

    //References
    GameSettings settings;
    #endregion Fields

    #region UI Builder
    public void Init()
    {
        SetupGUI();
        InputManager.Instance.RegisterMouseButtonsDown((button) => CloseAllDropDowns(), MouseButton.Left, MouseButton.Middle, MouseButton.Right);
        settings = new GameSettings();
        settings.LoadSettings();
        SetGUIFromSettingsFile();
    }
    private void SetGUIFromSettingsFile()
    {
        muteMaster.isOn = settings.MuteMaster;
        masterVolume.value = settings.MasterVolume;
        muteMusic.isOn = settings.MuteMusic;
        musicVolume.value = settings.MusicVolume;
        muteEffects.isOn = settings.MuteEffects;
        effectsVolume.value = settings.EffectsVolume;
        currentResButton.SetText(settings.ScreenWidth + "x" + settings.ScreenHeight + "(" + settings.RefreshRate + ")");
        SetResolution(settings.ScreenWidth, settings.ScreenHeight, settings.RefreshRate);
        fullScreenToggle.isOn = settings.FullScreen;
        SetFullScreen();
        currentQualButton.SetText(QualitySettings.names[settings.QualityPreset]);
        SetQualityLevel(settings.QualityPreset);
        vsyncToggle.isOn = settings.Vsync;
        SetVSync();
        currentTexButton.SetText(settings.TexQuality.ToString());
        SetTextureQuality(settings.TexQuality);
        currentAAButton.SetText(settings.AAValue.ToString() + 'x');
        SetAntiAliasing(settings.AAValue);
        anisToggle.isOn = settings.AnisoFiltering;
        SetAnistropicFiltering();
        tripBuffToggle.isOn = settings.TripleBuffering;
        SetTripleBuffering();
    }
    private void SetupGUI()
    {
        SetupResolutionDropDown();
        SetupQualityDropDown();
        SetupTexQualDropDown();
        SetupAADropDown();
    }
    private void CloseAllDropDowns()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = Input.mousePosition;
        List<RaycastResult> rayCastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, rayCastResults);
        if (!(rayCastResults
            .Select(result => result.gameObject)
            .Any(go=>go.tag == TagsAndLayers.DropDownButtonTag)))
        {
            OpenResolutionDropDown(false);
            currentResButton.RemoveOnClickListeners();
            currentResButton.AddOnClickListener(() => OpenResolutionDropDown(true));
            OpenQualityDropDown(false);
            currentQualButton.RemoveOnClickListeners();
            currentQualButton.AddOnClickListener(() => OpenQualityDropDown(true));
            OpenTexQualityDropDown(false);
            currentTexButton.RemoveOnClickListeners();
            currentTexButton.AddOnClickListener(() => OpenTexQualityDropDown(true));
            OpenAADropDown(false);
            currentAAButton.RemoveOnClickListeners();
            currentAAButton.AddOnClickListener(() => OpenAADropDown(true));
        }
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
            buttonClone.tag = TagsAndLayers.DropDownButtonTag;
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
        //Debug.Log("open res drop down " + open);
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
        for (int i = qualityNames.Length-1; i >= 0; i--)
        {
            int qualIndex = i;
            ButtonWithContent buttonClone = (ButtonWithContent)Instantiate(dropDownButtonPrefab);
            buttonClone.tag = TagsAndLayers.DropDownButtonTag;
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
        //Debug.Log("Open qual drop down: " + open);
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
            buttonClone.tag = TagsAndLayers.DropDownButtonTag;
            buttonClone.transform.SetParent(texButtonsParent, false);
            buttonClone.SetText(texQual.ToString());
            buttonClone.AddOnClickListener(() =>
                {
                    SetTextureQuality(texQual);
                    currentTexButton.SetText(texQual.ToString());
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
    private void SetupAADropDown()
    {
        currentAAButton.AddOnClickListener(() => OpenAADropDown(true));
        foreach (int aa in aaValues)
        {
            int aaValue = aa;
            ButtonWithContent buttonClone = (ButtonWithContent)Instantiate(dropDownButtonPrefab);
            buttonClone.tag = TagsAndLayers.DropDownButtonTag;
            buttonClone.transform.SetParent(aaButtonsParent, false);
            buttonClone.SetText(aaValue.ToString()+'x');
            buttonClone.AddOnClickListener(() =>
                {
                    SetAntiAliasing(aaValue);
                    currentAAButton.SetText(aaValue.ToString() + 'x');
                    OpenAADropDown(false);
                });
        }
    }
    private void OpenAADropDown(bool open)
    {
        aaButtonsParent.gameObject.SetActive(open);
        currentAAButton.RemoveOnClickListeners();
        currentAAButton.AddOnClickListener(()=>OpenAADropDown(!open));
    }
    #endregion UI Builder

    public void SaveSettings()
    {
        settings.SaveSettings();
    }

    #region Audio
    public void MuteMaster()
    {
        Debug.Log("Mute master " + muteMaster.isOn);
        settings.MuteMaster = muteMaster.isOn;
    }
    public void SetMasterVolume()
    {
        Debug.Log("Master Volume: " + masterVolume.value);
        settings.MasterVolume = masterVolume.value;
    }
    public void MuteMusic()
    {
        Debug.Log("Mute music " + muteMusic.isOn);
        settings.MuteMusic = muteMusic.isOn;
    }
    public void SetMusicVol()
    {
        Debug.Log("Music vol: " + musicVolume.value);
        settings.MusicVolume = musicVolume.value;
    }
    public void MuteEffects()
    {
        Debug.Log("Mute Effects " + muteEffects.isOn);
        settings.MuteEffects = muteEffects.isOn;
    }
    public void SetEffectsVol()
    {
        Debug.Log("Effects Vol " + effectsVolume.value);
        settings.EffectsVolume = effectsVolume.value;
    }
    #endregion Audio

    #region Video

    public void SetQualityLevel(int index)
    {
        QualitySettings.SetQualityLevel(index);
        Debug.Log("Quality Level: " + QualitySettings.names[QualitySettings.GetQualityLevel()]);
        settings.QualityPreset = index;
        vsyncToggle.isOn = GetVsync();
        settings.Vsync = vsyncToggle.isOn;
        settings.TexQuality = GetTextureQuality();
        currentTexButton.SetText(settings.TexQuality.ToString());
        settings.AAValue = GetAALevel();
        currentAAButton.SetText(settings.AAValue.ToString() + 'x');
        settings.AnisoFiltering = GetAnisoFiltering();
        anisToggle.isOn = settings.AnisoFiltering;
        settings.TripleBuffering = GetTripleBuffering();
        tripBuffToggle.isOn = settings.TripleBuffering;
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
        settings.AAValue = aaLevel;
    }
    public int GetAALevel()
    {
        return QualitySettings.antiAliasing;
    }
    public void SetTripleBuffering()
    {
        QualitySettings.maxQueuedFrames = tripBuffToggle.isOn ? 3 : 0;
        Debug.Log("Triple buffering: " + QualitySettings.maxQueuedFrames);
        settings.TripleBuffering = tripBuffToggle.isOn;
    }
    public bool GetTripleBuffering()
    {
        return !(QualitySettings.maxQueuedFrames == 0);
    }
    public void SetAnistropicFiltering()
    {
        QualitySettings.anisotropicFiltering = anisToggle.isOn ? AnisotropicFiltering.ForceEnable : AnisotropicFiltering.Disable;
        Debug.Log("Anistropic Filtering: " + QualitySettings.anisotropicFiltering);
        settings.AnisoFiltering = anisToggle.isOn;
    }
    public bool GetAnisoFiltering()
    {
        return QualitySettings.anisotropicFiltering == AnisotropicFiltering.ForceEnable || QualitySettings.anisotropicFiltering == AnisotropicFiltering.Enable;
    }
    public void SetResolution(int width, int height, int refreshRate)
    {
        Screen.SetResolution(width, height, Screen.fullScreen, refreshRate);
        Debug.Log("Resolution: " + Screen.currentResolution.width + "x" + Screen.currentResolution.height);
        settings.ScreenWidth = width;
        settings.ScreenHeight = height;
        settings.RefreshRate = refreshRate;
    }
    public void SetVSync()
    {
        QualitySettings.vSyncCount = vsyncToggle.isOn ? 1 : 0;
        Debug.Log("VSync: " + QualitySettings.vSyncCount);
        settings.Vsync = vsyncToggle.isOn;
    }
    private bool GetVsync()
    {
        return QualitySettings.vSyncCount == 1 || QualitySettings.vSyncCount == 2;
    }
    public void SetFullScreen()
    {
        Screen.fullScreen = fullScreenToggle.isOn;
        Debug.Log("Full screen: Button: "+fullScreenToggle.isOn+ " Screen: " + Screen.fullScreen);
        settings.FullScreen = fullScreenToggle.isOn;
    }
    public void SetTextureQuality(TextureQuality texQuality)
    {
        QualitySettings.masterTextureLimit = (int)texQuality;
        Debug.Log("Texture Quality: " + QualitySettings.masterTextureLimit);
        settings.TexQuality = texQuality;
    }
    public TextureQuality GetTextureQuality()
    {
        return (TextureQuality)QualitySettings.masterTextureLimit;
    }
    #endregion Video


}

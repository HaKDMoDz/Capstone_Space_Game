/*
  GameSettings.cs
  Mission: Invasion
  Created by Rohun Banerji on March 23, 2015.
  Copyright (c) 2015 Rohun Banerji. All rights reserved.
*/
#region Usings
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.IO;
#endregion Usings
public class GameSettings
{
    //Audio settings
    public bool MuteMaster { get; set; }
    public float MasterVolume { get; set; }
    public bool MuteMusic { get; set; }
    public float MusicVolume { get; set; }
    public bool MuteEffects { get; set; }
    public float EffectsVolume { get; set; }

    //Video settings
    public int ScreenWidth { get; set; }
    public int ScreenHeight { get; set; }
    public int RefreshRate { get; set; }
    public bool FullScreen { get; set; }
    public int QualityPreset { get; set; }
    public bool Vsync { get; set; }
    public TextureQuality TexQuality { get; set; }
    public int AAValue { get; set; }
    public bool AnisoFiltering { get; set; }
    public bool TripleBuffering { get; set; }
    //References
#if FULL_DEBUG
    private XmlSerializer serializer;
#else
    private BinaryFormatter serializer;
#endif
    private FileStream fileStream;
    private string path;
    public GameSettings()
    {
#if FULL_DEBUG
        serializer = new XmlSerializer(typeof(GameSettings));
#else
        serializer = new BinaryFormatter();
#endif
        CreateSettingsDirectory();
        path = Application.persistentDataPath + '/' + SaveFilesConfig.Directory_Settings + '/' + SaveFilesConfig.FileName_settings + '.' + SaveFilesConfig.FileExtension_Settings;

        MuteMaster = false;
        MasterVolume = 1.0f;
        MuteMusic = false;
        MusicVolume = 1.0f;
        MuteEffects = false;
        EffectsVolume = 1.0f;

        ScreenWidth = Screen.width;
        ScreenHeight = Screen.height;
        RefreshRate = 60;
        FullScreen = true;
        QualityPreset = QualitySettings.names.Length - 1;
        Vsync = true;
        TexQuality = TextureQuality.Full;
        AAValue = 8;
        AnisoFiltering = true;
        TripleBuffering = true;
    }
    private void CopyValues(GameSettings settings)
    {
        MuteMaster = settings.MuteMaster;
        MasterVolume = settings.MasterVolume;
        MuteMusic = settings.MuteMusic;
        MusicVolume = settings.MusicVolume;
        MuteEffects = settings.MuteEffects;
        EffectsVolume = settings.EffectsVolume;

        ScreenWidth = settings.ScreenWidth;
        ScreenHeight = settings.ScreenHeight;
        RefreshRate = settings.RefreshRate;
        FullScreen = settings.FullScreen;
        QualityPreset = settings.QualityPreset;
        Vsync = settings.Vsync;
        TexQuality = settings.TexQuality;
        AAValue = settings.AAValue;
        AnisoFiltering = settings.AnisoFiltering;
        TripleBuffering = settings.TripleBuffering;
    }
    public void SaveSettings()
    {
#if FULL_DEBUG
        Debug.Log("Saving settings to " + path);
#endif
        fileStream = File.Create(path);
        serializer.Serialize(fileStream, this);
        fileStream.Close();
    }
    public bool LoadSettings()
    {
        if (File.Exists(path))
        {
            fileStream = File.Open(path, FileMode.Open);
            GameSettings settings = serializer.Deserialize(fileStream) as GameSettings;
            fileStream.Close();
            CopyValues(settings);
            return true;
        }
        else
        {
            return false;
        }
    }
    private void CreateSettingsDirectory()
    {
        if(!Directory.Exists(Application.persistentDataPath + '/' + SaveFilesConfig.Directory_Settings))
        {
            Directory.CreateDirectory(Application.persistentDataPath + '/' + SaveFilesConfig.Directory_Settings);
        }
    }
}

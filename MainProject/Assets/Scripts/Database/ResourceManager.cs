/*
  ResourceManager.cs
  Mission: Invasion
  Created by Rohun Banerji on Feb 25/2015
  Copyright (c) 2015 Rohun Banerji. All rights reserved.
*/

#region Usings
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
#endregion Usings

#region AdditionalStructs
public enum SpriteName
{
    Laser,
    AllLasers,
    Missile,
    AllMissiles,
    Railgun,
    AllRailguns,
    Shield,
    AllShields
}
[Serializable]
public struct SpriteInfo
{
    public SpriteName spriteName;
    public Sprite sprite;

}
public enum Sound
{
    //tracks
    TestTrack,
    GalaxyMapTheme,
    SciFiTheme,
    //effects
    Laser,
    Nuke1, Nuke2
}
[Serializable]
public struct SoundInfo
{
    public Sound sound;
    public AudioClip audioClip;
    public float defaultVolume;
    public int priority;
}

#endregion AdditionalStructs

public class ResourceManager : ScriptableObject
{
    //EditorExposed
    //Images
    [SerializeField]
    private List<SpriteInfo> spriteInfoList;
    //Sounds
    [SerializeField]
    private List<SoundInfo> soundInfoList;

    //Database References
    static private Dictionary<SpriteName, SpriteInfo> sprite_info_table;
    static private Dictionary<Sound, SoundInfo> sound_info_table;

    public static Sprite GetSprite(SpriteName spriteName)
    {
#if FULL_DEBUG
        SpriteInfo spriteInfo;
        if (!sprite_info_table.TryGetValue(spriteName, out spriteInfo))
        {
            Debug.LogError("No image found for " + spriteName);
            return null;
        }
        else
        {
            return spriteInfo.sprite;
        }
#else
        return image_info_table[imageName].image;
#endif
    }
    public static SoundInfo GetSoundInfo(Sound sound)
    {
#if FULL_DEBUG
        SoundInfo soundInfo;
        if(!sound_info_table.TryGetValue(sound, out soundInfo))
        {
            Debug.LogError("No info found for " + sound);
        }
        return soundInfo;
#else
        return sound_info_table[sound];
#endif
    }

    private void OnEnable()
    {
#if FULL_DEBUG
        //Images
        if (spriteInfoList == null || spriteInfoList.Count == 0)
        {
            Debug.LogError("No sprites found");
            return;
        }
        else
        {
            sprite_info_table = spriteInfoList.ToDictionary(spriteInfo => spriteInfo.spriteName, spriteInfo => spriteInfo);
        }
        //Sounds
        if (soundInfoList == null || soundInfoList.Count == 0)
        {
           Debug.LogError("No sound info found");
           return;
        }
        else
        {
            sound_info_table = soundInfoList.ToDictionary(s => s.sound, s => s);
        }
#else
        sprite_info_table = spriteInfoList.ToDictionary(spriteInfo => spriteInfo.spriteName, spriteInfo => spriteInfo);
        sound_info_table = soundInfoList.ToDictionary(s => s.sound, s => s);
#endif

    }
}

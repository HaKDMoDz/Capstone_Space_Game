#region Usings
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
#endregion Usings

#region AdditionalStructs
public enum ImageName
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
public struct ImageInfo
{
    public ImageName imageName;
    public Texture2D image;

}
public enum Sound
{
    //tracks
    TestTrack,
    //effects
    Laser
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
    private List<ImageInfo> imageInfoList;
    //Sounds
    [SerializeField]
    private List<SoundInfo> soundInfoList;

    //Database References
    static private Dictionary<ImageName, ImageInfo> image_info_table;
    static private Dictionary<Sound, SoundInfo> sound_info_table;

    public static Texture2D GetImage(ImageName imageName)
    {
#if FULL_DEBUG
        ImageInfo imageInfo;
        if (!image_info_table.TryGetValue(imageName, out imageInfo))
        {
            Debug.LogError("No image found for " + imageName);
            return null;
        }
        else
        {
            return imageInfo.image;
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
        if (imageInfoList == null || imageInfoList.Count == 0)
        {
            Debug.LogError("No images found");
            return;
        }
        else
        {
            image_info_table = imageInfoList.ToDictionary(imageInfo => imageInfo.imageName, imageInfo => imageInfo);
        }
        //Sounds
        if (soundInfoList == null || soundInfoList.Count == 0)
        {
           // Debug.LogError("No sound info found");
        }
        else
        {
            sound_info_table = soundInfoList.ToDictionary(s => s.sound, s => s);
        }
#else
        image_info_table = imageInfoList.ToDictionary(imageInfo => imageInfo.imageName, imageInfo => imageInfo);
        sound_info_table = soundInfoList.ToDictionary(s => s.sound, s => s);

#endif

    }
}

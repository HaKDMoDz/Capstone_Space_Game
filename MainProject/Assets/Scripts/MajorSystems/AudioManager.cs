#region Usings
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
#endregion Usings

#region AdditionStructs
public enum Sound
{
    //tracks
    TestTrack,
    //effects
    LaserBeam
}
[Serializable]
public struct SoundInfo
{
    public Sound sound;
    public AudioClip audioClip;
    public float defaultVolume;
    public int priority;
}

#endregion AdditionStructs

public class AudioManager : Singleton<AudioManager>
{

    #region Fields
    //EditorExposed
    [SerializeField]
    private int numSources = 32;
    [SerializeField]
    private List<SoundInfo> soundInfoList;

    //Internal
    private Queue<AudioSource> availableSources = new Queue<AudioSource>();
    private Dictionary<Sound, SoundInfo> sound_info_table;
    private AudioSource mainTrackSource;
    private Transform audioSrcParent;

    #endregion Fields

    #region Methods

    #region PublicMethods
    /// <summary>
    /// Play a sound effect at the camera's position
    /// </summary>
    /// <param name="sound"></param>
    public void PlayEffect(Sound sound)
    {
        PlaySound(sound, Vector3.zero);
    }
    /// <summary>
    /// Play a sound effect at the specified world position
    /// </summary>
    /// <param name="sound"></param>
    /// <param name="position"></param>
    public void PlayEffect(Sound sound, Vector3 position)
    {
        PlaySound(sound, position);
    }
    /// <summary>
    /// Play a sound effect and attach it to a specified transform
    /// </summary>
    /// <param name="sound"></param>
    /// <param name="parent"></param>
    public void PlayEffectAndAttachTo(Sound sound, Transform parent)
    {
        SoundInfo soundInfo;
        GetSoundInfo(sound, out soundInfo);
        StartCoroutine(PlayEffectAndAttach(soundInfo, parent));
    }
    /// <summary>
    /// Set the main track. Will replace the previously playing main track, if there is one.
    /// </summary>
    /// <param name="sound"></param>
    public void SetMainTrack(Sound sound)
    {
        SoundInfo soundInfo;
        GetSoundInfo(sound, out soundInfo);
        SetMainTrack(soundInfo);
    }
    #endregion PublicMethods

    #region PrivateMethods


    private void PlaySound(Sound sound, Vector3 position)
    {
        SoundInfo soundInfo;
        GetSoundInfo(sound, out soundInfo);
        if(availableSources.Count == 0)
        {
            Debug.LogError("No available sources found");
            return;
        }
        else
        {
            StartCoroutine(PlayEffect(soundInfo, position));
        }
    
    }
    private void SetMainTrack(SoundInfo soundInfo)
    {
        if(mainTrackSource.isPlaying)
        {
            mainTrackSource.Stop();
        }
        mainTrackSource.clip = soundInfo.audioClip;
        mainTrackSource.Play();
    }
    private IEnumerator PlayEffect(SoundInfo soundInfo, Vector3 position)
    {
        AudioSource source = availableSources.Dequeue();
        if (position != Vector3.zero)
        {
            Transform sourceTrans = source.gameObject.transform;
            sourceTrans.position = position;
            source.clip = soundInfo.audioClip;
            source.Play();
            yield return new WaitForSeconds(soundInfo.audioClip.length);
            sourceTrans.position = Vector3.zero;
        }
        else
        {
            source.clip = soundInfo.audioClip;
            source.Play();
            yield return new WaitForSeconds(soundInfo.audioClip.length);
        }

        source.Stop();
        availableSources.Enqueue(source);
    }

    private IEnumerator PlayEffectAndAttach(SoundInfo soundInfo, Transform parent)
    {
        AudioSource source = availableSources.Dequeue();
        Transform sourceTrans = source.gameObject.transform;
        sourceTrans.SetParent(parent, false);
        source.clip = soundInfo.audioClip;
        source.Play();

        yield return new WaitForSeconds(soundInfo.audioClip.length);
        
        source.Stop();
        sourceTrans.SetParent(audioSrcParent, false);
        sourceTrans.position = Vector3.zero;
        availableSources.Enqueue(source);
    }

    private void GetSoundInfo(Sound sound, out SoundInfo soundInfo)
    {
        #if FULL_DEBUG || LOW_DEBUG
        if (!sound_info_table.TryGetValue(sound, out soundInfo))
        {
            Debug.LogError("No info found for " + sound);
            return;
        }
        #else
        soundInfo = sound_info_table[sound];
        #endif
    }

    #endregion PrivateMethods


    #region UnityCallbacks
    
    private void Awake()
    {
        #if FULL_DEBUG
        if(soundInfoList==null || soundInfoList.Count==0)
        {
            Debug.LogError("No sound info found");
        }
        #endif
        sound_info_table = soundInfoList.ToDictionary(s => s.sound, s => s);

        GameObject audioSrcParentObj = new GameObject("AudioSources");
        audioSrcParent = audioSrcParentObj.transform;
        audioSrcParent.SetParent(transform, false);
        
        for (int i = 0; i < numSources; i++)
        {
            GameObject sourceObject = new GameObject("AudioSource " + i);
            sourceObject.transform.SetParent(audioSrcParent, false);
            AudioSource source = sourceObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            availableSources.Enqueue(source);
        }
        mainTrackSource = availableSources.Dequeue();
        mainTrackSource.loop = true;
    }
    #endregion UnityCallbacks

    #endregion Methods
}

/*
  AudioManager.cs
  Mission: Invasion
  Created by Rohun Banerji on Feb 2/2015
  Copyright (c) 2015 Rohun Banerji. All rights reserved.
*/

#region Usings
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using Random = UnityEngine.Random;
#endregion Usings
public class AudioManager : Singleton<AudioManager>
{

    #region Fields
    //EditorExposed
    [SerializeField]
    private int numSources = 32;
    [SerializeField]
    private Vector2 pitchRange = new Vector2(0.7f, 1.3f);

    //Internal
    private Queue<AudioSource> availableSources = new Queue<AudioSource>();
    private List<AudioSource> playingSources = new List<AudioSource>();
    private AudioSource mainTrackSource;
    private Transform audioSrcParent;
    private GameSettings settings;

    #endregion Fields

    #region Methods

    #region PublicMethods
    /// <summary>
    /// Play a sound effect at the camera's position
    /// </summary>
    /// <param name="sound"></param>
    public void PlayEffect(Sound sound, bool varyPitch = false)
    {
        PlaySound(sound, Vector3.zero, varyPitch);
    }
    /// <summary>
    /// Play a sound effect at the specified world position
    /// </summary>
    /// <param name="sound"></param>
    /// <param name="position"></param>
    public void PlayEffect(Sound sound, Vector3 position, bool varyPitch = false)
    {
        PlaySound(sound, position, varyPitch);
    }
    /// <summary>
    /// Play a sound effect and attach it to a specified transform
    /// </summary>
    /// <param name="sound"></param>
    /// <param name="parent"></param>
    public void PlayEffectAndAttachTo(Sound sound, Transform parent, bool varyPitch = false)
    {
        StartCoroutine(PlayEffectAndAttach(GetSoundInfo(sound), parent, varyPitch));
    }
    /// <summary>
    /// Set the main track. Will replace the previously playing main track, if there is one.
    /// </summary>
    /// <param name="sound"></param>
    public void SetMainTrack(Sound sound)
    {
        SetMainTrack(GetSoundInfo(sound));
    }
    public void UpdateSettings(GameSettings settings)
    {
        this.settings = settings;
        mainTrackSource.volume = ResourceManager.GetDefaultVolume(mainTrackSource.clip) * settings.MasterVolume * settings.MusicVolume;
        if (settings.MuteMaster || settings.MuteMusic) mainTrackSource.volume = 0.0f;
        if(settings.MuteMusic || settings.MuteEffects)
        {
            foreach (AudioSource source in playingSources)
            {
                source.volume = 0.0f;
            }
        }
    }
    #endregion PublicMethods

    #region PrivateMethods

    private void PlaySound(Sound sound, Vector3 position, bool varyPitch)
    {
        if(availableSources.Count == 0)
        {
            Debug.LogError("No available sources found");
            return;
        }
        else
        {
            StartCoroutine(PlayEffect(GetSoundInfo(sound), position, varyPitch));
        }
    
    }
    private void SetMainTrack(SoundInfo soundInfo)
    {
        if(mainTrackSource.isPlaying)
        {
            mainTrackSource.Stop();
        }
        mainTrackSource.clip = soundInfo.audioClip;
        mainTrackSource.volume = (settings.MuteMaster || settings.MuteMusic)? 0.0f : soundInfo.defaultVolume * settings.MasterVolume * settings.MusicVolume;
        mainTrackSource.Play();
    }
    private IEnumerator PlayEffect(SoundInfo soundInfo, Vector3 position, bool varyPitch)
    {
        AudioSource source = availableSources.Dequeue();
        playingSources.Add(source);
        if (position != Vector3.zero)
        {
            Transform sourceTrans = source.gameObject.transform;
            sourceTrans.position = position;
            source.clip = soundInfo.audioClip;
            source.volume = (settings.MuteMaster || settings.MuteEffects) ? 0.0f : soundInfo.defaultVolume * settings.MasterVolume * settings.EffectsVolume;
            source.pitch = varyPitch ? Random.Range(pitchRange.x, pitchRange.y) : 1.0f;
            source.Play();
            yield return new WaitForSeconds(soundInfo.audioClip.length);
            sourceTrans.position = Vector3.zero;
        }
        else
        {
            source.pitch = varyPitch ? Random.Range(pitchRange.x, pitchRange.y) : 1.0f;
            source.clip = soundInfo.audioClip;
            source.volume = (settings.MuteMaster || settings.MuteEffects) ? 0.0f: soundInfo.defaultVolume * settings.MasterVolume * settings.EffectsVolume;
            source.Play();
            yield return new WaitForSeconds(soundInfo.audioClip.length);
        }

        source.Stop();
        availableSources.Enqueue(source);
        playingSources.Remove(source);
    }

    private IEnumerator PlayEffectAndAttach(SoundInfo soundInfo, Transform parent, bool varyPitch)
    {
        AudioSource source = availableSources.Dequeue();
        playingSources.Add(source);
        Transform sourceTrans = source.gameObject.transform;
        sourceTrans.SetParent(parent, false);
        source.clip = soundInfo.audioClip;
        source.volume = (settings.MuteMaster || settings.MuteEffects) ? 0.0f : soundInfo.defaultVolume * settings.MasterVolume * settings.EffectsVolume;
        source.pitch = varyPitch ? Random.Range(pitchRange.x, pitchRange.y) : 1.0f;
        source.Play();

        yield return new WaitForSeconds(soundInfo.audioClip.length);
        
        source.Stop();
        sourceTrans.SetParent(audioSrcParent, false);
        sourceTrans.position = Vector3.zero;
        availableSources.Enqueue(source);
        playingSources.Remove(source);
    }

    private SoundInfo GetSoundInfo(Sound sound)
    {
        return ResourceManager.GetSoundInfo(sound);
    }

    #endregion PrivateMethods


    #region UnityCallbacks
    
    private void Awake()
    {
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
        settings = new GameSettings();
        settings.LoadSettings();
    }
    #endregion UnityCallbacks

    #endregion Methods
}

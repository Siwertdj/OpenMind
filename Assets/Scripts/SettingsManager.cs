// This program has been developed by students from the bachelor Computer Science at Utrecht University within the Software Project course.
// © Copyright Utrecht University (Department of Information and Computing Sciences)
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    // SettingsManager has a static instance, so that we can fetch its settings from anywhere.
    public static SettingsManager sm;
    
    // the audiomixer that contains all soundchannels
    public AudioMixer audioMixer;

    private void Awake()
    {
        // create static instance of settingsmanager and make it DDOL
        sm = this;
        DontDestroyOnLoad(this.gameObject);
    }


    #region Audio
    /// <summary>
    /// Called through SettingsMenuManager to set the volume of the Master-channel of the Audiomixer.
    /// </summary>
    /// <param name="volume"></param>
    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", volume);
    }

    /// <summary>
    /// Called through SettingsMenuManager to set the volume of the Music-channel of the Audiomixer.
    /// </summary>
    /// <param name="volume"></param>
    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", volume);
    }

    /// <summary>
    /// Called through SettingsMenuManager to set the volume of the SFX-channel of the Audiomixer.
    /// </summary>
    /// <param name="volume"></param>
    public void SetSfxVolume(float volume)
    {
        audioMixer.SetFloat("SfxVolume", volume);
        
    }
    
    /// <summary>
    /// this method should fade-out the previous track, then fade-in the new track
    /// </summary>
    private void SwitchMusic()
    {
        
    }
    #endregion
}

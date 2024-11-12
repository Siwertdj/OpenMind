using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

// Most code in this script is credited to Brackey's tutorial
// https://www.youtube.com/watch?v=YOaYQrN1oYQ
public class SettingsManager : MonoBehaviour
{
    public static SettingsManager sm;
    private       AudioManager    am;
    
    // the audiomixer that contains all soundchannels
    public AudioMixer audioMixer;

    private void Awake()
    {
        // create static instance of settingsmanager and make it DDOL
        sm = this;
        am = GetComponent<AudioManager>();
        DontDestroyOnLoad(this.gameObject);
    }


    #region Audio
    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", volume);
    }
    
    public void SetSfxVolume(float volume)
    {
        audioMixer.SetFloat("SfxVolume", volume);
        
    }
    
    // this method should fade-out the previous track, then fade-in the new track
    private void SwitchMusic()
    {
        
    }
    #endregion
}

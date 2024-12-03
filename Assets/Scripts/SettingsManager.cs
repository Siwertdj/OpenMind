// This program has been developed by students from the bachelor Computer Science at Utrecht University within the Software Project course.
// © Copyright Utrecht University (Department of Information and Computing Sciences)
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    // SettingsManager has a static instance, so that we can fetch its settings from anywhere.
    public static SettingsManager sm;
    
    // the audiomixer that contains all soundchannels
    public AudioMixer audioMixer;
    
    private AudioSource musicSource;
    
    // Text size to be used for the text components
    public TextSize textSize;
    
    public enum TextSize
    {
        Small,
        Medium,
        Large
    }
    
    [FormerlySerializedAs("musicFadeInTime")] [SerializeField] float defaultMusicFadeInTime = 0.5f;
    
    private void Awake()
    {
        // create static instance of settingsmanager and make it DDOL
        sm = this;
        DontDestroyOnLoad(this.gameObject);
        
        // Set the default textSize to medium.
        textSize = TextSize.Medium;
        
        // Set reference to music-audiosource by component
        musicSource = GetComponents<AudioSource>()[0];
    }
    
    #region TextSize

    /// <summary>
    /// Get the fontSize of the dialogue text.
    /// </summary>
    public int GetFontSize()
    {
        switch (textSize)
        {
            case TextSize.Small:
                return 35;
            case TextSize.Medium:
                return 45;
            default:
                return 55;
        }
    }

    #endregion

    #region Audio
    /// <summary>
    /// Called through SettingsMenuManager to set the volume of the Master-channel of the Audiomixer.
    /// </summary>
    /// <param name="volume"></param>
    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("masterVolume", volume);
    }

    /// <summary>
    /// Called through SettingsMenuManager to set the volume of the Music-channel of the Audiomixer.
    /// </summary>
    /// <param name="volume"></param>
    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("musicVolume", volume);
    }

    /// <summary>
    /// Called through SettingsMenuManager to set the volume of the SFX-channel of the Audiomixer.
    /// </summary>
    /// <param name="volume"></param>
    public void SetSfxVolume(float volume)
    {
        audioMixer.SetFloat("sfxVolume", volume);
        
    }
    
    /// <summary>
    /// this method should fade-out the previous track, then fade-in the new track
    /// </summary>
    public void SwitchMusic(AudioClip newClip, float? fadeTime)
    {
        if (newClip != null)
        {
            // If the passed fadeTime is null, we use the default music fade-in time
            float _fadeTime = fadeTime ?? defaultMusicFadeInTime;

            // If the newclip is different than the current clip, we fade the new one in.
            if (newClip != musicSource.clip)
                StartCoroutine(FadeOutMusic(newClip, _fadeTime));
        }
    }

    /// <summary>
    /// Fades out currently playing Audioclip, then fades in Audioclip passed as argument.
    /// Speed of fade depends on fadetime.
    /// </summary>
    /// <param name="newClip"></param>
    /// <param name="fadeTime"></param>
    /// <returns></returns>
    private IEnumerator FadeOutMusic(AudioClip newClip, float fadeTime)
    {
        float startVolume = musicSource.volume;
        while (musicSource.volume > 0)
        {
            musicSource.volume -= startVolume * Time.deltaTime / fadeTime;
            yield return null;
        }
        musicSource.Stop();
        musicSource.clip = newClip;
        musicSource.Play();
        while (musicSource.volume < 1)
        {
            musicSource.volume += startVolume * Time.deltaTime / fadeTime;
            yield return null;
        }
    }
    #endregion
}
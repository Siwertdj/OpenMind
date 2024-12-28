// This program has been developed by students from the bachelor Computer Science at Utrecht University within the Software Project course.
// © Copyright Utrecht University (Department of Information and Computing Sciences)
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    // SettingsManager has a static instance, so that we can fetch its settings from anywhere.
    public static SettingsManager sm;

    [Header("Component References")]
    public AudioMixer audioMixer; // The audiomixer that contains all soundchannels
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Settings (?)")]
    [SerializeField] float defaultMusicFadeInTime = 0.5f;
    [SerializeField] AudioClip defaultButtonClickSound;

    public float TalkingDelay {  get; private set; }

    // TODO: Integrate this with text-size
    [NonSerialized] public int maxLineLength = 30;
    
    // Text size to be used for the text components
    [NonSerialized] public TextSize textSize;
    
    public enum TextSize
    {
        Small,
        Medium,
        Large
    }

    // Multipliers for different text sizes
    public const float M_SMALL_TEXT = 0.8f;
    public const float M_LARGE_TEXT = 1.4f;

    public UnityEvent OnTextSizeChanged;
    

    #region Settings Variables
    [NonSerialized] public float musicVolume = 0;
    [NonSerialized] public float sfxVolume = 0;
    [NonSerialized] public float talkingSpeed = 1;
    #endregion
    
    private void Awake()
    {
        // create static instance of settingsmanager and make it DDOL
        sm = this;
        
        // Set the default textSize to medium.
        textSize = TextSize.Medium;
    }
    
    private void Start()
    {
        if (audioMixer != null)
            ApplySavedSettings();
    }

    private void ApplySavedSettings()
    {
        // Get the saved values
        musicVolume = PlayerPrefs.GetFloat(nameof(musicVolume), 0);
        sfxVolume = PlayerPrefs.GetFloat(nameof(sfxVolume), 0);
        talkingSpeed = PlayerPrefs.GetFloat(nameof(talkingSpeed), 1);
        textSize = (TextSize)PlayerPrefs.GetInt(nameof(textSize), 1);

        // Apply the saved values
        SetMusicVolume(musicVolume);
        SetSfxVolume(sfxVolume);
        SetTalkingSpeed(talkingSpeed);
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat(nameof(musicVolume), musicVolume);
        PlayerPrefs.SetFloat(nameof(sfxVolume), sfxVolume);
        PlayerPrefs.SetFloat(nameof(talkingSpeed), talkingSpeed);
        PlayerPrefs.SetInt(nameof(textSize), (int)textSize);
    }

    public void OnClick(Component sender, params object[] data)
    {
        if (data[0] is AudioClip audioClip)
            sfxSource.clip = audioClip;
        else
            sfxSource.clip = defaultButtonClickSound;

        sfxSource.Play();
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
                return 55;
            case TextSize.Medium:
                return 70;
            default:
                return 85;
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
        audioMixer.SetFloat(nameof(musicVolume), volume);
        musicVolume = volume;
    }

    /// <summary>
    /// Called through SettingsMenuManager to set the volume of the SFX-channel of the Audiomixer.
    /// </summary>
    /// <param name="volume"></param>
    public void SetSfxVolume(float volume)
    {
        if (volume <= -40)
            volume = -80;
        audioMixer.SetFloat(nameof(sfxVolume), volume);
        sfxVolume = volume;
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

    #region Accessibility
    public void SetTalkingSpeed(float multiplier) 
    { 
        TalkingDelay = 0.05f * multiplier;
        talkingSpeed = multiplier;
    }
    #endregion
}
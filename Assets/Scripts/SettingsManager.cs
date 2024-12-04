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

    public float TalkingDelay {  get; private set; }
    
    private AudioSource musicSource;

    private TextToSpeech tts;

    #region Settings Variables
    [NonSerialized] public float musicVolume = 0;
    [NonSerialized] public float sfxVolume = 0;
    [NonSerialized] public float talkingSpeed = 1;
    [NonSerialized] public bool ttsEnabled = false;
    #endregion

    [FormerlySerializedAs("musicFadeInTime")] [SerializeField] float defaultMusicFadeInTime = 0.5f;
    
    private void Awake()
    {
        // create static instance of settingsmanager and make it DDOL
        sm = this;
        DontDestroyOnLoad(this.gameObject);
        
        // Set reference to music-audiosource by component
        musicSource = GetComponents<AudioSource>()[0];

        tts = GetComponent<TextToSpeech>();
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
        ttsEnabled = PlayerPrefs.GetInt(nameof(ttsEnabled), 0) == 1;

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
        PlayerPrefs.SetInt(nameof(ttsEnabled), ttsEnabled ? 1 : 0);
    }

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

    public void AttemptSpeakTTS(string text)
    {
        if (ttsEnabled)
        {
            Debug.Log("tts");
            tts.StartSpeech(text);
        }
    }
    #endregion
}
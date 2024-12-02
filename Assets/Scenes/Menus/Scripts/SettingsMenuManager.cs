// This program has been developed by students from the bachelor Computer Science at Utrecht University within the Software Project course.
// © Copyright Utrecht University (Department of Information and Computing Sciences)
using System;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine.Audio;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SettingsMenuManager : MonoBehaviour
{
    [Header("Audio References")]
    [SerializeField] private GameObject audioSliderGroup;

    private Slider musicVolumeSlider;
    private Slider sfxVolumeSlider;

    [Header("Accessibility References")]
    [SerializeField] private GameSlider talkingSpeedSlider;
    [SerializeField] private Toggle textToSpeechToggle;

    private void Start()
    {
        // Get the sliders
        Slider[] sliders = audioSliderGroup.GetComponentsInChildren<Slider>();
        musicVolumeSlider = sliders[0];
        sfxVolumeSlider = sliders[1];

        // Set the values on the UI elements
        musicVolumeSlider.SetValueWithoutNotify(SettingsManager.sm.musicVolume);
        sfxVolumeSlider.SetValueWithoutNotify(SettingsManager.sm.sfxVolume);
        talkingSpeedSlider.slider.SetValueWithoutNotify(SettingsManager.sm.talkingSpeed);
        textToSpeechToggle.SetIsOnWithoutNotify(SettingsManager.sm.ttsEnabled);
        
    }

    /// <summary>
    /// Called when the scene is unloaded.
    /// Saves the values which were set in the settings screen.
    /// </summary>
    private void OnDestroy()
    {
        SettingsManager.sm.SaveSettings();
    }

    /// <summary>
    /// Called to exit the settingsmenu. It sets any other menu overlays to 'active',
    /// such as the notebook- and menu-buttons in the game, and unloads its own scene.
    /// </summary>
    public void ExitSettings()
    {
        // If a scenecontroller exists, we exit the settings using the transition-graph.
        if (SceneController.sc != null)
        {
            // '_ =' throws away the await
            _ = SceneController.sc.TransitionScene(SceneController.SceneName.SettingsScene,
                SceneController.SceneName.GameMenuScene,
                SceneController.TransitionType.Unload);
        }
        // otherwise, we use the built-in SceneManager to unload.
        else
        {
            SceneManager.UnloadSceneAsync("SettingsScene");
        }
    }
    
    /// <summary>
    /// Called to set the volume of the Master-channel of the Audiomixer, using dynamic floats.
    /// </summary>
    /// <param name="volume"></param>
    public void SetMasterVolume(float volume)
    {
        SettingsManager.sm.SetMasterVolume(volume);
    }
    
    /// <summary>
    /// Called to set the volume of the Music-channel of the Audiomixer, using dynamic floats.
    /// </summary>
    /// <param name="volume"></param>
    public void SetMusicVolume(float volume)
    {
        SettingsManager.sm.SetMusicVolume(volume);
    }
    
    /// <summary>
    /// Called to set the volume of the Sfx-channel of the Audiomixer, using dynamic floats.
    /// </summary>
    /// <param name="volume"></param>
    public void SetSfxVolume(float volume)
    {
        SettingsManager.sm.SetSfxVolume(volume);        
    }

    public void SetTalkingSpeed(float multiplier)
    {
        SettingsManager.sm.SetTalkingSpeed(multiplier);
    }
}
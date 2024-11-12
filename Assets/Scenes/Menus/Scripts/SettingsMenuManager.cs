// This program has been developed by students from the bachelor Computer Science at Utrecht University within the Software Project course.
// © Copyright Utrecht University (Department of Information and Computing Sciences)
using System;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine.Audio;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class SettingsMenuManager : MonoBehaviour
{
    [CanBeNull] private GameObject otherMenuOverlay;
    
    // SLIDERS
    [SerializeField] private GameObject sliderGroup;
    
    private void Start()
    {
        // When the menu is first opened, we run this.
        otherMenuOverlay = GameObject.FindGameObjectWithTag("MenuOverlay");
        // If its not null, we set it to inactive
        otherMenuOverlay?.SetActive(false);
    }

    /// <summary>
    /// This method is intended to correct the sliders when opening the menu, so the sliders
    /// correspond with the values of the audiomixer.
    /// TODO: Currently doesnt work.
    /// </summary>
    public void CorrectSliderValues()
    {
        // Get the sliders
        Slider[] sliders = sliderGroup.GetComponentsInChildren<Slider>();
        Slider masterVolumeSlider = sliders[0];
        Slider musicVolumeSlider = sliders[1];
        Slider sfxVolumeSlider = sliders[2];
        
        // Fetch the values from the audiomixer
        SettingsManager.sm.audioMixer.GetFloat("MasterVolume", out float masterVolume);
        SettingsManager.sm.audioMixer.GetFloat("MusicVolume", out float musicVolume);
        SettingsManager.sm.audioMixer.GetFloat("SfxVolume", out float sfxVolume);
        // Set the values
        masterVolumeSlider.SetValueWithoutNotify(masterVolume);
        musicVolumeSlider.SetValueWithoutNotify(musicVolume);
        sfxVolumeSlider.SetValueWithoutNotify(sfxVolume);
    }
    
    /// <summary>
    /// Called to exit the settingsmenu. It sets any other menu overlays to 'active',
    /// such as the notebook- and menu-buttons in the game, and unloads its own scene.
    /// </summary>
    public void ExitSettings()
    {
        // If the othermenuoverlay is not null, we set it back to 'active'
        otherMenuOverlay?.SetActive(true);
        SceneManager.UnloadSceneAsync("SettingsScene");
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

}

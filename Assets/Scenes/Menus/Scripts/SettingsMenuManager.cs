// This program has been developed by students from the bachelor Computer Science at Utrecht University within the Software Project course.
// © Copyright Utrecht University (Department of Information and Computing Sciences)
using System;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine.Audio;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class SettingsMenuManager : MonoBehaviour
{
    // SLIDERS
    [SerializeField] private GameObject sliderGroup;
    
    // Buttons from TextMenuOptions
    [SerializeField] private GameObject buttonGroup;
    
    // The active size for the text settings (default: medium)
    private GameButton activeButton;
    
    // Chosen text size
    private SettingsManager.TextSize size;
    
    void Awake()
    {
        // Set the active button.
        size = SettingsManager.sm.textSize;
        activeButton = GetButton(SettingsManager.sm.textSize);
        SetActiveButton(activeButton);
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
        SettingsManager.sm.textSize = size;
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
    
    #region TextSettings
    
    // TODO: add a dialogue box showing the difference in text sizes.
    
    /// <summary>
    /// Give the chosen text-size a color indicating that it is chosen.
    /// </summary>
    /// <param name="index"></param>
    public void SetActiveSize(GameButton button)
    {
        GameButton[] children = buttonGroup.GetComponentsInChildren<GameButton>();
        // Set all buttons (excluding return button) to the color white.
        for (int i = 0; i < children.Length - 1; i++)
        {
            children[i].GetComponent<Image>().color = Color.white;
        }
        // Set the chosen size to a green color.
        activeButton = button;
        size = GetTextSize(button);
        button.GetComponent<Image>().color = Color.green;
    }
    
    /// <summary>
    /// Set the button corresponding to the TextSize to active.
    /// </summary>
    /// <param name="button"></param>
    private void SetActiveButton(GameButton button)
    {
        button.GetComponent<Image>().color = Color.green;
    }
    
    
    /// <summary>
    /// Get the GameButton corresponding to the TextSize
    /// </summary>
    /// <param name="size"></param>
    /// <returns></returns>
    private GameButton GetButton(SettingsManager.TextSize size)
    {
        GameButton[] children = buttonGroup.GetComponentsInChildren<GameButton>();
        if (size == SettingsManager.TextSize.Small)
        {
            // Small button
            return children[0];
        }
        else if (size == SettingsManager.TextSize.Medium)
        {
            // Medium button
            return children[1];
        }
        else
        {
            // Large button
            return children[2];
        }
    }
    
    /// <summary>
    /// Get the TextSize corresponding to the GameButton
    /// </summary>
    /// <param name="button"></param>
    /// <returns></returns>
    private SettingsManager.TextSize GetTextSize(GameButton button)
    {
        if (button.name == "SmallButton")
        {
            // Small TextSize
            return SettingsManager.TextSize.Small;
        }
        else if (button.name == "MediumButton")
        {
            // Medium TextSize
            return SettingsManager.TextSize.Medium;
        }
        else
        {
            // Large TextSize
            return SettingsManager.TextSize.Large;
        }
    }

    #endregion
}
// This program has been developed by students from the bachelor Computer Science at Utrecht University within the Software Project course.
// © Copyright Utrecht University (Department of Information and Computing Sciences)
using System;
using JetBrains.Annotations;
using TMPro;
using UnityEditor;
using UnityEngine.Audio;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class SettingsMenuManager : MonoBehaviour
{
    [Header("Audio References")]
    [SerializeField] private GameObject audioSliderGroup;

    private GameSlider musicVolumeSlider;
    private GameSlider sfxVolumeSlider;
    
    // Buttons from TextMenuOptions
    [SerializeField] private GameObject buttonGroup;
    
    // The active size for the text settings (default: medium)
    private GameButton activeButton;
    
    // Chosen text size
    private SettingsManager.TextSize textSize;

    // Example tmp_text objects
    [SerializeField] private TMP_Text characterNameField;
    [SerializeField] private TMP_Text dialogueBox;
    
    // Events
    [SerializeField] GameEvent onTextSizeChanged; 
    
    void Awake()
    {
        // Set the active text size button
        textSize = SettingsManager.sm.textSize;
        activeButton = GetButton(SettingsManager.sm.textSize);
        SetActiveButton(activeButton);
        
        // Change the text size
        characterNameField.GetComponentInChildren<TMP_Text>().enableAutoSizing = true;
        dialogueBox.GetComponentInChildren<TMP_Text>().enableAutoSizing = true;
        ChangeTextSize();
    }

    [Header("Accessibility References")]
    [SerializeField] private GameSlider textSpeedSlider;
    [SerializeField] private Toggle textToSpeechToggle;

    private void Start()
    {
        // Get the sliders
        GameSlider[] sliders = audioSliderGroup.GetComponentsInChildren<GameSlider>();
        musicVolumeSlider = sliders[0];
        sfxVolumeSlider = sliders[1];

        // Set the values on the UI elements
        musicVolumeSlider.UpdateSlider(SettingsManager.sm.musicVolume);
        sfxVolumeSlider.UpdateSlider(SettingsManager.sm.sfxVolume);
        textSpeedSlider.slider.SetValueWithoutNotify(SettingsManager.sm.talkingSpeed);        
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
        if (SceneManager.GetSceneByName("StartScreenScene").isLoaded)
        {
            SceneManager.UnloadSceneAsync("SettingsScene");
        }
        else if (SceneManager.GetSceneByName("Loading").isLoaded)
        {
            // '_ =' throws away the await
            _ = SceneController.sc.TransitionScene(SceneController.SceneName.SettingsScene,
                SceneController.SceneName.Loading,
                SceneController.TransitionType.Unload,
                true);
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
    
    #region TextSettings
    
    // TODO: add a dialogue box showing the difference in text sizes.
    
    /// <summary>
    /// Give the chosen text-size a color indicating that it is chosen.
    /// </summary>
    /// <param name="index"></param>
    public void SetActiveSize(GameButton button)
    {
        GameButton[] children = buttonGroup.GetComponentsInChildren<GameButton>();
        
        // Set all buttons (excluding return button) to the color white and disable all arrows.
        for (int i = 0; i < children.Length; i++)
            children[i].GetComponent<Image>().color = Color.white;
        
        // Set the chosen size to a green color and enable the arrow.
        activeButton = button;
        SetActiveButton(button);
        textSize = GetTextSize(button);
        
        // Change the textSize from SettingsManager
        SettingsManager.sm.textSize = textSize;
        SettingsManager.sm.OnTextSizeChanged.Invoke();
        
        // Change the fontSize of the example
        ChangeTextSize();
        
        // Raise the onTextSizeChanged event to change the text size
        onTextSizeChanged.Raise(null, SettingsManager.sm.GetFontSize());
    }
    
    /// <summary>
    /// Set the button corresponding to the TextSize to active.
    /// </summary>
    /// <param name="button"></param>
    private void SetActiveButton(GameButton button)
    {
        // Set the chosen size to a green color
        button.GetComponent<Image>().color = Color.green;
    }
    
    /// <summary>
    /// Change the fontSize of the tmp_text components
    /// </summary>
    private void ChangeTextSize()
    {
        int fontSize = SettingsManager.sm.GetFontSize();
        // Change the fontSize of the confirmSelectionButton
        characterNameField.GetComponentInChildren<TMP_Text>().fontSizeMax = fontSize;
        
        // Change the fontSize of the headerText
        dialogueBox.GetComponentInChildren<TMP_Text>().fontSizeMax = fontSize;
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
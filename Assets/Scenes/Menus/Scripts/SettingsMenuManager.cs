// This program has been developed by students from the bachelor Computer Science at Utrecht University within the Software Project course.
// © Copyright Utrecht University (Department of Information and Computing Sciences)
using System;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine.Audio;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsMenuManager : MonoBehaviour
{
    [CanBeNull] private GameObject otherMenuOverlay;
    
    private void Start()
    {
        // When the menu is first opened, we run this.
        otherMenuOverlay = GameObject.FindGameObjectWithTag("MenuOverlay");
        // If its not null, we set it to inactive
        otherMenuOverlay?.SetActive(false);
    }
    
    public void ExitSettings()
    {
        // If the othermenuoverlay is not null, we set it back to 'active'
        otherMenuOverlay?.SetActive(true);
        SceneManager.UnloadSceneAsync("SettingsScene");
    }
    
    public void SetMusicVolume(float volume)
    {
        Debug.Log($"Set music-volume to {volume}");
        SettingsManager.sm.SetMusicVolume(volume);
    }
    
    public void SetSfxVolume(float volume)
    {
        SettingsManager.sm.SetSfxVolume(volume);
        
    }

}

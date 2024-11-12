using System;
using UnityEditor;
using UnityEngine.Audio;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsMenuManager : MonoBehaviour
{
    public void ExitSettings()
    {
        // TODO: Load it on-top of the in-game menu
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

using System;
using UnityEngine.Audio;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsMenuManager : MonoBehaviour
{
    public void ExitSettings()
    {
        SceneManager.UnloadSceneAsync("SettingsScene");
    }
    
    public void SetMusicVolume(float volume)
    {
        SettingsManager.sm.SetMusicVolume(volume);
    }
    
    public void SetSfxVolume(float volume)
    {
        SettingsManager.sm.SetSfxVolume(volume);
        
    }
}

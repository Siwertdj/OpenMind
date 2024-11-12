using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Variables
    private int musicVolume;
    private int sfxVolume;

    // Audioclips
    private AudioClip click;
    
    // References
    private AudioSource click_;
    
    private void Start()
    {
        musicVolume = 0;
        sfxVolume = 0;
    }

    public void PlayCLick()
    {
        click_.Play();
    }
}

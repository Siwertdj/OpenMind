using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AndroidTTS : TextToSpeech
{
    public override void Speak(string text)
    {
        Debug.Log("Android is speaking");
    }
}

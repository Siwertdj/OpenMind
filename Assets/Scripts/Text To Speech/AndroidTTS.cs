using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AndroidTTS : TextToSpeechParent
{
    TextToSpeech tts;

    public override void Speak(string text)
    {
        Debug.Log("Android is speaking");
        tts.Speak(text);
    }
}

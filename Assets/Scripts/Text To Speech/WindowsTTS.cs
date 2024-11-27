using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Speech.Synthesis;

public class WindowsTTS : TextToSpeechParent
{
    SpeechSynthesizer synthesizer;

    public WindowsTTS()
    {
        synthesizer = new SpeechSynthesizer();
    }

    public override void Speak(string text)
    {
        Debug.Log("Windows speaking");
        synthesizer.Speak(text);
    }
}

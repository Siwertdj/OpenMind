#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpeechLib;
#endif

public class WindowsTTS : TextToSpeech
{
    private SpVoice voice = new();

    public override void Speak(string text)
    {
        voice.Speak(text, SpeechVoiceSpeakFlags.SVSFlagsAsync | SpeechVoiceSpeakFlags.SVSFPurgeBeforeSpeak);
    }
}
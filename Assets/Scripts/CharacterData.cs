using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

[CreateAssetMenu(fileName = "newCharacter", menuName = "Character")]
public class CharacterData : ScriptableObject
{
    public string characterName;
    public int id;
    public Sprite avatar;
    [Range(0.5f, 2f)] public float voicePitch = 1;

    [SerializeField]
    public AnswerTuple[] answers;

    [SerializeField]
    public DialogueLine[][] greetings;
}

// KeyValuePair & DialogueLines must be individual objects in order to show up in the inspector
[Serializable]
public struct AnswerTuple
{
    [SerializeField]
    public Question question;
    [SerializeField]
    public DialogueLine[] answer;
    [SerializeField]
    public DialogueLine[] trait;
}

[Serializable]
public class DialogueLines
{
    private DialogueLine[] _lines;

    private 
}

//[Serializable]
//public struct DialogueLines
//{
//    public List<string> strings { get lines.Select(x => x.line); } }

//    public DialogueLines(DialogueLine[] lines)
//    {
//        this.lines = lines;
//    }

//    [SerializeField]
//    public DialogueLine[] lines;
//}

[Serializable]
public class DialogueLine
{
    private string name = "Line";

    public DialogueLine(string line, Emotion emotion)
    {
        this.line = line;
        this.emotion = emotion;
    }

    public DialogueLine(string line)
    {
        this.line = line;
    }

    [SerializeField]
    public string line;
    [SerializeField]
    public Emotion emotion;
}

public enum Emotion
{
    Neutral,
    Happy,
    Sad,
    Angry
}
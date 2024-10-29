using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

[CreateAssetMenu(fileName = "newCharacter", menuName = "Character")]
public class CharacterData : ScriptableObject
{

    public enum Moods
    {
        Neutral,
        Happy,
        Sad,
        Angry
    }

    //public Answers testing = new Answers();

    //string test1;

    

    public string characterName;
    public int id;
    public Sprite avatar;
    [Range(0.5f, 2f)] public float voicePitch = 1;

    [SerializeField]
    public KeyValuePair[] answers;

    [SerializeField]
    public DialogueLines[] greetings;

    //string test()
    //{
    //    return test1 = answers[0].answer[0].answer;
    //}
}

// KeyValuePair & DialogueLines must be individual objects in order to show up in the inspector
[Serializable]
public struct KeyValuePair
{
    [SerializeField]
    public Question question;
    //[SerializeField]
    //public List<Answers> answer2;
    [SerializeField]
    public List<string> answer;
    [SerializeField]
    public List<DialogueObject.Mood> mood;
    [SerializeField]
    public List<string> trait;
    
}

[Serializable]
public struct Answers
{
    [SerializeField]
    public string answer;
    [SerializeField]
    public CharacterData.Moods moods;
}

[Serializable]
public struct DialogueLines
{
    [SerializeField]
    public List<string> lines;
}
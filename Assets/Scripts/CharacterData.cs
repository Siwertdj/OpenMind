using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A scriptable object to store all data involving a single character.
/// </summary>
[CreateAssetMenu(fileName = "newCharacter", menuName = "Character")]

public class CharacterData : ScriptableObject
{
    public string characterName;
    public int id;
    public Sprite avatar;
    [Range(0.5f, 2f)] public float voicePitch = 1;

    [SerializeField] public KeyValuePair[] answers;
    [SerializeField] public DialogueLines[] greetings;
}

// KeyValuePair & DialogueLines must be individual objects in order to show up in the inspector
[Serializable]
public struct KeyValuePair
{
    [SerializeField] public Question question;
    [SerializeField] public List<string> answer;
    [SerializeField] public List<string> trait;
}

[Serializable]
public struct DialogueLines
{
    [SerializeField] public List<string> lines;
}
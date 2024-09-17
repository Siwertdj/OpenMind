using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newCharacter", menuName = "Character")]
public class CharacterData : ScriptableObject
{
    public string characterName;
    public int id;
    public Sprite avatar;

    // TODO: Potentially auto-fill the dialogueList automatically based on the content of each character's respective folder.
    public List<DialogueObject> dialogueList;

    [SerializeField]
    public KeyValuePair[] answers;

    public bool isCulprit;      // This character's random characteristic is revealed every cycle
    public bool isActive;       // If they havent yet been the victim, should be true. Use this to track who is "alive" and you can talk to, and who can be removed by the culprit\
}

[Serializable]
public class KeyValuePair
{
    [SerializeField]
    public Question question;
    [SerializeField]
    public List<string> answer;
}
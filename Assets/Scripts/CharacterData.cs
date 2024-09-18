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

    // TODO: Potentially auto-fill the dialogueList automatically based on the content of each character's respective folder.
    public List<DialogueObject> dialogueList;

    [SerializeField]
    public KeyValuePair[] answers;
}

[Serializable]
public struct KeyValuePair
{
    [SerializeField]
    public Question question;
    [SerializeField]
    public List<string> answer;
}
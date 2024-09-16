using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newCharacter", menuName = "Character")]
public class Character : ScriptableObject
{
    public string characterName;
    public int id;
    public Sprite avatar;

    // TODO: Potentially auto-fill the dialogueList automatically based on the content of each character's respective folder.
    public List<DialogueObject> dialogueList;

    public bool isCulprit;      // This character's random characteristic is revealed every cycle
    public bool isActive;       // If they havent yet been the victim, should be true. Use this to track who is "alive" and you can talk to, and who can be removed by the culprit
    public bool TalkedTo;       // If the player has already talked to this NPC in the current cycle, should be false at the start of every cycle and set to true once the player has talked with them
}

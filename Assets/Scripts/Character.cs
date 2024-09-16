using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newCharacter", menuName = "Character")]
public class Character : ScriptableObject
{
    public string characterName;
    public int id;
    public Sprite avatar;

    public List<DialogueObject> dialogueList;

    // TODO: Potentially auto-fill the dialogueList automatically based on the content of each character's respective folder.

}

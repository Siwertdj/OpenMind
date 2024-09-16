using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

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

    /// <summary>
    /// The logic for obtaining a random trait.
    /// If the random variable is left null, it will be obtained from gameManager, but it can be provided for slight optimization
    /// </summary>
    public string GetRandomTrait(Random random = null)
    {
        List<string> allTraits = new List<string>();
        
        //logic for obtaining traits, can be changed in the future
        allTraits = dialogueList.Select(dO => dO.answerText).ToList();
        //endlogic for traits

        if (random == null)
            random = FindObjectOfType<GameManager>().random;

        return allTraits[random.Next(allTraits.Count)];
    }
}

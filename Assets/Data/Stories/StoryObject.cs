using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newStory", menuName = "Story")]
public class StoryObject : ScriptableObject
{
    // This ScriptableObject contains the settings pertaining to a Story-type.

    [SerializeField] private string storyName;
    
    [Header("Story Assets")]
    [SerializeField] private GameObject introBackground;
    [SerializeField] private GameObject epilogueBackground;
    [SerializeField] public GameObject dialogueBackground;

    [Header("Game Settings")] 
    [SerializeField] public string victimDialogue;
    [SerializeField] public string hintDialogue;
    [SerializeField] public int numberOfCharacters;            // How many characters each session should have
    [SerializeField] private int numQuestions; // Amount of times the player can ask a question
    [SerializeField] private int minimumRemaining; // The amount of active characters at which the session should end
    [SerializeField] private bool immediateVictim; // Start the first round with an inactive characters
    
   

}

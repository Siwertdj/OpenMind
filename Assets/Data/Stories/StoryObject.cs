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
    // Introduction Dialogue
    [SerializeField] private DialogueLines introDialogue;
    [SerializeField] private GameObject introBackground;
    // Epilogue Dialogue
    [SerializeField] private DialogueLines epilogueDialogue;
    [SerializeField] private GameObject epilogueBackground;
    // NPC Dialogue Background
    public GameObject DialogueBackground { get; private set; }
    // Victim Dialogue
    public string VictimDialogue { get; private set; }
    // Hint Dialogue
    public string GintDialogue { get; private set; }

    [Header("Game Settings")] 
    [SerializeField] private int numQuestions; // Amount of times the player can ask a question
    [SerializeField] private int minimumRemaining; // The amount of active characters at which the session should end
    [SerializeField] private bool immediateVictim; // Start the first round with an inactive characters
    
    public int numberOfCharacters { get; private set; } // How many characters each session should have
    public DialogueObject IntroDialogueObject { get; private set; }
    public DialogueObject EpilogueDialogueObject { get; private set; }

}

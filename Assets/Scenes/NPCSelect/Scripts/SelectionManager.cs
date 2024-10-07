using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectionManager : MonoBehaviour
{
    // Prefab which is used to create SelectOption objects.
    [SerializeField] private GameObject selectionOption;
    
    // The SelectionSpace object, which has character spaces as children.
    public GameObject parent;

    // The header text at the top of the character selection screen.
    public TextMeshProUGUI headerText;

    private SceneController sc;
    
    // Variable which helps to decide whether the npcselect screen should be treated
    // as dialogue or as for deciding the criminal.
    // TODO: this 'string' is not very robust. We should find a better way to select the 'game state' during selection
    private string selectionType;
    
    private void Start()
    {
        sc = SceneController.sc;
        
        SetSceneType();
        
        //Line for testing decidecriminal.
        //selectionType = "decidecriminal";
        
        SetHeaderText(selectionType);
        GenerateOptions();
    }

    // Set the selectionType variable.
    private void SetSceneType()
    {        
        // If the number of characters has reached the minimum amount, and the player has no more questions left,
        // set the selectionType variable to decidecriminal.
        if (!GameManager.gm.EnoughCharactersRemaining() && !GameManager.gm.HasQuestionsLeft()) 
            selectionType = "decidecriminal";
        else
            selectionType = "dialogue";
    }
    
    /// <summary>
    /// Change the Header text if the culprit needs to be chosen.
    /// </summary>
    /// <param name="sceneType"> Can take "dialogue" or "decidecriminal" as value. </param>
    private void SetHeaderText(string sceneType)
    {
        if (sceneType == "decidecriminal")
            headerText.text = "Choose the character u think is the culprit";
    }
    
    /// <summary>
    /// Generates the selectOption objects for the characters.
    /// </summary>
    private void GenerateOptions()
    {
        // Create a SelectOption object for every character in currentCharacters.
        int counter = 0;
        foreach (CharacterInstance character in GameManager.gm.currentCharacters)
        {
            // Create a new SelectOption object.
            SelectOption newOption = Instantiate(selectionOption).GetComponent<SelectOption>();
            newOption.character = character;                
            // TODO: not correct yet, this will go out of bounds when there are more than 8 characters.
            // Sets one of the 8 character spaces as parent of the SelectOption object.
            newOption.transform.parent = parent.transform.GetChild(counter);
            // Sets the position of the SelectOption object to the same position as the parent (character space).
            newOption.transform.position = newOption.transform.parent.position;
            counter++;            
        }
    }
    
    /// <summary>
    /// Event for when a character is selected.
    /// </summary>
    /// <param name="option"> The character space on which has been clicked on. </param>
    /// TODO: Add an intermediate choice for the culprit. (if everyone agrees with the storyline epilogue)
    public void ButtonClicked(GameObject option)
    {
        // Get the SelectOption object from the character space.
        SelectOption selectOption = option.GetComponentInChildren<SelectOption>();
        // Only active characters can be talked to.
        if (selectOption.character.isActive)
        {
            // Start the dialogue if a criminal does not need to be decided yet.
            if (selectionType == "dialogue")
            {
                GameManager.gm.StartDialogue(selectOption.character);
            }
            else
            {
            
                // Set the FinalChosenCulprit variable to the chosen character in GameManager.
                //GameManager.gm.FinalChosenCuplrit = selectOption.character;
                // Set the dialogueRecipient to the chosen character in GameManager.
                //GameManager.gm.dialogueRecipient = selectOption.character;
                
                // Set the hasWon variable to true if the correct character has been chosen, else set it to false.
                if (GameManager.gm.GetCulprit().characterName == selectOption.character.characterName)
                    GameManager.gm.hasWon = true;
                else
                    GameManager.gm.hasWon = false;
                
                // Load the epilogue scene.
                GameManager.gm.StartEpilogueDialogue(selectOption.character);
            }
        }
    }
}

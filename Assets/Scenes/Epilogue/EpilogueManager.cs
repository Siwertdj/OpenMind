using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EpilogueManager : MonoBehaviour
{
    [Header ("Scene Objects")]
    [SerializeField] GameObject PortraitContainer;
    
    [Header("Resources")] 
    [SerializeField] private GameObject portraitPrefab;

    [Header("Epilogue")] 
    [SerializeField] private GameEvent onDialogueStart;
    
    /// <summary>
    /// Used to start dialogue in the epilogue scene (talking to the person chosen as the final choice).
    /// </summary>
    /// <param name="character"> The character which has been chosen. </param>
    public async void StartEpilogueDialogue(CharacterInstance character)
    {
        // Get the epilogue dialogue.
        remainingDialogueScenario = character.GetEpilogueDialogue(hasWon);

        // Create the DialogueObject and corresponding children.
        // This background displays the suspected culprit over the Dialogue-background
        var background = CreateDialogueBackground(character, story.dialogueBackground);
        var dialogueObject = GetEpilogueStart(background);
        
        // Transition to the dialogue scene.
        SceneManager.LoadScene("DialogueScene", LoadSceneMode.Additive);
        onDialogueStart.Raise(this, dialogueObject, character);
    }
    
    /// <summary>
    /// For each character provided, instantiate a prefab of a portrait with the correct portrait
    /// linked to the correct id.
    /// We later use this to see if we chose the correct Culprit.
    /// </summary>
    public void PopulateGrid(List<CharacterInstance> characters)
    {
        Transform parent = PortraitContainer.transform;
        foreach (CharacterInstance character in characters)
        { 
            // Create a new SelectOption object.
            SelectOption newOption = Instantiate(portraitPrefab).GetComponent<SelectOption>();
            newOption.character = character;
            newOption.transform.SetParent(parent);
        }
    }
    
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
                GameManager.gm.FinalChosenCuplrit = selectOption.character;
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

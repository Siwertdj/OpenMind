using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    [SerializeField] private GameObject selectionOption;
    
    public GameObject parent;

    // The text at the top
    public TextMeshProUGUI headerText;
    
    // Variable which helps to decide whether the npcselect screen should be treated
    // as dialogue or as for deciding the criminal.
    // (Weet niet zeker of deze variable verschoven moet worden naar gamemanager of dat je het hier in houdt maar ja)
    private string scene;
    
    private void Start()
    {
        Debug.Log("any questions left?:" + GameManager.gm.HasQuestionsLeft());
        setSceneType();
        
        //Line for testing decidecriminal.
        scene = "decidecriminal";
        
        setHeaderText(scene);
        GenerateOptions();
    }

    // Set the scene variable.
    private void setSceneType()
    {
        int numberOfActiveCharacters = GameManager.gm.currentCharacters.Where(c => c.isActive).Count();
        
        // If the number of characters has reached the minimum amount, and the player has no more questions left,
        // set the scene variable to decidecriminal.
        if (numberOfActiveCharacters <= GameManager.gm.minimumRemaining && !GameManager.gm.HasQuestionsLeft()) 
        {
            scene = "decidecriminal";
        }
        else
        {
            scene = "dialogue";
        }
    }

    // Change the Header text if the culprit needs to be chosen.
    private void setHeaderText(string sceneType)
    {
        if (sceneType == "decidecriminal")
        {
            headerText.text = "Choose the character u think is the culprit";
        }
    }
    
    // Generates the selectOption objects for the characters.
    private void GenerateOptions()
    {
        int counter = 0;
        foreach (CharacterInstance character in GameManager.gm.currentCharacters)
        {            
            // TODO: give proper transform?
            // TODO: make child of SelectionSpace, so it fits in there correctly
            SelectOption newOption = Instantiate(selectionOption).GetComponent<SelectOption>();
            newOption.character = character;                
            
            // not correct yet, this will go out of bounds when there are more than 8 characters.
            // sets one of the 8 characterspaces as parent of the selectoption object.
            newOption.transform.parent = parent.transform.GetChild(counter);
            // sets the position of the selectoption object to the same position as the parent (characterspace)
            newOption.transform.position = newOption.transform.parent.position;

            counter++;            
        }
    }
    
    // Event for when a character is selected.
    public void ButtonClicked(GameObject option)
    {
        // get the selectoption object
        SelectOption selectOption = option.GetComponentInChildren<SelectOption>();
        // Only active characters can be talked to
        if (selectOption.character.isActive)
        {
            // Start the dialogue if a criminal does not need to be decided yet.
            // (scene variable staat boven, moet nog ergens meegegeven worden wanneer de npcselectscene geladen wordt)
            if (scene == "dialogue")
            {
                // TODO: ensure that the correct id is passed based on the button
                
                GameManager.gm.StartDialogue(selectOption.character);
            }
            else
            {
                // Get the culprit from gamemanager and compare it with the clicked character
                // If the correct character is clicked, start the GameWin scene, else start the GameOver scene
                CharacterInstance culprit = GameManager.gm.GetCulprit();
                if (culprit.characterName == selectOption.character.characterName)
                {
                    Debug.Log("win"); // Debug.logs voor testen
                    GameManager.gm.ToggleNPCSelectScene();
                    GameManager.gm.ToggleGameWinScene();
                    
                }
                else
                {
                    Debug.Log("loss"); // Debug.logs voor testen
                    GameManager.gm.ToggleNPCSelectScene();
                    GameManager.gm.ToggleGameOverScene();
                    
                }
            }
            
        }
    }
}

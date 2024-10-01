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

    private SceneController sc;
    
    // Variable which helps to decide whether the npcselect screen should be treated
    // as dialogue or as for deciding the criminal.
    // TODO: this 'string' is not very robust. We should find a better way to select the 'game state' during selection
    private string selectionType;
    
    private void Start()
    {
        sc = SceneController.sc;
        
        setSceneType();
        
        //Line for testing decidecriminal.
        //selectionType = "decidecriminal";
        
        setHeaderText(selectionType);
        GenerateOptions();
    }

    // Set the selectionType variable.
    private void setSceneType()
    {        
        // If the number of characters has reached the minimum amount, and the player has no more questions left,
        // set the selectionType variable to decidecriminal.
        if (!GameManager.gm.EnoughCharactersRemaining() && !GameManager.gm.HasQuestionsLeft()) 
        {
            selectionType = "decidecriminal";
        }
        else
        {
            selectionType = "dialogue";
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
        // Get the selectoption object
        SelectOption selectOption = option.GetComponentInChildren<SelectOption>();
        // Only active characters can be talked to
        if (selectOption.character.isActive)
        {
            // Start the dialogue if a criminal does not need to be decided yet.
            if (selectionType == "dialogue")
            {
                // TODO: ensure that the correct id is passed based on the button 
                GameManager.gm.StartDialogue(selectOption.character);
            }
            else
            {
                // Get the culprit from gamemanager and compare it with the clicked character
                // If the correct character is clicked, start the GameWin selectionType, else start the GameOver selectionType
                CharacterInstance culprit = GameManager.gm.GetCulprit();
                // If the player chose the right target, the targetscene is 'GameWin', else 'GameOver'.
                SceneController.SceneName targetScene =
                    culprit.characterName == selectOption.character.characterName
                        ? SceneController.SceneName.GameWinScene
                        : SceneController.SceneName.GameOverScene;
                
                // Transition to the right scene.
                sc.TransitionScene(
                    SceneController.SceneName.NPCSelectScene,
                    targetScene, 
                    SceneController.TransitionType.Transition);
            }
            
        }
    }
}

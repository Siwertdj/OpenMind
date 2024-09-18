using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    [SerializeField] private GameObject selectionOption;
    
    public GameObject parent;
    
    private void Start()
    {
        GenerateOptions();
    }

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
    
    
    public void ButtonClicked(GameObject option)
    {
        // get the selectoption object
        SelectOption selectOption = option.GetComponentInChildren<SelectOption>();

        // Only active characters can be talked to
        if (selectOption.character.isActive)
        {
            // TODO: ensure that the correct id is passed based on the button
            GameManager.gm.StartDialogue(selectOption.character);
        }
    }
}

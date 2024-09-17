using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    [SerializeField] private GameObject selectionOption;

    // kan vgm wel weg maar ik laat het ff staan
    public GameObject[] characterTransforms;
    
    public GameObject parent;
    
    private void Start()
    {
        GenerateOptions();
    }

    private void GenerateOptions()
    {
        // get number of total characters from Gamemanager
        var gm = FindObjectOfType<GameManager>();

        int counter = 0;
        foreach (Character c in gm.currentCharacters)
        {
            //float width = characterspace.rect.width;
            //float height = characterspace.rect.height;
            
            // TODO: give proper transform?
            // TODO: make child of SelectionSpace, so it fits in there correctly
            GameObject newOption = Instantiate(selectionOption);
            newOption.GetComponent<SelectOption>().characterId = c.id;
            newOption.GetComponent<SelectOption>().avatar.sprite = c.avatar;
                
            
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
        SelectOption character = option.GetComponentInChildren<SelectOption>();
        
        // TODO: ensure that the correct id is passed based on the button
        var gm = FindObjectOfType<GameManager>();
        gm.StartDialogue(character.characterId);
    }
}

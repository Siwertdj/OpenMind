using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    [SerializeField] private GameObject selectionOption;
    
    private void Start()
    {
        GenerateOptions();
    }

    private void GenerateOptions()
    {
        // get number of total characters from Gamemanager
        var gm = FindObjectOfType<GameManager>();

        foreach (Character c in gm.currentCharacters)
        {
            // TODO: give proper transform?
            // TODO: make child of SelectionSpace, so it fits in there correctly
            GameObject newOption = Instantiate(selectionOption, transform);
            newOption.GetComponent<SelectOption>().characterId = c.id;
            newOption.GetComponent<SelectOption>().avatar.sprite = c.avatar;
            newOption.transform.parent =
                FindObjectOfType<Canvas>().transform.GetChild(1).transform.GetChild(1).transform;
            // FindObjectOfType<SelectionSpace>().transform;
        }
    }
    
    public void ButtonClicked(int id)
    {
        // TODO: ensure that the correct id is passed based on the button
        var gm = FindObjectOfType<GameManager>();
        gm.StartDialogue(id);
    }
}

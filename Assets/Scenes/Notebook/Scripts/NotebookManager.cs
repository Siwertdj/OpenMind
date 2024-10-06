using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using System.Linq;
using System.Net.Mime;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class NotebookManager : MonoBehaviour
{
    public GameObject inputField;
    public GameObject inputFieldCharacters;
    public GameObject characterInfo;
    public GameObject nameButtons;
    public Button personalButton;
    [NonSerialized] public NotebookData notebookData;
    private CharacterInstance currentCharacter;
    private List<Button> buttons;
    private Button selectedButton;

    // Start is called before the first frame update
    void Start()
    {
        // close character notes
        characterInfo.SetActive(false);
        inputFieldCharacters.SetActive(false);
        // Open personal notes
        inputField.SetActive(true);
        // assign character names to buttons
        InitializeCharacterButtons();
        // get notebookdata
        notebookData = GameManager.gm.notebookData;
        inputField.GetComponent<TMP_InputField>().text = notebookData.GetPersonalNotes();
        selectedButton = personalButton;
        personalButton.interactable = false;
    }

    public void InitializeCharacterButtons()
    {
        // get all buttons
        buttons = nameButtons.GetComponentsInChildren<Button>().ToList();
        for (int i = 0; i < buttons.Count; i++)
        {
            int id = i;
            Button button = buttons[id];
            button.GetComponentInChildren<TextMeshProUGUI>().text = 
                GameManager.gm.currentCharacters[i].characterName;
            button.onClick.AddListener(()=>CharacterTab(id));
        }
    }
    
    public void OpenPersonalNotes()
    {
        // Save character notes
        SaveNotes();
        // Close the character inputfield
        inputFieldCharacters.SetActive(false);
        
        // activate input
        inputField.SetActive(true);
        inputField.GetComponent<TMP_InputField>().text = notebookData.GetPersonalNotes();

        // Make button clickable
        ChangeButtons(personalButton);
    }
    
    private void CharacterTab(int id)
    {
        // Save notes
        SaveNotes();
        
        // Deactivate the personal notes tab if it's opened
        if (inputField.activeInHierarchy)
        {
            inputField.SetActive(false);
            characterInfo.SetActive(false);
        }
        
        // Activate written character notes
        inputFieldCharacters.SetActive(true);
        
        // Get the character
        currentCharacter = GameManager.gm.currentCharacters[id];
        
        // Write the notes to the notebook tab
        characterInfo.GetComponentInChildren<TextMeshProUGUI>().text = notebookData.GetAnswers(currentCharacter);
        
        // Write text to notebook
        inputFieldCharacters.GetComponent<TMP_InputField>().text = notebookData.GetCharacterNotes(currentCharacter);
        
        // Make button clickable
        ChangeButtons(buttons[id]);
    }
    
    public void ToggleCharacterInfo()
    {
        // Toggle character tab
        if (characterInfo.activeInHierarchy)
        {
            characterInfo.SetActive(false);
        }
        else
        {
            characterInfo.SetActive(true);
        }
    }
    
    public void SaveNotes()
    {
        if (inputField.activeInHierarchy)
        {
            // Save the written personal text to the notebook data
            notebookData.UpdatePersonalNotes(inputField.GetComponent<TMP_InputField>().text);
        }
        else
        {
            // Save the written character text to the notebook data
            notebookData.UpdateCharacterNotes(currentCharacter, 
                inputFieldCharacters.GetComponent<TMP_InputField>().text);
        }
    }

    private void ChangeButtons(Button clickedButton)
    {
        selectedButton.interactable = true;
        selectedButton = clickedButton;
        selectedButton.interactable = false;
    }
}

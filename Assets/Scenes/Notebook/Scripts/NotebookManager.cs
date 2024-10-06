using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using System.Linq;
using System.Net.Mime;
using UnityEngine.UI;

public class NotebookManager : MonoBehaviour
{
    public GameObject inputField;
    public GameObject inputFieldCharacters;
    public GameObject characterNotes;
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
        characterNotes.SetActive(false);
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
    
    private void CharacterTab(int id)
    {
        // Deactivate the personal notes tab, save text
        if (inputField.activeInHierarchy)
        {
            inputField.SetActive(false);
            notebookData.UpdatePersonalNotes(inputField.GetComponent<TMP_InputField>().text);
            // Activate character tab
            characterNotes.SetActive(false);
        }
        else
        {
            // Save the written text to the notebook data
            notebookData.UpdateNotes(currentCharacter, 
                inputFieldCharacters.GetComponent<TMP_InputField>().text);
        }
        // Deactivate written character notes
        inputFieldCharacters.SetActive(true);
        
        // Get the character
        currentCharacter = GameManager.gm.currentCharacters[id];
        
        // Write the notes to the notebook tab
        characterNotes.GetComponentInChildren<TextMeshProUGUI>().text = notebookData.GetAnswers(currentCharacter);
        
        // Write text to notebook
        inputFieldCharacters.GetComponent<TMP_InputField>().text = notebookData.GetPage(currentCharacter);
        
        // Make button clickable
        selectedButton.interactable = true;
        selectedButton = buttons[id];
        selectedButton.interactable = false;
    }

    public void ToggleCharacterInfo()
    {
        // Toggle character tab
        if (characterNotes.activeInHierarchy)
        {
            characterNotes.SetActive(false);
        }
        else
        {
            characterNotes.SetActive(true);
        }
    }
    
    public void SaveNotes()
    {
        if (inputField.activeInHierarchy)
        {
            notebookData.UpdatePersonalNotes(inputField.GetComponent<TMP_InputField>().text);
        }
        else
        {
            // Save the written text to the notebook data
            notebookData.UpdateNotes(currentCharacter, 
                inputFieldCharacters.GetComponent<TMP_InputField>().text);
        }
    }

    public void OpenPersonalNotes()
    {
        // activate input thing
        inputField.SetActive(true);
        inputField.GetComponent<TMP_InputField>().text = notebookData.GetPersonalNotes();

        // Save the written text to the notebook data
        notebookData.UpdateNotes(currentCharacter, 
            inputFieldCharacters.GetComponent<TMP_InputField>().text);
        // Close the inputfield
        inputFieldCharacters.SetActive(false);
        
        // Make button clickable
        selectedButton.interactable = true;
        selectedButton = personalButton;
        selectedButton.interactable = false;
    }
}

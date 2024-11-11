// This program has been developed by students from the bachelor Computer Science at Utrecht University within the Software Project course.
// © Copyright Utrecht University (Department of Information and Computing Sciences)
using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.UI;

/// <summary>
/// Manager class for the notebook scene.
/// </summary>
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

    /// <summary>
    /// On startup, go to the personal notes and make sure the correct data is shown
    /// </summary>
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
    
    /// <summary>
    /// Initialize the character buttons, use their names as the button text and add the button event.
    /// </summary>
    public void InitializeCharacterButtons()
    {
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
    
    /// <summary>
    /// Open the personal notes tab and load the notes.
    /// </summary>
    public void OpenPersonalNotes()
    {
        // Save character notes
        SaveNotes();
        // Close the character tab 
        inputFieldCharacters.SetActive(false);
        // activate input
        inputField.SetActive(true);
        inputField.GetComponent<TMP_InputField>().text = notebookData.GetPersonalNotes();
        // Make button clickable
        ChangeButtons(personalButton);
    }
    
    /// <summary>
    /// Open a character tab and load and display the notes on that character.
    /// </summary>
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
    
    /// <summary>
    /// Make the character log visible or not.
    /// </summary>
    public void ToggleCharacterInfo()
    {
        characterInfo.SetActive(!characterInfo.activeInHierarchy);
    }
    
    /// <summary>
    /// Save the notes on the (character) inputfield to the notebookdata.
    /// </summary>
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
    
    /// <summary>
    /// Make the clicked button non-interactable and make the last clicked buttons interactable again.
    /// </summary>
    private void ChangeButtons(Button clickedButton)
    {
        selectedButton.interactable = true;
        selectedButton = clickedButton;
        selectedButton.interactable = false;
    }
}

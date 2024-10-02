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
    public GameObject characterNotes;
    private string notesFilePath = "Assets\\Scenes\\Notebook\\";
    private GameManager gameManager;
    public GameObject nameButtons;
    private List<CharacterInstance> characters;
    private Dictionary<Button, CharacterInstance> ButtonToCharacter = new();

    // Start is called before the first frame update
    void Start()
    {
        // Read notes previously saved to notes.txt
        using StreamReader reader = new(notesFilePath + "notes.txt");
        string fetchedNotes = reader.ReadToEnd();
        Debug.Log("Fetched notes: " + fetchedNotes);
        inputField.GetComponent<TMP_InputField>().text = fetchedNotes;  // Put said notes into the inputfield
        
        // close character notes
        characterNotes.SetActive(false);
        
        // get all buttons
        List<Button> buttons = nameButtons.GetComponentsInChildren<Button>().ToList();
        
        // get characters
        gameManager = FindAnyObjectByType<GameManager>();
        characters = gameManager.currentCharacters;
        
        // Combine buttons and characters
        ButtonToCharacter = buttons.Zip(characters, (k, v) => new { k, v }).ToDictionary(x => x.k, x => x.v);

        // Assign character names to the buttons
        foreach (Button b in ButtonToCharacter.Keys)
        {
            b.GetComponentInChildren<TextMeshProUGUI>().text = ButtonToCharacter[b].characterName;
        }
    }
    
    public void CharacterTab(Button thisButton)
    {
        // Deactivate the personal notes tab
        inputField.SetActive(false);
        // Activate character tab
        characterNotes.SetActive(true);
        
        // Get the character
        CharacterInstance currentCharacter = ButtonToCharacter[thisButton];
        
        // Get the string to display in the notebook
        string answerNotes;
        
        if (currentCharacter.AnsweredQuestions.Count > 0)
        {
            answerNotes = GetCollectedInfo(currentCharacter);
        }
        else
        {
            answerNotes = "You have not asked " + currentCharacter.characterName + " any questions.";
        }
        // Write the notes to the notebook tab
        characterNotes.GetComponentInChildren<TextMeshProUGUI>().text = answerNotes;
    }

    private string GetCollectedInfo(CharacterInstance character)
    {
        string output = "";

        foreach (Question q in character.AnsweredQuestions)
        {
            output += q + "\n";
            foreach (string s in character.Answers[q])
            {
                output += s + " ";
            }
            output += "\n \n";
        }
        return output;
    }

    public void OpenPersonalNotes()
    {
        // activate input thing
        inputField.SetActive(true);
        //deactivate character thing
        characterNotes.SetActive(false);
    }
    
    public void SaveNotes()
    {
        Debug.Log("Saving...");
        // Write written notes from the TMP input field to notes.txt to save notes after closing scene
        string savedNotes = inputField.GetComponent<TMP_InputField>().text;
        using (StreamWriter writer = new (notesFilePath + "notes.txt"))
        {
            writer.WriteLine(savedNotes);
        }

        Debug.Log("Saved notes: " + savedNotes);
    }
}

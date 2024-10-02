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
    private Dictionary<Button, CharacterInstance> ButChar = new();

    // Start is called before the first frame update
    void Start()
    {
        // Read notes previously saved to notes.txt
        using StreamReader reader = new(notesFilePath + "notes.txt");
        string fetchedNotes = reader.ReadToEnd();
        Debug.Log("Fetched notes: " + fetchedNotes);
        inputField.GetComponent<TMP_InputField>().text = fetchedNotes;  // Put said notes into the inputfield
        
        // close characternotes
        characterNotes.SetActive(false);
        gameManager = FindAnyObjectByType<GameManager>();
        
        // get all buttons
        List<Button> buttons = nameButtons.GetComponentsInChildren<Button>().ToList();
        // get characters
        characters = gameManager.currentCharacters;
        ButChar = buttons.Zip(characters, (k, v) => new { k, v }).ToDictionary(x => x.k, x => x.v);
        
        // assign character names to buttons
        //for (int i = 0; i < characters.Count; i++)
        //{
        //    buttons[i].GetComponentInChildren<TextMeshProUGUI>().text = characters[i].characterName;
        //}
        
        foreach (Button b in ButChar.Keys)
        {
            b.GetComponentInChildren<TextMeshProUGUI>().text = ButChar[b].characterName;
        }
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

    public void CharacterTab(Button thisButton)
    {
        // deactivate the other text
        inputField.SetActive(false);
        //activate character thing
        characterNotes.SetActive(true);

        string answerNotes = "Your notes on " + ButChar[thisButton].characterName + ".\n";
        answerNotes += GetQuestions(ButChar[thisButton]);
        
        characterNotes.GetComponentInChildren<TextMeshProUGUI>().text = answerNotes;
    }

    private string GetQuestions(CharacterInstance character)
    {
        return "";
    }

    public void OpenPersonalNotes()
    {
        // activate input thing
        inputField.SetActive(true);
        //deactivate character thing
        characterNotes.SetActive(false);
    }
}

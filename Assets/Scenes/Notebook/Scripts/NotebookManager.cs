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

    // Start is called before the first frame update
    void Start()
    {
        // Read notes previously saved to notes.txt
        using StreamReader reader = new(notesFilePath + "notes.txt");
        string fetchedNotes = reader.ReadToEnd();
        Debug.Log("Fetched notes: " + fetchedNotes);
        inputField.GetComponent<TMP_InputField>().text = fetchedNotes; // Put said notes into the inputfield
        
        // close character notes
        characterNotes.SetActive(false);

        // get characters
        gameManager = FindAnyObjectByType<GameManager>();
        characters = gameManager.currentCharacters;
        
        // assign character names to buttons
        InitializeCharacterButtons();
    }

    private void InitializeCharacterButtons()
    {
        // get all buttons
        List<Button> buttons = nameButtons.GetComponentsInChildren<Button>().ToList();
        // create button events and name text
        for (int i = 0; i < characters.Count; i++)
        {
            int characterIndex = i;
            Button button = buttons[characterIndex];
            buttons[characterIndex].GetComponentInChildren<TextMeshProUGUI>().text = 
                characters[characterIndex].characterName;
            
            button.onClick.AddListener(()=>CharacterTab(characterIndex));
        }
    }
    
    private void CharacterTab(int characterIndex)
    {
        // Deactivate the personal notes tab
        inputField.SetActive(false);
        // Activate character tab
        characterNotes.SetActive(true);
        
        // Get the character
        CharacterInstance currentCharacter = characters[characterIndex];
        
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
            output += GetQuestionText(q).ToUpper() + "\n";
            foreach (string s in character.Answers[q])
            {
                output += s + " ";
            }
            output += "\n \n";
        }
        return output;
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

    public void OpenPersonalNotes()
    {
        // activate input thing
        inputField.SetActive(true);
        //deactivate character thing
        characterNotes.SetActive(false);
    }
    
    private string GetQuestionText(Question questionType)
    {
        return questionType switch
        {
            Question.Name => "Name",
            Question.Age => "Age",
            Question.Wellbeing => "Wellbeing",
            Question.Political => "Political ideology",
            Question.Personality => "Personality",
            Question.Hobby => "Hobbies",
            Question.CulturalBackground => "Cultural background",
            Question.Education => "Education level",
            Question.CoreValues => "Core values",
            Question.ImportantPeople => "Most important people",
            Question.PositiveTrait => "Positive trait",
            Question.NegativeTrait => "Bad trait",
            Question.OddTrait => "Odd trait",
            _ => "",
        };
    }
}

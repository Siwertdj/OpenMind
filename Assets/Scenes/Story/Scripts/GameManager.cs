using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = System.Random;

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField] public int numberOfCharacters;
    [SerializeField] private List<CharacterData> characters;
    [NonSerialized] public int numTalked; // The amount of times  the player has talked, should be 0 at the start of each cycle
    [SerializeField] private int numQuestions; // Amount of times the player can ask a question
    [SerializeField] private int minimumRemaining;
    [SerializeField] private bool immediateVictim;
    
    // The current "active" characters, any characters that became inactive should be removed from this list.
    public List<CharacterInstance> currentCharacters; 
    // Target of the current dialogue
    public CharacterInstance dialogueRecipient;
    public DialogueObject dialogueObject;
    
    /// <summary>
    /// The amount of times  the player has talked, should be 0 at the start of each cycle
    /// </summary>
    [NonSerialized] public int numQuestionsAsked;

    /// <summary>
    /// Amount of times the player can ask a question
    /// </summary>

    public Random random = new Random(); //random variable is made global so it can be reused
    public static GameManager gm;       // static instance of the gamemanager

    
    private void Awake()
    {
        Load();
    }

    private void Start()
    {
        // Initialize an empty list of characters
        currentCharacters = new List<CharacterInstance>();
        // Now, populate this list.
        PopulateCharacters();
        // Prints to console the characters that were selected to be in the current game. UNCOMMENT WHILE DEBUGGING
        Test_CharactersInGame();
        // On load start cycle, depending on whether we want an immediate victim or not.
        if (immediateVictim)
            StartCycle();
        else
            FirstCycle();
    }

    /// <summary>
    /// Creates persistent toolbox of important manager objects, 
    /// </summary>
    private void Load()
    {
        gm = this;
        
        // Make parentobject persistent, so that all objects in the toolbox remain persistent.
        // This includes gamemanager, audiomanager, main camera and eventsystem.
        DontDestroyOnLoad(gameObject.transform.parent);
        SceneManager.UnloadSceneAsync("Loading");
    }
    
    /// <summary>
    /// Makes a randomized selection of characters for this loop of the game, from the total database of all characters.
    /// Also makes sure they are all set to 'Active', and selects a random culprit.
    /// </summary>
    private void PopulateCharacters()
    {
        // Create a random population of 'numberOfCharacters' number, initialize them, and choose a random culprit.

        // Create array to remember what indices we have already visited, so we don't get doubles.
        // Because this empty array is initiated with 0's, we need to offset our number generated below with +1.
        // When we use this index to retrieve a character from the characters-list, we reverse the offset with -1.
        int[] visitedIndices = new int[numberOfCharacters];

        // We iterate over a for-loop to find a specific number of characters to populate our game with.
        // We clamp it down to the smallest value, in case numberOfCharacters is more than the number we have generated.
        numberOfCharacters = Math.Min(characters.Count, numberOfCharacters);
        for (int i = 0; i < numberOfCharacters; i++)
        {
            bool foundUniqueInt = false; // We use this bool to exist the while-loop when we find a unique index
            while (!foundUniqueInt)
            {
                int index = random.Next(0, numberOfCharacters) + 1; // offset by 1 to check existence

                string arrayString = "";
                for (int j = 0; j< visitedIndices.Length; j++)
                    arrayString += (visitedIndices[j] + ", ");
                
                //Debug.Log("Trying index: " + index + " over index-array: [" + arrayString + "]");
                if (!visitedIndices.Contains(index))
                {
                    //Debug.Log("Unique index found!");
                    var toAdd = characters[index - 1]; // correct the offset
                    currentCharacters.Add(new CharacterInstance(toAdd)); // add the character we found to the list of current characters
                    visitedIndices[i] = index; // add the index with the offset to the array of visited indices
                    foundUniqueInt = true; // change the boolean-value to exit the while-loop
                }
                else
                {
                    //Debug.Log("Index not unique");
                }
            }
        }

        // Make sure all the characters are 'active'
        foreach (var c in currentCharacters)
        {
            c.isActive = true;
            c.isCulprit = false;
        }
        //Randomly select a culprit
        currentCharacters[random.Next(0, numberOfCharacters)].isCulprit = true;
    }

    /// <summary>
    /// Returns the culprit, used to give hints for the companion
    /// Assumes a culprit exists
    /// </summary>
    public CharacterInstance GetCulprit() => currentCharacters.Find(c => c.isCulprit);

    public bool EnoughCharactersRemaining()
    {
        int numberOfActiveCharacters = GameManager.gm.currentCharacters.Count(c => c.isActive);
        return numberOfActiveCharacters > GameManager.gm.minimumRemaining;
    }

    /// <summary>
    /// Returns a random (non-culprit and active) character, used to give hints for the companion
    /// Assumes there is only 1 culprit
    /// </summary>
    public CharacterInstance GetRandomVictimNoCulprit()
    {
        List<CharacterInstance> possibleVictims = currentCharacters.FindAll(c => !c.isCulprit && c.isActive); 
        return possibleVictims[random.Next(possibleVictims.Count- 1)];
    }

    /// <summary>
    /// Prints the name of all characters in the current game to the console, for debugging purposes.
    /// </summary>
    private void Test_CharactersInGame()
    {
        string output = "";
        for (int i = 0; i < currentCharacters.Count; i++)
        {
            // If its the second last, surfix is "and", if its the last, there is no surfix. If its any other, its ", "
            output += (currentCharacters[i].characterName + (i + 1 == currentCharacters.Count
                ? "."
                : (i + 2 == currentCharacters.Count ? " and " : ", ")));
        }
        Debug.Log("The " + currentCharacters.Count + " characters currently in game are " + output);
        
        foreach (var c in currentCharacters.Where(c => c.isCulprit))
            Debug.Log(c.characterName + " is the culprit!");
    }

    // IF we want to start the first cycle without casualties, we use this instead.
    private void FirstCycle()
    {
        // Reset number of times the player has talked
        numQuestionsAsked = 0;
        // Start the NPC Selection scene
        SceneController.sc.ToggleNPCSelectScene();
    }
    
    /// <summary>
    /// The main cycle of the game.
    /// This should loop everytime the player speaks to an NPC until a certain number of NPCs have been spoken to,
    /// at that point the cycle ends and the player has to choose which NPC they think is the culprit
    /// </summary>
    private void StartCycle()
    {
        // Choose a victim, make them inactive, and print the hints to the console.
        string victimName = ChooseVictim();
        // Transition
        gameObject.GetComponent<UIManager>().Transition(victimName + " went home..");
        // Reset number of times the player has talked
        numQuestionsAsked = 0;
        // Start the NPC Selection scene
        SceneController.sc.ToggleNPCSelectScene();
    }

    public void EndCycle() 
    {
        if (EnoughCharactersRemaining())
            StartCycle();
        else
        {
            Debug.Log("Ending cycle: not enough characters remaining");
            SceneController.sc.ToggleNPCSelectScene();
        }
    }

    public void CheckEndCycle()
    {
        Debug.Log("Checking end Cycle");
        if (!HasQuestionsLeft())
        {
            Debug.Log("No questions remaining");
            EndCycle();
        }
    }
    
    /// <summary>
    /// Chooses a victim, changes the isActive bool to 'false' and randomly selects a trait from both the culprit and
    /// the victim that is removed from their list of questions and prints to to the debuglog
    /// </summary>
    private string ChooseVictim()
    {
        CharacterInstance culprit = GetCulprit();
        CharacterInstance victim = GetRandomVictimNoCulprit();

        // Victim put on inactive so we cant ask them questions
        victim.isActive = false;
        
        //TODO: wait until I have a dialogue box to put this in
        //Debug.Log(string.Join(", ", randTraitCulprit)); 
        //Debug.Log(string.Join(", ", randTraitVictim));
        return victim.characterName;
    }

    /// <summary>
    /// Checks if the player can ask more questions this cycle.
    /// </summary>
    /// <returns>True if player can ask more questions, otherwise false.</returns>
    public bool HasQuestionsLeft()
    {
        Debug.Log("Has questions left: " + (numQuestionsAsked < numQuestions));
        return numQuestionsAsked < numQuestions;
    }
    public string GetPromptText(Question questionType)
    {
        return questionType switch
        {
            Question.Name => "What is your name?",
            Question.Age => "How old are you?",
            Question.Wellbeing => "How are you doing?",
            Question.Political => "What are your political thoughts?",
            Question.Personality => "Can you describe what your personality is like?",
            Question.Hobby => "What are some of your hobbies?",
            Question.CulturalBackground => "What is your cultural background?",
            Question.Education => "What is your education level?",
            Question.CoreValues => "What core values are the most important to you?",
            Question.ImportantPeople => "Who are the most important people in your life?",
            Question.PositiveTrait => "What do you think is your best trait?",
            Question.NegativeTrait => "What is a bad trait you may have?",
            Question.OddTrait => "Do you have any odd traits?",
            _ => "",
        };
    }

    /// <summary>
    /// Closes the game.
    /// </summary>
    public void EndGame()
    {
        Debug.Log("End game.");
        UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }
    
    /// <summary>
    /// Reset game from start with the same characters
    /// </summary>
    public void RetryStoryScene()
    {
        Debug.Log("Retry story scene");

        // Unload all active scenes except the story scene
        SceneController.sc.UnloadAdditiveScenes();
        // Reset these characters
        foreach (CharacterInstance character in currentCharacters)
        {
            // Reset the questions and active-status of this character
            character.isActive = true;
            character.InitializeQuestions();
        }
        //Test_CharactersInGame();
        if (immediateVictim)
            StartCycle();
        else
            FirstCycle();
    }
    
    /// <summary>
    /// Restart game from start with new characters
    /// </summary>
    public void RestartStoryScene()
    {
        Debug.Log("Restart story scene");
        //Remove the gamemanager to start a new game
        //Destroy(gameObject);
        // Load the story scene
        //SceneManager.LoadScene("StoryScene");

        //or
        // unload all scenes except story scene
        SceneController.sc.UnloadAdditiveScenes();
        // reset game
        Start();
        
    }
    
    /// <summary>
    /// Opens the dialogue scene to talk to the given character.
    /// </summary>
    /// <param name="character">The character the player will talk to</param>
    public void StartCharacterDialogue(CharacterInstance character)
    {
        SceneController.sc.ToggleNPCSelectScene();

        dialogueRecipient = character;
        dialogueObject = new SpeakingObject(character.GetGreeting());
        dialogueObject.Responses.Add(new QuestionObject());
        SceneManager.LoadScene("DialogueScene", LoadSceneMode.Additive);
    }

    /// <summary>
    /// <i>Deprecated</i>, this function only remains in case we want to do something with it later.
    /// </summary>
    /// <param name="id"></param>
    public void StartDialogue(int id)
    {
        SceneController.sc.ToggleNPCSelectScene(); // NPC selected, so unload
        
        dialogueRecipient = currentCharacters[id];
        SceneManager.LoadScene("DialogueScene", LoadSceneMode.Additive);
    }    
}

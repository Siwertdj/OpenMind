using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = System.Random;

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField] public int numberOfCharacters;
    [SerializeField] private List<CharacterData> characters;

    [SerializeField] private int numQuestions; // Amount of times the player can ask a question
    [SerializeField] private int minimumRemaining;
    [SerializeField] private bool immediateVictim;
    
    // The current "active" characters, any characters that became inactive should be removed from this list.
    public List<CharacterInstance> currentCharacters; 
    
    /// The amount of times  the player has talked, should be 0 at the start of each cycle
    [NonSerialized] public int numQuestionsAsked;

    // Game Events
    public GameEvent onDialogueStart;
    
    // Instances
    public Random random = new Random(); //random variable is made global so it can be reused
    public static GameManager gm;       // static instance of the gamemanager
    private SceneController sc;
    
    // Called when this script instance is being loaded
    private void Awake()
    {
        Load();
    }

    private void Start()
    {
        sc = SceneController.sc;
        
        // Initialize an empty list of characters
        currentCharacters = new List<CharacterInstance>();
        // Now, populate this list.
        PopulateCharacters();
        // Prints to console the characters that were selected to be in the current game. UNCOMMENT WHILE DEBUGGING
        //Test_CharactersInGame();
        // On load start cycle, depending on whether we want an immediate victim or not.
        FirstCycle();
    }

    /// <summary>
    /// Creates persistent toolbox of important manager objects, 
    /// </summary>
    private void Load()
    {
        gm = this;
        
        // Make parentobject persistent, so that all objects in the toolbox remain persistent.
        DontDestroyOnLoad(gameObject.transform.parent);
    }
    
    // This region contains methods that start or end the cycles.
    #region Cycles
    /// <summary>
    /// Works like StartCycle, but can optionally skip the immediate victim.
    /// This method starts the game with a special SceneTransition-invocation for the first scene of the game.
    /// </summary>
    private void FirstCycle()
    {
        if (immediateVictim)
        {
            // Choose a victim, make them inactive, and print the hints to the console.
            string victimName = ChooseVictim();
            // Transition-effect
            gameObject.GetComponent<UIManager>().Transition(victimName + " went home..");
        }
        // Reset number of times the player has talked
        numQuestionsAsked = 0;
        // Start the game at the first scene; the NPC Selection scene
        sc.StartScene(SceneController.SceneName.NPCSelectScene);
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
        sc.TransitionScene(
            SceneController.SceneName.DialogueScene, 
            SceneController.SceneName.NPCSelectScene, 
            SceneController.TransitionType.Transition);
    }

    /// <summary>
    /// Ends the cycle when all questions have been asked.
    /// If we have too few characters remaining, we must select the culprit,
    /// otherwise we start a new cycle.
    /// </summary>
    public void EndCycle() 
    {
        // Start Cycle as normal
        if (EnoughCharactersRemaining())    
            StartCycle();
        // Select the Culprit
        else
        {
            sc.TransitionScene(
                SceneController.SceneName.DialogueScene, 
                SceneController.SceneName.NPCSelectScene, 
                SceneController.TransitionType.Transition);
        }
    }
    #endregion
    
    #region InstantiateGameOrCycles
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
                
                if (!visitedIndices.Contains(index))
                {
                    var toAdd = characters[index - 1]; // correct the offset
                    currentCharacters.Add(new CharacterInstance(toAdd)); // add the character we found to the list of current characters
                    visitedIndices[i] = index; // add the index with the offset to the array of visited indices
                    foundUniqueInt = true; // change the boolean-value to exit the while-loop
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
    /// Returns the culprit, used to give hints for the companion
    /// Assumes a culprit exists
    /// </summary>
    public CharacterInstance GetCulprit() => currentCharacters.Find(c => c.isCulprit);

    /// <summary>
    /// Returns a random (non-culprit and active) character, used to give hints for the companion
    /// Assumes there is only 1 culprit
    /// </summary>
    public CharacterInstance GetRandomVictimNoCulprit()
    {
        List<CharacterInstance> possibleVictims = currentCharacters.FindAll(c => !c.isCulprit && c.isActive); 
        return possibleVictims[random.Next(possibleVictims.Count- 1)];
    }

    #endregion
    
    // This region contains methods that directly change the Game State.
    #region ChangeGameState
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
        // unload all scenes except story scene
        SceneController.sc.UnloadAdditiveScenes();
        // reset game
        Start();
        
    }

    /// <summary>
    /// Can be called to start Dialogue with a specific character, taking a CharacterInstance as parameter.
    /// This toggles-off the NPCSelectScene,
    /// and switches the dialogueRecipient-variable to the characterInstance that is passed as a parameter.
    /// Then, it loads the DialogueScene.
    /// </summary>
    /// <param name="character"></param>
    ///  TODO: Should use the id of a character instead of the CharacterInstance.
    public async void StartDialogue(CharacterInstance character)
    {
        // Transition to dialogue scene and await the loading operation
        await sc.TransitionScene(
            SceneController.SceneName.NPCSelectScene,
            SceneController.SceneName.DialogueScene,
            SceneController.TransitionType.Transition);

        // Until DialogueManager gets its information, it shouldnt do anything there.
        var dialogueRecipient = character;
        var dialogueObject = new SpeakingObject(character.GetGreeting());
        dialogueObject.Responses.Add(new QuestionObject());

        // The gameevent here should pass the information to Dialoguemanager
        // ..at which point dialoguemanager will start.
        Debug.Log("Raising event to pass data to DialogueManager.");
        onDialogueStart.Raise(this, dialogueRecipient, dialogueObject);
    }    
    
    /// <summary>
    /// Called by DialogueManager when dialogue is ended, by execution of a TerminateDialogueObject.
    /// Checks if questions are remaining:
    /// .. if no, end cycle.
    /// .. if yes, 'back to NPCSelect'-button was clicked, so don't end cycle.
    /// </summary>
    /// TODO: Check naming convention for events and listeners, if this is right
    public void EndDialogue()
    {
        if (!HasQuestionsLeft())
        {
            // No questions left, so we end the cycle 
            EndCycle();
        }
        else
        {
            // We can still ask questions, so toggle back to NPCSelectMenu without ending the cycle.
            sc.TransitionScene(
                SceneController.SceneName.DialogueScene, 
                SceneController.SceneName.NPCSelectScene, 
                SceneController.TransitionType.Transition);
        }
    }
    #endregion

    // This region contains methods that check certain properties that affect the Game State.
    #region CheckProperties
    /// <summary>
    /// Checks if the number of characters
    /// ..in the currently active game (selected in the 'currentCharacters'-list)
    /// ..that are also 'active' (the isActive-bool of these CharacterInstances)
    /// is more than the gamemanager variable 'numberOfActiveCharacters', which is the minimum amount of characters
    /// that should be remaining until we transition into selecting a culprit.
    /// </summary>
    /// <returns></returns>
    public bool EnoughCharactersRemaining()
    {
        int numberOfActiveCharacters = GameManager.gm.currentCharacters.Count(c => c.isActive);
        return numberOfActiveCharacters > GameManager.gm.minimumRemaining;
    }
    
    
    /// <summary>
    /// Checks if the player can ask more questions this cycle.
    /// </summary>
    /// <returns>True if player can ask more questions, otherwise false.</returns>
    public bool HasQuestionsLeft()
    {
        return numQuestionsAsked < numQuestions;
    }
    #endregion

    // This region contains methods necessary purely for debugging-purposes.
    #region DebuggingMethods
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
    #endregion
}

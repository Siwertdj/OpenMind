using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.U2D.Animation;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.TextCore.Text;
using Random = System.Random;

public class GameManager : MonoBehaviour
{
    [Header("Game Resources")]
    [SerializeField] private List<CharacterData> characters; // The full list of characters in the game

    [Header("Background Prefabs")]
    [SerializeField] private GameObject avatarPrefab; // A prefab containing a character
    [SerializeField] private GameObject[] backgroundPrefabs; // The list of backgrounds for use in character dialogue
    
    [Header("Events")]
    public GameEvent onDialogueStart;
    
    // GAME VARIABLES
    /*private int numberOfCharacters; // How many characters each session should have
    private int numQuestions; // Amount of times the player can ask a question
    private int minimumRemaining; // The amount of active characters at which the session should end
    private bool immediateVictim; // Start the first round with an inactive characters*/
    [NonSerialized] public int numQuestionsAsked;   // The amount of times  the player has talked, should be 0 at the start of each cycle
    public List<CharacterInstance> currentCharacters;   // The list of the characters in the current game. This includes both active and inactive characters
    [NonSerialized] public GameState gameState;     // This gamestate is tracked to do transitions properly and work the correct behaviour of similar methods
    public StoryObject story; // Contains information about the current game pertaining to the story
    
    // EPILOGUE VARIABLES
    public bool hasWon;     // Set this bool to true if the correct character has been chosen at the end, else false.
    public CharacterInstance IntermediateChosenCuplrit; // Save the character that has been chosen during the intermediate choice moment.
    public CharacterInstance FinalChosenCuplrit;    // Save the character that has been chosen at the end of the game.
    public List<List<string>> remainingDialogueScenario; // Holds the remainder of the conversation in the epilogue.
    
    // Instances
    public Random random = new Random(); //random variable is made global so it can be reused
    public static GameManager gm;       // static instance of the gamemanager
    private SceneController sc;
    public NotebookData notebookData;

    // Enumerations
    #region Enumerations
    // This enumeration defines all the possible GameStates, which we can use to test correct behavior
    public enum GameState
    {
        // Is there a gamestate for when the game is loading in?
        Loading,        //      --> NPCSelect, HintDialogue(immediate victim)
        NpcSelect,      //      --> NpcDialogue
        CulpritSelect,  //      --> GameWon, GameLoss
        NpcDialogue,    //      --> NpcSelect, CulpritSelect
        HintDialogue,   //      --> NpcSelect
        GameLoss,       //      --> Loading (restart/retry)
        GameWon,        //      --> Loading (restart/retry)
        Epilogue
    }
    #endregion
    
    // Called when this script instance is being loaded
    private void Awake()
    {
        // make GameManager static, so it can be easily reached
        gm = this;

        // Make parentobject (the toolbox) persistent, so that all objects in the toolbox remain persistent.
        DontDestroyOnLoad(gameObject.transform.parent);
    }
    
    // Calls FirstCycle(), this function is called by the NewGame button on the StartScreen
    public void StartGame(Component sender, params object[] data)
    {
        // Set reference to static SceneController
        sc = SceneController.sc;

        // Set the gamestory based on the data we passed
        if (data[0] is StoryObject storyObject)
        {
            // Set the story object
            story = storyObject;
            // Initialize a new game
            NewGame();
        }
        else if (data[0] is SaveData saveData)
        {
            LoadGame(saveData);
        }
    }

    private void LoadGame(SaveData saveData)
    {
        currentCharacters = currentCharacters.Select(c =>
        {
            c.isActive = saveData.activeCharacters.Contains(c.id);
            c.isCulprit = saveData.culprit == c.id;
            if (c.isActive)
            {
                c.RemainingQuestions = saveData.remainingQuestions.First(qs => qs.Item1 == c.id).Item2;
            }
            c.AskedQuestions = saveData.askedQuestionsPerCharacter.First(qs => qs.Item1 == c.id).Item2;
            notebookData.UpdateCharacterNotes(c, saveData.characterNotes.First(note => note.Item1 == c.id).Item2);
            return c;
        }).ToList();
        
        notebookData.UpdatePersonalNotes(saveData.personalNotes);
    }

    /// <summary>
    /// This private method initializes a new game.
    /// </summary>
    private void NewGame()
    {
        // put data from story into variables.
        /*numberOfCharacters = story.numberOfCharacters;
        numQuestions = story.numQuestions;
        minimumRemaining = story.minimumRemaining;
        immediateVictim = story.immediateVictim;*/
        
        // Initialize an empty list of characters
        currentCharacters = new List<CharacterInstance>();
        // Now, populate this list.
        PopulateCharacters();
        // Empty notebook data
        notebookData = new NotebookData();

        FirstCycle();
    }
    
    
    // This region contains methods that start or end the cycles.
    #region Cycles
    /// <summary>
    /// Works like StartCycle, but can optionally skip the immediate victim.
    /// This method starts the game with a special SceneTransition-invocation for the first scene of the game.
    /// </summary>
    private void FirstCycle()
    {
        if (story.immediateVictim)
        {
            // Choose a victim, make them inactive, and print the hints to the console.
            string victimName = ChooseVictim();
            // Transition-effect
            CycleTransition(victimName);
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
        CycleTransition(victimName);
        // Reset number of times the player has talked
        numQuestionsAsked = 0;

        var dialogue = new List<string> {
            $"{victimName} has disappeared.",
            "There is some new information about the culprit:",
        };
        dialogue.AddRange(GetCulprit().GetRandomTrait());

        // Creates Dialogue that says who disappeared and provides a new hint.
        var dialogueObject = new SpeakingObject(dialogue, CreateDialogueBackground(null, story.hintBackground));

        StartDialogue(dialogueObject);
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
        int[] visitedIndices = new int[story.numberOfCharacters];

        // We iterate over a for-loop to find a specific number of characters to populate our game with.
        // We clamp it down to the smallest value, in case numberOfCharacters is more than the number we have generated.
        story.numberOfCharacters = Math.Min(characters.Count, story.numberOfCharacters);
        for (int i = 0; i < story.numberOfCharacters; i++)
        {
            bool foundUniqueInt = false; // We use this bool to exist the while-loop when we find a unique index
            while (!foundUniqueInt)
            {
                int index = random.Next(0, story.numberOfCharacters) + 1; // offset by 1 to check existence

                string arrayString = "";
                for (int j = 0; j < visitedIndices.Length; j++)
                    arrayString += (visitedIndices[j] + ", ");

                if (!visitedIndices.Contains(index))
                {
                    var toAdd = characters[index - 1]; // correct the offset
                    currentCharacters.Add(
                        new CharacterInstance(toAdd)); // add the character we found to the list of current characters
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
        currentCharacters[random.Next(0, story.numberOfCharacters)].isCulprit = true;
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
        
        // Start the game again
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
        NewGame();     
    }

    /// <summary>
    /// Performs a visual fade-in/out when called,
    /// displaying the victim's name and their fate, depending on the Story we are currently in.
    /// </summary>
    /// <param name="victimName"></param>
    private void CycleTransition(string victimName)
    {
        string victimFate = story.victimDialogue;
        gameObject.GetComponent<UIManager>().Transition(victimName + " " + victimFate);
    }
    
    #endregion

    // This region contains methods regarding dialogue
    #region Dialogue
    public async void StartDialogue(DialogueObject dialogueObject)
    {
        // Transition to dialogue scene and await the loading operation
        if (gameState == GameState.NpcSelect)
        {
            await sc.TransitionScene(
                SceneController.SceneName.NPCSelectScene,
                SceneController.SceneName.DialogueScene,
                SceneController.TransitionType.Transition);
        }
        else if (gameState == GameState.NpcDialogue)
        {
            await sc.TransitionScene(
                SceneController.SceneName.DialogueScene,
                SceneController.SceneName.DialogueScene,
                SceneController.TransitionType.Transition);
        }

        gameState = GameState.HintDialogue;
        // The gameevent here should pass the information to Dialoguemanager
        // ..at which point dialoguemanager will start.
        onDialogueStart.Raise(this, dialogueObject);
    }

    /// <summary>
    /// Can be called to start Dialogue with a specific character, taking a CharacterInstance as parameter.
    /// This toggles-off the NPCSelectScene,
    /// 
    /// and switches the dialogueRecipient-variable to the characterInstance that is passed as a parameter.
    /// Then, it loads the DialogueScene.
    /// </summary>
    /// <param name="character"></param>
    ///  TODO: Should use the id of a character instead of the CharacterInstance.
    public async void StartDialogue(CharacterInstance character)
    {
        GameObject[] background = CreateDialogueBackground(character, story.dialogueBackground);

        var dialogueObject = new SpeakingObject(
            character.GetGreeting(),
            background);
        
        dialogueObject.Responses.Add(new QuestionObject(background));

        // Until DialogueManager gets its information, it shouldnt do anything there.
        var dialogueRecipient = character;

        // Transition to dialogue scene and await the loading operation
        await sc.TransitionScene(
            SceneController.SceneName.NPCSelectScene,
            SceneController.SceneName.DialogueScene,
            SceneController.TransitionType.Transition);

        gameState = GameState.NpcDialogue;

        // The gameevent here should pass the information to Dialoguemanager
        // ..at which point dialoguemanager will start.
        onDialogueStart.Raise(this, dialogueObject, dialogueRecipient);
    }

    private GameObject[] CreateDialogueBackground(CharacterInstance character = null, GameObject background = null)
    {
        List<GameObject> background_ = new();
        // If the passed background is null, we use 'dialogueBackground' as the default. Otherwise, we use the passed one.
        background_.Add(background == null ? story.dialogueBackground : background);

        if (character != null)
        {
            avatarPrefab.GetComponent<SpriteRenderer>().sprite = character.avatar;
            background_.Add(avatarPrefab);
        }

        return background_.ToArray();
    }
    
    /// <summary>
    /// Called by DialogueManager when dialogue is ended, by execution of a TerminateDialogueObject.
    /// Checks if questions are remaining:
    /// .. if no, end cycle.
    /// .. if yes, 'back to NPCSelect'-button was clicked, so don't end cycle.
    /// </summary>
    public async void EndDialogue(Component sender, params object[] data)
    {
        // TODO: Refactor this.
        // If we are in the epilogue and we terminate, load either the Win or GameOver scene.
        if (gameState == GameState.Epilogue)
        {
            // If we want to start a dialogue with a different person, and do not want to end
            // the epilogue scene, the responses list should be non-empty.
            DialogueObject currentObject = (DialogueObject)data[0];
            CharacterInstance culprit = GetCulprit();
            
            // change the character of the dialogue.
            // TODO: misschien moet het veranderd worden dat de background vastgebonden zit aan de character.
            DialogueManager dm = (DialogueManager)sender;
            var backgroundCulprit = CreateDialogueBackground(culprit, story.epilogueBackground);
            dm.ReplaceBackground(backgroundCulprit);
            // If the TerminateDialogueObject has a SpeakingObject in the Responses list, start dialogue with a different person.
            if (currentObject.Responses.Count > 0)
            {
                // Transition to dialogue with a different person.
                await sc.TransitionScene(
                    SceneController.SceneName.DialogueScene,
                    SceneController.SceneName.DialogueScene,
                    SceneController.TransitionType.Transition);
                
                // If we want to start dialogue with a different person in the epilogue,
                // there will be a SpeakingObject under the Responses list of the TerminateDialogueObject,
                // which will be used for the dialogue for dialogue with the next person.
                
                onDialogueStart.Raise(this, currentObject.Responses[0], culprit);
            }
            else
            {
                if (hasWon)
                {
                    // Transition to the GameWinScene and set the gameState to GameWon.
                    await SceneController.sc.TransitionScene(
                        SceneController.SceneName.DialogueScene,
                        SceneController.SceneName.GameWinScene,
                        SceneController.TransitionType.Transition);

                    gameState = GameState.GameWon;
                }
                else
                {
                    // Transition to the GameOverScene and set the gameState to GameLoss.
                    await SceneController.sc.TransitionScene(
                        SceneController.SceneName.DialogueScene,
                        SceneController.SceneName.GameOverScene,
                        SceneController.TransitionType.Transition);
                
                    gameState = GameState.GameLoss;
                }
            }
        }
        else
        {
            if (!HasQuestionsLeft())
            {
                // No questions left, so we end the cycle 
                EndCycle();
            }
            else
            {
                // We can still ask questions, so toggle back to NPCSelectMenu without ending the cycle.
                if (gameState == GameState.GameLoss)
                {
                    Debug.Log("transition from game loss to npcselect");
                    await sc.TransitionScene(
                        SceneController.SceneName.GameOverScene, 
                        SceneController.SceneName.NPCSelectScene, 
                        SceneController.TransitionType.Transition);
                }
                else
                {
                        await sc.TransitionScene(
                            SceneController.SceneName.DialogueScene, 
                            SceneController.SceneName.NPCSelectScene, 
                            SceneController.TransitionType.Transition);
                    }
            
                gameState = GameState.NpcSelect;
            }
        }
    }
    
    /// <summary>
    /// Used to start dialogue in the epilogue scene (talking to the person chosen as the final choice).
    /// </summary>
    /// <param name="character"> The character which has been chosen. </param>
    public async void StartEpilogueDialogue(CharacterInstance character)
    {
        gameState = GameState.Epilogue;

        // Get the epilogue dialogue.
        remainingDialogueScenario = character.GetEpilogueDialogue(hasWon);

        // Create the DialogueObject and corresponding children.
        // This background displays the suspected culprit over the Dialogue-background
        var background = CreateDialogueBackground(character, story.dialogueBackground);
        var dialogueObject = GetEpilogueStart(background);
        
        // Transition to the dialogue scene.
        await SceneController.sc.TransitionScene(
            SceneController.SceneName.NPCSelectScene,
            SceneController.SceneName.DialogueScene,
            SceneController.TransitionType.Transition);
        
        onDialogueStart.Raise(this, dialogueObject, character);
    }

    /// <summary>
    /// Method which returns the DialogueObjects that need to be used at the start of the epilogue.
    /// </summary>
    /// <returns></returns>
    DialogueObject GetEpilogueStart(GameObject[] background)
    {
        var dialogueObject = new SpeakingObject(remainingDialogueScenario[0], background);
        // Remove the first element of the list.
        remainingDialogueScenario.RemoveAt(0);
        if (!hasWon)
        {
            // If the player loses, the dialogue with the wrong person should end,
            // and a new dialogue with the culprit should start.
            // note: SpeakingObject gets again gets the list at index 0, since the previous
            // dialogue at index 0 gets removed at line 490.
            TerminateDialogueObject endDialogue = new TerminateDialogueObject();
            dialogueObject.Responses.Add(endDialogue);
            
            SpeakingObject nextDialogue = new SpeakingObject(remainingDialogueScenario[0], background);
            // Remove the first element of the list.
            remainingDialogueScenario.RemoveAt(0);
            endDialogue.Responses.Add(nextDialogue);
            
            nextDialogue.Responses.Add(new OpenResponseObject(background));
        }
        else
        {
            dialogueObject.Responses.Add(new OpenResponseObject(background));
        }
        return dialogueObject;
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
        return numberOfActiveCharacters > story.minimumRemaining;
    }
    
    /// <summary>
    /// Checks if the player can ask more questions this cycle.
    /// </summary>
    /// <returns>True if player can ask more questions, otherwise false.</returns>
    public bool HasQuestionsLeft()
    {
        return numQuestionsAsked < story.numQuestions;
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

        
        //dialogueRecipient = currentCharacters[id];
        SceneManager.LoadScene("DialogueScene", LoadSceneMode.Additive);
    }    
    
    #endregion
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = System.Random;

public class GameManager : MonoBehaviour
{
    private bool notebookOn = false;

    public static GameManager gm;
    
    [SerializeField] public int numberOfCharacters;
    [SerializeField] private List<CharacterData> characters;
    
    /// <summary>
    /// The current "active" characters, any characters that became inactive should be removed from this list.
    /// </summary>
    public List<CharacterInstance> currentCharacters;

    public CharacterInstance dialogueRecipient;
    
    //random variable is made global so it can be reused
    public Random random = new Random();
    /// <summary>
    /// The amount of times  the player has talked, should be 0 at the start of each cycle
    /// </summary>
    [NonSerialized] public int numTalked;

    /// <summary>
    /// Amount of times the player can ask a question
    /// </summary>
    [SerializeField] private int numQuestions;

    [SerializeField] public int minimumRemaining;
    
    private void Awake()
    {
        gm = this;
        // Makes this GameManager persistent throughout the scenes.
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        // Initialize an empty list of characters
        currentCharacters = new List<CharacterInstance>();
        // Now, populate this list.
        PopulateCharacters();
        // Prints to console the characters that were selected to be in the current game. UNCOMMENT WHILE DEBUGGING
        Test_CharactersInGame();
        // On load start cycles.
        StartCycle();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            UnloadDialogueScene();
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            ToggleGameOverScene();
        }
    }

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

    /// <summary>
    /// Returns a random (non-culprit and active) character, used to give hints for the companion
    /// Assumes there is only 1 culprit
    /// </summary>
    public CharacterInstance GetRandomVictimNoCulprit()
    {
        List<CharacterInstance> possibleVictims = currentCharacters.FindAll(c => !c.isCulprit && c.isActive); 
        return possibleVictims[random.Next(possibleVictims.Count- 1)];
    }

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
    
    /// <summary>
    /// The main cycle of the game.
    /// This should loop everytime the player speaks to an NPC until a certain number of NPCs have been spoken to,
    /// at that point the cycle ends and the player has to choose which NPC they think is the culprit
    /// </summary>
    private void StartCycle()
    {
        Debug.Log("New cycle started.");
        // Choose a victim, make them inactive, and print the hints to the console.
        ChooseVictim();
        // Reset number of times the player has talked
        numTalked = 0;
        // Start the NPC Selection scene
        ToggleNPCSelectScene();
    }

    public void EndCycle() 
    {
        UnloadDialogueScene(); // stop dialogue immediately.

        if (currentCharacters.Count(c=>c.isActive) > minimumRemaining)
            StartCycle();
        else 
        {
            // TODO: Select culprit to end game
            ToggleGameOverScene();
        }
    }

    /// <summary>
    /// Chooses a victim, changes the isActive bool to 'false' and randomly selects a trait from both the culprit and
    /// the victim that is removed from their list of questions and prints to to the debuglog
    /// </summary>
    private void ChooseVictim()
    {
        CharacterInstance culprit = GetCulprit();
        CharacterInstance victim = GetRandomVictimNoCulprit();

        // Select a random trait and remove it from the list of available questions
        List<string> randTraitCulprit = culprit.GetRandomTrait();
        List<string> randTraitVictim = victim.GetRandomTrait();

        // Victim put on inactive so we cant ask them questions
        victim.isActive = false;
        
        //TODO: wait until I have a dialogue box to put this in
        //Debug.Log(string.Join(", ", randTraitCulprit)); 
        //Debug.Log(string.Join(", ", randTraitVictim));
    }

    /// <summary>
    /// Sends the player to the appropriate scene depending on the ammount of NPCs they have talked to this cycle
    /// </summary>
    /*private void TalkOrEnd()
    {
        if (HasQuestionsLeft()) // Placeholder value, not decided how many NPCs the player can talk to in one cycle
        {
            //SceneManager.LoadScene("NPCSelectScene", LoadSceneMode.Additive);
            ToggleNPCSelectScene();
        }
        else
        {
            // End cycle.
            // Remove 1 character
            // Then, if there are less than X remaining (after the hint-sequence), choose culprit then end
        }
    }*/

    public bool HasQuestionsLeft()
    {
        Debug.Log("Has questions left: " + (numTalked < numQuestions));
        return numTalked < numQuestions;
    }
    
    /// <summary>
    /// Counts how many characters have been talked to this cycle.
    /// </summary>
    /*private void CharactersTalkedTo()
    {
        numTalked = 0;
        for (int i = 0; i < currentCharacters.Count; i++)
        {
            if (currentCharacters[i].isActive && !currentCharacters[i].TalkedTo)
            {
                numTalked++;
            }
        }
    }*/

    public void UnloadDialogueScene()
    {
        string sceneName = "DialogueScene";
        if (SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            SceneManager.UnloadSceneAsync(sceneName);
        }
        else
        {
            Debug.Log("Dialogue scene not loaded");
        }
    }
    
    public void ToggleCompanionHintScene()
    {
        string sceneName = "Companion Hint";
        if (SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            SceneManager.UnloadSceneAsync(sceneName);
        }
        else
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        }
    }

    public void ToggleNPCSelectScene()
    {
        string sceneName = "NPCSelectScene";
        if (SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            SceneManager.UnloadSceneAsync((sceneName));
        }
        else
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        }
    }

    public void StartDialogue(CharacterInstance character)
    {
        ToggleNPCSelectScene();

        dialogueRecipient = character;
        SceneManager.LoadScene("DialogueScene", LoadSceneMode.Additive);
    }

    // Is deze nog nodig? (staat hierboven ook)
    public void StartDialogue(int id)
    {
        ToggleNPCSelectScene(); // NPC selected, so unload
        
        dialogueRecipient = currentCharacters[id];
        SceneManager.LoadScene("DialogueScene", LoadSceneMode.Additive);
    }
    
    public void ToggleGameWinScene()
    {
        if (SceneManager.GetSceneByName("GameWinScene").isLoaded)
        {
            SceneManager.UnloadSceneAsync("GameWinScene");
        }
        else
        {
            SceneManager.LoadScene("GameWinScene", LoadSceneMode.Additive);
        }
    }
    
    public void ToggleGameOverScene()
    {
        if (SceneManager.GetSceneByName("GameOverScene").isLoaded)
        {
            SceneManager.UnloadSceneAsync("GameOverScene");
        }
        else
        {
            SceneManager.LoadScene("GameOverScene", LoadSceneMode.Additive);
        }
    }
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
        UnloadAdditiveScenes();
        // Reset these characters
        foreach (CharacterInstance character in currentCharacters)
        {
            // Reset the questions and active-status of this character
            character.isActive = true;
            character.InitializeQuestions();
        }
        //Test_CharactersInGame();
        StartCycle();
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
        UnloadAdditiveScenes();
        // reset game
        Start();
        
    }

    private void UnloadAdditiveScenes()
    {
        //Get the story scene
        Scene storyScene = SceneManager.GetSceneByName("StoryScene");

        // Unload all loaded scenes that are not the story scene
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene loadedScene = SceneManager.GetSceneAt(i);
            if (loadedScene != storyScene) SceneManager.UnloadSceneAsync(loadedScene.name);
        }
    }

    public void ToggleNotebookScene()
    {
        if (notebookOn)
        {

            SceneManager.UnloadSceneAsync("NotebookScene");
            notebookOn = false;
        }
        else
        {
            SceneManager.LoadScene("NotebookScene", LoadSceneMode.Additive);
            notebookOn= true;
        }
    }
}

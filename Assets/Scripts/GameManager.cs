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
    [SerializeField] public int numberOfCharacters;
    [SerializeField] private List<Character> characters;
    
    /// <summary>
    /// The current "active" characters, any characters that became inactive should be removed from this list.
    /// </summary>
    private List<Character> currentCharacters;
    
    //random variable is made global so it can be reused
    public Random random = new Random();
    /// <summary>
    /// The amount of characters the player has talked to, should be 0 at the start of each cycle
    /// </summary>
    private int numTalkedTo;
    /// <summary>
    /// If the player has already received hints from the assistant, should be false at the start of every cycle
    /// </summary>
    private bool hintsDone; 


    // Start is called before the first frame update
    void Start()
    {
        // Makes this GameManager persistent throughout the scenes.
        DontDestroyOnLoad(this.gameObject);

        // Initialize an empty list of characters
        currentCharacters = new List<CharacterData>();
        // Now, populate this list.
        PopulateCharacters();
        // Prints to console the characters that were selected to be in the current game. UNCOMMENT WHILE DEBUGGING
        //Test_CharactersInGame();

        //LoadDialogueScene();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ToggleDialogueScene();
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
                int index = random.Next(0, numberOfCharacters + 1) + 1; // offset by 1 to check existence

                string arrayString = "";
                for (int j = 0; j< visitedIndices.Length; j++)
                    arrayString += (visitedIndices[j] + ", ");
                
                //Debug.Log("Trying index: " + index + " over index-array: [" + arrayString + "]");
                if (!visitedIndices.Contains(index))
                {
                    //Debug.Log("Unique index found!");
                    var toAdd = characters[index - 1]; // correct the offset
                    currentCharacters.Add(toAdd); // add the character we found to the list of current characters
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
    public Character GetCulprit() => currentCharacters.Find(c => c.isCulprit);

    /// <summary>
    /// Returns a random (non-culprit) character, used to give hints for the companion
    /// Assumes currentCharacters only contains active characters
    /// Assumes there is only 1 culprit
    /// </summary>
    public Character GetRandomCharacterNoCulprit() =>
        currentCharacters.FindAll(c => !c.isCulprit)[random.Next(currentCharacters.Count - 1)];

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
    private void Cycle()
    {
        // Once at the start of the cycle have the assistant give hints
        if (!hintsDone)
        {
            //make a character "disappear"
            Character theUnluckyOne = GetRandomCharacterNoCulprit();
            currentCharacters = currentCharacters.FindAll(c => c.id != theUnluckyOne.id).ToList();
            theUnluckyOne.isActive = false;
            
            ToggleCompanionHintScene();
            hintsDone = true;
        }
        CharactersTalkedTo();
        TalkorEnd();
    }

    /// <summary>
    /// Sends the player to the appropriate scene depending on the ammount of NPCs they have talked to this cycle
    /// </summary>
    private void TalkorEnd()
    {
        if (numTalkedTo < 5) // Placeholder value, not decided how many NPCs the player can talk to in one cycle
        {
            SceneManager.LoadScene("NPCSelectScene", LoadSceneMode.Additive);
        }
        else
        {
            // Load the scene where the player chooses the culprit
            // This scene does not exist yet
        }
    }
    /// <summary>
    /// Counts how many characters have been talked to this cycle.
    /// </summary>
    private void CharactersTalkedTo()
    {
        numTalkedTo = 0;
        for (int i = 0; i < currentCharacters.Count; i++)
        {
            if (currentCharacters[i].isActive && !currentCharacters[i].TalkedTo)
            {
                numTalkedTo++;
            }
        }
    }

    public void ToggleDialogueScene()
    {
        string sceneName = "Dialogue Test";
        if (SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            SceneManager.UnloadSceneAsync(sceneName);
        }
        else
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
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
}

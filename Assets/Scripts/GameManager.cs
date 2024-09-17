using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = System.Random;

public class GameManager : MonoBehaviour
{
    [SerializeField] public int numberOfCharacters;
    [SerializeField] private List<CharacterData> characters;
    private List<CharacterData> currentCharacters;


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
        Random r = new Random();

        // We iterate over a for-loop to find a specific number of characters to populate our game with.
        // We clamp it down to the smallest value, in case numberOfCharacters is more than the number we have generated.
        numberOfCharacters = Math.Min(characters.Count, numberOfCharacters);
        for (int i = 0; i < numberOfCharacters; i++)
        {
            bool foundUniqueInt = false; // We use this bool to exist the while-loop when we find a unique index
            while (!foundUniqueInt)
            {
                int index = r.Next(0, numberOfCharacters + 1) + 1; // offset by 1 to check existence

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
        currentCharacters[r.Next(0, numberOfCharacters)].isCulprit = true;
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

    public void ToggleDialogueScene()
    {
        if (SceneManager.GetSceneByName("Dialogue Test").isLoaded)
        {
            SceneManager.UnloadSceneAsync("Dialogue Test");
        }
        else
        {
            SceneManager.LoadScene("Dialogue Test", LoadSceneMode.Additive);
        }
    }
}

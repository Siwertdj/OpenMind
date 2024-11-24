using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using UnityEditor;
using Scene = UnityEngine.SceneManagement.Scene;

public class SystemTests
{
    [UnitySetUp]
    public IEnumerator Setup()
    {
        SceneManager.LoadScene("StartScreenScene");
        yield return new WaitUntil(() => SceneManager.GetSceneByName("StartScreenScene").isLoaded);
        yield return null;
    }

    [UnityTest]
    public IEnumerator PlayTheGame()
    {
        // Find the New Game button and click it
        GameObject.Find("NewGameButton").GetComponent<Button>().onClick.Invoke();

        // Choose to view the prologue
        GameObject.Find("YesButton").GetComponent<Button>().onClick.Invoke();
        yield return new WaitUntil(() => SceneManager.GetSceneByName("PrologueScene").isLoaded);

        // Check if we are in the prologue
        Assert.AreEqual(SceneManager.GetSceneByName("PrologueScene"), SceneManager.GetActiveScene());

        // Play the prologue
        while (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("PrologueScene"))
        {
            // Wait a second, otherwise the test crashes
            yield return new WaitForSeconds(1);
            
            // Find button (if it is active, and click it to proceed)
            if (GameObject.Find("Button parent") != null)
                GameObject.Find("Button parent").GetComponent<Button>().onClick.Invoke();
        }
        
        yield return new WaitUntil(() => SceneManager.GetSceneByName("StorySelectScene").isLoaded);
        
        // Check if we are in the StorySelect scene
        Assert.AreEqual(SceneManager.GetSceneByName("StorySelectScene"), SceneManager.GetActiveScene());
        
        // Select story
        GameObject.Find("StoryA_Button").GetComponent<Button>().onClick.Invoke();
        yield return new WaitUntil(() => SceneManager.GetSceneByName("IntroStoryScene").isLoaded);
        
        // Check if we are in the story intro
        Assert.AreEqual(SceneManager.GetSceneByName("IntroStoryScene"), SceneManager.GetActiveScene());

        // Play the story intro
        while (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("IntroStoryScene"))
        {
            // Wait a second, otherwise the test crashes
            yield return new WaitForSeconds(1);
            
            // Find button (if it is active, and click it to proceed)
            if (GameObject.Find("Button parent") != null)
                GameObject.Find("Button parent").GetComponent<Button>().onClick.Invoke();
        }

        // Number of characters in the game
        int numCharacters = GameManager.gm.story.numberOfCharacters;
        
        // Number of characters that are left when you have to choose the culprit
        int charactersLeft = GameManager.gm.story.minimumRemaining;

        // Play the main loop of the game
        for (int i = 0; i <= (numCharacters - charactersLeft); i++)
        {
            yield return new WaitUntil(() => SceneManager.GetSceneByName("NPCSelectScene").isLoaded);

            // Check if we are in the NPC Select scene
            Assert.AreEqual(SceneManager.GetSceneByName("NPCSelectScene"), SceneManager.GetSceneAt(1));
            
            // Use Notebook if not the first cycle
            if (i > 0)
            {
                // Open Notebook
                GameObject.Find("NotebookButton").GetComponent<Button>().onClick.Invoke();

                // Wait until loaded
                yield return new WaitUntil(() =>
                    SceneManager.GetSceneByName("NotebookScene").isLoaded);
                
                // Check if we are in the Notebook scene
                Assert.AreEqual(SceneManager.GetSceneByName("NotebookScene"), SceneManager.GetSceneAt(2));
                
                // Open character tab
                GameObject.Find("NameButton1").GetComponent<Button>().onClick.Invoke();
                
                // Log notes
                GameObject.Find("Button").GetComponent<Button>().onClick.Invoke();
                
                // Open personal notes
                GameObject.Find("PersonalButton").GetComponent<Button>().onClick.Invoke();
                
                // Close notebook
                GameObject.Find("NotebookButton").GetComponent<Button>().onClick.Invoke();
                yield return new WaitUntil(() =>
                    SceneManager.GetSceneByName("NPCSelectScene").isLoaded);
                
                // Check if we are back in the NPC Select scene
                Assert.AreEqual(SceneManager.GetSceneByName("NPCSelectScene"), SceneManager.GetSceneAt(1));
            }

            // Find an active character and click to talk to them
            foreach (CharacterInstance c in GameManager.gm.currentCharacters)
            {
                if (c.isActive)
                {
                    string name = "characterspace " + (GameManager.gm.currentCharacters.IndexOf(c) + 1);
                    GameObject.Find(name).GetComponent<Button>().onClick.Invoke();
                    break;
                }
            }

            yield return new WaitForSeconds(1);
            yield return new WaitUntil(() => SceneManager.GetSceneByName("DialogueScene").isLoaded);

            // Check if we are in the Dialogue scene
            Assert.AreEqual(SceneManager.GetSceneByName("DialogueScene"), SceneManager.GetSceneAt(1));

            yield return new WaitForSeconds(1);
            
            // Wait until you can ask a question
            GameObject.Find("Skip Dialogue Button").GetComponent<Button>().onClick.Invoke();

            // Ask a question
            GameObject.Find("questionButton").GetComponent<Button>().onClick.Invoke();

            // Skip dialogue until new cycle starts
            while (SceneManager.GetSceneAt(1) != SceneManager.GetSceneByName("NPCSelectScene"))
            {
                yield return new WaitForSeconds(1);
                
                if (GameObject.Find("Skip Dialogue Button") != null)
                    GameObject.Find("Skip Dialogue Button").GetComponent<Button>().onClick.Invoke();
            }
        }

        // Check if we have to choose a culprit
        Assert.AreEqual(SceneManager.GetSceneByName("NPCSelectScene"), SceneManager.GetSceneAt(1));
        
        // Find an active character and click to choose them
        foreach (CharacterInstance c in GameManager.gm.currentCharacters)
        {
            if (c.isActive)
            {
                string name = "characterspace " + (GameManager.gm.currentCharacters.IndexOf(c) + 1);
                GameObject.Find(name).GetComponent<Button>().onClick.Invoke();
                break;
            }
        }

        // Check if we are in the Dialogue scene
        yield return new WaitUntil(() => SceneManager.GetSceneByName("DialogueScene").isLoaded);
        Assert.AreEqual(SceneManager.GetSceneByName("DialogueScene"), SceneManager.GetSceneAt(1));
        
        // Skip dialogue until first reflection moment
        while (GameObject.Find("Input Field") == null)
        {
            yield return new WaitForSeconds(1);
                
            if (GameObject.Find("Skip Dialogue Button") != null)
                GameObject.Find("Skip Dialogue Button").GetComponent<Button>().onClick.Invoke();
        }
        
        // Click on submit input button
        yield return new WaitForSeconds(1);
        GameObject.Find("Submit input Button").GetComponent<Button>().onClick.Invoke();
        yield return new WaitForSeconds(1);
        
        // Skip dialogue until second reflection moment
        while (GameObject.Find("Input Field") == null)
        {
            yield return new WaitForSeconds(1);
                
            if (GameObject.Find("Skip Dialogue Button") != null)
                GameObject.Find("Skip Dialogue Button").GetComponent<Button>().onClick.Invoke();
        }
        
        // Click on submit input button
        yield return new WaitForSeconds(1);
        GameObject.Find("Submit input Button").GetComponent<Button>().onClick.Invoke();
        yield return new WaitForSeconds(1);
        
        // Skip dialogue until GameOver or GameWin
        while (SceneManager.GetSceneAt(1) != SceneManager.GetSceneByName("GameOverScene") && SceneManager.GetSceneAt(1) != SceneManager.GetSceneByName("GameWinScene"))
        {
            yield return new WaitForSeconds(1);
                
            if (GameObject.Find("Skip Dialogue Button") != null)
                GameObject.Find("Skip Dialogue Button").GetComponent<Button>().onClick.Invoke();
        }

        // Check if we are in the GameWin or GameOver scene
        yield return new WaitForSeconds(3);
        var gameOver = SceneManager.GetSceneAt(1) == SceneManager.GetSceneByName("GameOverScene");
        var gameWon = SceneManager.GetSceneAt(1) == SceneManager.GetSceneByName("GameWinScene");
        Assert.IsTrue(gameOver || gameWon);
    }

    [UnityTest]
    public IEnumerator SaveGame()
    {
        // Find the New Game button and click it
        GameObject.Find("NewGameButton").GetComponent<Button>().onClick.Invoke();

        // Choose to view the prologue
        GameObject.Find("YesButton").GetComponent<Button>().onClick.Invoke();
        yield return new WaitUntil(() => SceneManager.GetSceneByName("PrologueScene").isLoaded);

        // Check if we are in the prologue
        Assert.AreEqual(SceneManager.GetSceneByName("PrologueScene"), SceneManager.GetActiveScene());

        // Play the prologue
        while (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("PrologueScene"))
        {
            // Wait a second, otherwise the test crashes
            yield return new WaitForSeconds(1);
            
            // Find button (if it is active, and click it to proceed)
            if (GameObject.Find("Button parent") != null)
                GameObject.Find("Button parent").GetComponent<Button>().onClick.Invoke();
        }
        
        yield return new WaitUntil(() => SceneManager.GetSceneByName("StorySelectScene").isLoaded);
        
        // Check if we are in the StorySelect scene
        Assert.AreEqual(SceneManager.GetSceneByName("StorySelectScene"), SceneManager.GetActiveScene());
        
        // Select story
        GameObject.Find("StoryA_Button").GetComponent<Button>().onClick.Invoke();
        yield return new WaitUntil(() => SceneManager.GetSceneByName("IntroStoryScene").isLoaded);
        
        // Check if we are in the story intro
        Assert.AreEqual(SceneManager.GetSceneByName("IntroStoryScene"), SceneManager.GetActiveScene());

        // Play the story intro
        while (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("IntroStoryScene"))
        {
            // Wait a second, otherwise the test crashes
            yield return new WaitForSeconds(1);
            
            // Find button (if it is active, and click it to proceed)
            if (GameObject.Find("Button parent") != null)
                GameObject.Find("Button parent").GetComponent<Button>().onClick.Invoke();
        }
        
        yield return new WaitUntil(() => SceneManager.GetSceneByName("NPCSelectScene").isLoaded);

        // Check if we are in the NPC Select scene
        Assert.AreEqual(SceneManager.GetSceneByName("NPCSelectScene"), SceneManager.GetSceneAt(1));
        
        // Find an active character and click to talk to them
        foreach (CharacterInstance c in GameManager.gm.currentCharacters)
        {
            if (c.isActive)
            {
                GameObject.Find("Confirm Selection Button").GetComponent<Button>().onClick.Invoke();
                break;
            }
        }

        yield return new WaitForSeconds(1);
        yield return new WaitUntil(() => SceneManager.GetSceneByName("DialogueScene").isLoaded);

        // Check if we are in the Dialogue scene
        // TODO: check for isloaded.
        Assert.AreEqual(SceneManager.GetSceneByName("DialogueScene"), SceneManager.GetSceneAt(1));

        yield return new WaitForSeconds(1);
            
        // Wait until you can ask a question
        GameObject.Find("Skip Dialogue Button").GetComponent<Button>().onClick.Invoke();

        // Ask a question
        GameObject.Find("questionButton").GetComponent<Button>().onClick.Invoke();

        // Skip dialogue until new cycle starts
        while (SceneManager.GetSceneAt(1) != SceneManager.GetSceneByName("NPCSelectScene"))
        {
            yield return new WaitForSeconds(1);
                
            if (GameObject.Find("Skip Dialogue Button") != null)
                GameObject.Find("Skip Dialogue Button").GetComponent<Button>().onClick.Invoke();
        }
        
        // Open Notebook
        GameObject.Find("NotebookButton").GetComponent<Button>().onClick.Invoke();

        // Wait until loaded
        yield return new WaitUntil(() =>
            SceneManager.GetSceneByName("NotebookScene").isLoaded);
                
        // Check if we are in the Notebook scene
        Assert.AreEqual(SceneManager.GetSceneByName("NotebookScene"), SceneManager.GetSceneAt(2));
                
        // Open character tab
        GameObject.Find("NameButton1").GetComponent<Button>().onClick.Invoke();
                
        // Log notes
        GameObject.Find("Button").GetComponent<Button>().onClick.Invoke();
                
        // Open personal notes
        GameObject.Find("PersonalButton").GetComponent<Button>().onClick.Invoke();
        
        // Write down notes
        string notebookTextPrior = "hello";
        var notebookManager = GameObject.Find("NotebookManager").GetComponent<NotebookManager>();
        notebookManager.inputField.GetComponent<TMP_InputField>().text = notebookTextPrior;
        notebookManager.inputField.GetComponent<TMP_InputField>().onEndEdit.Invoke(notebookTextPrior);
        
        // Close notebook
        GameObject.Find("NotebookButton").GetComponent<Button>().onClick.Invoke();
        yield return new WaitUntil(() =>
            SceneManager.GetSceneByName("NPCSelectScene").isLoaded);
        
        // Check if we are back in the NPC Select scene
        Assert.AreEqual(SceneManager.GetSceneByName("NPCSelectScene"), SceneManager.GetSceneAt(1));
        
        // Open the menu
        GameObject.Find("MenuButton").GetComponent<Button>().onClick.Invoke();
        
        // Wait until loaded
        yield return new WaitUntil(() =>
            SceneManager.GetSceneByName("GameMenuScene").isLoaded);

        yield return new WaitForSeconds(1);
        
        // Save the game
        GameObject.Find("SaveButton").GetComponent<Button>().onClick.Invoke();
        
        // Wait for the data to be saved
        yield return new WaitForSeconds(3);
        
        bool saveFileExists = FilePathConstants.DoesSaveFileLocationExist();
        Assert.IsTrue(saveFileExists);
        
        // Retrieve the data from the save file
        SaveData saveData = Load.Loader.GetSaveData();
        
        // Check if the following variables are equal to the saved data
        var gm = GameManager.gm;
        
        // Check if storyID is equal
        Assert.AreEqual(gm.story.storyID, saveData.storyId);
        
        // Check if the activeCharacterIds are equal by checking the following 2 properties:
        // 1: Check if the array of activeCharacterIds has the same length as the array of
        // activeCharacterIds from the saveData.
        // 2: Check if both arrays contain the same elements.
        int[] activeCharIdArray = gm.currentCharacters.FindAll(c => c.isActive).ToArray().Select(c => c.id).ToArray();
        // Check if the arrays have the same length
        Assert.AreEqual(activeCharIdArray.Length, saveData.activeCharacterIds.Length);
        // Check if both arrays contain the same elements
        Assert.IsTrue(activeCharIdArray.All(saveData.activeCharacterIds.Contains));
        
        // Check if the inactiveCharacterIds are equal by checking the following 2 properties:
        // 1: Check if the array of inactiveCharacterIds has the same length as the array of
        // inactiveCharacterIds from the saveData.
        // 2: Check if both arrays contain the same elements.
        int[] inactiveCharIdArray = gm.currentCharacters.FindAll(c => !c.isActive).ToArray().Select(c => c.id).ToArray();
        // Check if the arrays have the same length
        Assert.AreEqual(inactiveCharIdArray.Length, saveData.inactiveCharacterIds.Length);
        // Check if both arrays contain the same elements
        Assert.IsTrue(inactiveCharIdArray.All(saveData.inactiveCharacterIds.Contains));
        
        // Check if the culpritId is equal
        Assert.AreEqual(gm.GetCulprit().id, saveData.culpritId);
        
        // Check if the remainingQuestions are equal by checking the following 2 properties:
        // 1: Check if the array of remainingQuestions has the same length as the array of
        // remainingQuestions from the saveData.
        // 2: Check if both arrays contain the same elements.
        (int, List<Question>)[] remainingQuestionsArray = gm.currentCharacters.Select(a => (a.id, a.RemainingQuestions)).ToArray();
        // Check if the arrays have the same length
        Assert.AreEqual(remainingQuestionsArray.Length, saveData.remainingQuestions.Length);
        // Check if both arrays contain the same elements
        for (int i = 0; i < remainingQuestionsArray.Length; i++)
        {
            // Check if the first elements of the pairs are equal
            Assert.AreEqual(remainingQuestionsArray[i].Item1, saveData.remainingQuestions[i].Item1);
            // Check if the second elements of the pairs (question list) are equal
            Assert.IsTrue(remainingQuestionsArray[i].Item2.All(saveData.remainingQuestions[i].Item2.Contains));
        }
        
        // Check if the personalNotes are equal
        Assert.AreEqual(gm.notebookData.GetPersonalNotes(), saveData.personalNotes);
        
        // Check if the characterNotes are equal by checking the following 2 properties:
        // 1: Check if the array of characterNotes has the same length as the array of
        // characterNotes from the saveData.
        // 2: Check if both arrays contain the same elements.
        (int, string)[] characterNotes = gm.currentCharacters
            .Select(c => (c.id, gm.notebookData.GetCharacterNotes(c))).ToArray();
        // Check if the arrays have the same length
        Assert.AreEqual(characterNotes.Length, saveData.characterNotes.Length);
        // Check if both arrays contain the same elements
        for (int i = 0; i < characterNotes.Length; i++)
        {
            // Check if the first elements of the pairs are equal
            Assert.AreEqual(characterNotes[i].Item1, saveData.characterNotes[i].Item1);
            // Check if the second elements of the pairs are equal
            Assert.AreEqual(characterNotes[i].Item2, saveData.characterNotes[i].Item2);
        }
        
        // Check if the askedQuestionsPerCharacter are equal by checking the following 2 properties:
        // 1: Check if the array of askedQuestionsPerCharacter has the same length as the array of
        // askedQuestionsPerCharacter from the saveData.
        // 2: Check if both arrays contain the same elements.
        (int, List<Question>)[] askedQuestionsPerCharArray =
            gm.currentCharacters.Select(a => (a.id, a.AskedQuestions)).ToArray();
        // Check if the arrays have the same length
        Assert.AreEqual(askedQuestionsPerCharArray.Length, saveData.askedQuestionsPerCharacter.Length);
        // Check if both arrays contain the same elements
        for (int i = 0; i < askedQuestionsPerCharArray.Length; i++)
        {
            // Check if the first elements of the pairs are equal
            Assert.AreEqual(askedQuestionsPerCharArray[i].Item1, saveData.askedQuestionsPerCharacter[i].Item1);
            // Check if the second elements of the pairs are equal
            Assert.AreEqual(askedQuestionsPerCharArray[i].Item2, saveData.askedQuestionsPerCharacter[i].Item2);
        }
        
        // Check if the numQuestionsAsked is equal
        Assert.AreEqual(gm.numQuestionsAsked, saveData.numQuestionsAsked);
        
        yield return null;
    }

    [UnityTest]
    public IEnumerator LoadGame()
    {
        // TODO: check if save file exists
        
        // Find the New Game button and click it
        GameObject.Find("ContinueButton").GetComponent<Button>().onClick.Invoke();
        
        // Check if we are in the Dialogue scene
        yield return new WaitUntil(() => SceneManager.GetSceneByName("NPCSelectScene").isLoaded);
        
        // Number of characters in the game
        int numCharacters = GameManager.gm.story.numberOfCharacters;
        
        // Number of characters that are left when you have to choose the culprit
        int charactersLeft = GameManager.gm.story.minimumRemaining;

        // Play the main loop of the game
        for (int i = 0; i <= (numCharacters - charactersLeft); i++)
        {
            yield return new WaitUntil(() => SceneManager.GetSceneByName("NPCSelectScene").isLoaded);

            // Check if we are in the NPC Select scene
            Assert.AreEqual(SceneManager.GetSceneByName("NPCSelectScene"), SceneManager.GetSceneAt(1));
            
            // Use Notebook if not the first cycle
            if (i > 0)
            {
                // Open Notebook
                GameObject.Find("NotebookButton").GetComponent<Button>().onClick.Invoke();

                // Wait until loaded
                yield return new WaitUntil(() =>
                    SceneManager.GetSceneByName("NotebookScene").isLoaded);
                
                // Check if we are in the Notebook scene
                Assert.AreEqual(SceneManager.GetSceneByName("NotebookScene"), SceneManager.GetSceneAt(2));
                
                // Open character tab
                GameObject.Find("NameButton1").GetComponent<Button>().onClick.Invoke();
                
                // Log notes
                GameObject.Find("Button").GetComponent<Button>().onClick.Invoke();
                
                // Open personal notes
                GameObject.Find("PersonalButton").GetComponent<Button>().onClick.Invoke();
                
                // Close notebook
                GameObject.Find("NotebookButton").GetComponent<Button>().onClick.Invoke();
                yield return new WaitUntil(() =>
                    SceneManager.GetSceneByName("NPCSelectScene").isLoaded);
                
                // Check if we are back in the NPC Select scene
                Assert.AreEqual(SceneManager.GetSceneByName("NPCSelectScene"), SceneManager.GetSceneAt(1));
            }
            
            // Start at the leftmost character
            while (GameObject.Find("NavLeft"))
            {
                GameObject.Find("NavLeft").GetComponent<Button>().onClick.Invoke();
                yield return new WaitForSeconds(2);
            }
            
            // Find an active character and click to choose them
            foreach (CharacterInstance c in GameManager.gm.currentCharacters)
            {
                if (c.isActive)
                {
                    GameObject.Find("Confirm Selection Button").GetComponent<Button>().onClick.Invoke();
                    break;
                }
                else
                {
                    if (GameObject.Find("NavRight"))
                    {
                        GameObject.Find("NavRight").GetComponent<Button>().onClick.Invoke();
                        yield return new WaitForSeconds(2);
                    }
                    else
                    {
                        throw new Exception("There are no active characters");
                    }
                }
            }

            yield return new WaitForSeconds(1);
            yield return new WaitUntil(() => SceneManager.GetSceneByName("DialogueScene").isLoaded);

            // Check if we are in the Dialogue scene
            Assert.AreEqual(SceneManager.GetSceneByName("DialogueScene"), SceneManager.GetSceneAt(1));

            yield return new WaitForSeconds(1);
            
            // Wait until you can ask a question
            GameObject.Find("Skip Dialogue Button").GetComponent<Button>().onClick.Invoke();

            // Ask a question
            GameObject.Find("questionButton").GetComponent<Button>().onClick.Invoke();

            // Skip dialogue until new cycle starts
            while (SceneManager.GetSceneAt(1) != SceneManager.GetSceneByName("NPCSelectScene"))
            {
                yield return new WaitForSeconds(1);
                
                if (GameObject.Find("Skip Dialogue Button") != null)
                    GameObject.Find("Skip Dialogue Button").GetComponent<Button>().onClick.Invoke();
            }
        }

        // Check if we have to choose a culprit
        Assert.AreEqual(SceneManager.GetSceneByName("NPCSelectScene"), SceneManager.GetSceneAt(1));
        
        // Start at the leftmost character
        while (GameObject.Find("NavLeft"))
        {
            GameObject.Find("NavLeft").GetComponent<Button>().onClick.Invoke();
            yield return new WaitForSeconds(2);
        }
            
        // Find an active character and click to choose them
        foreach (CharacterInstance c in GameManager.gm.currentCharacters)
        {
            if (c.isActive)
            {
                GameObject.Find("Confirm Selection Button").GetComponent<Button>().onClick.Invoke();
                break;
            }
            else
            {
                if (GameObject.Find("NavRight"))
                {
                    GameObject.Find("NavRight").GetComponent<Button>().onClick.Invoke();
                    yield return new WaitForSeconds(2);
                }
                else
                {
                    throw new Exception("There are no active characters");
                }
            }
        }

        // Check if we are in the Dialogue scene
        yield return new WaitUntil(() => SceneManager.GetSceneByName("DialogueScene").isLoaded);
        Assert.AreEqual(SceneManager.GetSceneByName("DialogueScene"), SceneManager.GetSceneAt(1));
        
        // Skip dialogue until first reflection moment
        while (GameObject.Find("Input Field") == null)
        {
            yield return new WaitForSeconds(1);
                
            if (GameObject.Find("Skip Dialogue Button") != null)
                GameObject.Find("Skip Dialogue Button").GetComponent<Button>().onClick.Invoke();
        }
        
        // Click on submit input button
        yield return new WaitForSeconds(1);
        GameObject.Find("Submit input Button").GetComponent<Button>().onClick.Invoke();
        yield return new WaitForSeconds(1);
        
        // Skip dialogue until second reflection moment
        while (GameObject.Find("Input Field") == null)
        {
            yield return new WaitForSeconds(1);
                
            if (GameObject.Find("Skip Dialogue Button") != null)
                GameObject.Find("Skip Dialogue Button").GetComponent<Button>().onClick.Invoke();
        }
        
        // Click on submit input button
        yield return new WaitForSeconds(1);
        GameObject.Find("Submit input Button").GetComponent<Button>().onClick.Invoke();
        yield return new WaitForSeconds(1);
        
        // Skip dialogue until GameOver or GameWin
        while (SceneManager.GetSceneAt(1) != SceneManager.GetSceneByName("GameOverScene") && SceneManager.GetSceneAt(1) != SceneManager.GetSceneByName("GameWinScene"))
        {
            yield return new WaitForSeconds(1);
                
            if (GameObject.Find("Skip Dialogue Button") != null)
                GameObject.Find("Skip Dialogue Button").GetComponent<Button>().onClick.Invoke();
        }

        // Check if we are in the GameWin or GameOver scene
        yield return new WaitForSeconds(3);
        var gameOver = SceneManager.GetSceneAt(1) == SceneManager.GetSceneByName("GameOverScene");
        var gameWon = SceneManager.GetSceneAt(1) == SceneManager.GetSceneByName("GameWinScene");
        Assert.IsTrue(gameOver || gameWon);
        
        yield return null;
    }

    [UnityTest]
    public IEnumerator ChangeSettings()
    {
        yield return null;
    }

    [UnityTest]
    public IEnumerator RestartGame()
    {
        yield return null;
    }

    [UnityTest]
    public IEnumerator RetryGame()
    {
        yield return null;
    }
}
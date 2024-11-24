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
                string name = "characterspace " + (GameManager.gm.currentCharacters.IndexOf(c) + 1);
                GameObject.Find(name).GetComponent<Button>().onClick.Invoke();
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
        
        //notebookManager = GameObject.Find("NotebookManager").GetComponent<NotebookManager>();
        //string notebookTextPosterior = notebookManager.inputField.GetComponent<TMP_InputField>().text;
        //Debug.Log("posterior = " + notebookTextPosterior);
        
        // Check if we are back in the NPC Select scene
        Assert.AreEqual(SceneManager.GetSceneByName("NPCSelectScene"), SceneManager.GetSceneAt(1));
        
        // Open the menu
        GameObject.Find("MenuButton").GetComponent<Button>().onClick.Invoke();
        
        // Save the game
        GameObject.Find("SaveButton").GetComponent<Button>().onClick.Invoke();
        
        // Wait for the data to be saved
        //yield return new WaitForSeconds(3);

        //SaveData saveData = Loading.GetSaveData();
        
        // Load the game
        GameObject.Find("QuitButton").GetComponent<Button>().onClick.Invoke();
        
        // TODO: check the save file
        yield return null;
    }

    [UnityTest]
    public IEnumerator LoadGame()
    {
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
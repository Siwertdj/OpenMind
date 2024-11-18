using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using NUnit.Framework;
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

            // Find an active character in the scene and click to talk to them
            for (int j = 1; j <= numCharacters; j++)
            {
                string name = "characterspace " + j;
                if (GameObject.Find(name) != null)
                {
                    GameObject.Find(name).GetComponent<Button>().onClick.Invoke();
                    break;
                }
            }

            yield return new WaitForSeconds(1);
            yield return new WaitUntil(() => SceneManager.GetSceneByName("DialogueScene").isLoaded);

            // Check if we are in the Dialogue scene
            Assert.AreEqual(SceneManager.GetSceneByName("DialogueScene"), SceneManager.GetSceneAt(1));

            yield return new WaitForSeconds(5);
            
            Debug.Log("hello");
            
            // Wait until you can ask a question
            //GameObject.Find("Skip Dialogue Button").GetComponent<Button>().onClick.Invoke();
            //DialogueManager.dm.OnDialogueComplete();
            // HEEEEEEEEEEEEEEEEELP IM GOING MAD

            Debug.Log("hi");
            
            // Ask a question
            GameObject.Find("questionButton").GetComponent<Button>().onClick.Invoke();

            // Skip dialogue until new cycle starts
            while (SceneManager.GetSceneAt(1) != SceneManager.GetSceneByName("NPCSelectScene"))
                GameObject.Find("Skip Dialogue Button").GetComponent<Button>().onClick.Invoke();
        }

        // Check if we have to choose a culprit
        Assert.AreEqual(SceneManager.GetSceneByName("NPCSelectScene"), SceneManager.GetSceneAt(1));
        
        // Find an active character in the scene and click to choose them
        /*for (int j = 1; j <= numCharacters; j++)
        {
            string name = "characterspace " + j;
            if (GameObject.Find(name) != null)
            {
                GameObject.Find(name).GetComponent<Button>().onClick.Invoke();
                break;
            }
        }*/
    }

    [UnityTest]
    public IEnumerator SaveGame()
    {
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
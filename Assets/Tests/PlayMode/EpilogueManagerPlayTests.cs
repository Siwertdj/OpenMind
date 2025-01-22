// This program has been developed by students from the bachelor Computer Science at Utrecht University within the Software Project course.
// © Copyright Utrecht University (Department of Information and Computing Sciences)
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EpilogueManagerPlayTests
{
    private StoryObject     story;
    private GameManager     gm;
    private DialogueManager dm;

    /// <summary>
    /// Set up the game so that each test starts at the Epilogue with the chosen story.
    /// </summary>
    [UnitySetUp]
    public IEnumerator SetUp()
    {
        // Load StartScreenScene in order to put the SettingsManager into DDOL
        SceneManager.LoadScene("StartScreenScene");
        yield return new WaitUntil(() => SceneManager.GetSceneByName("StartScreenScene").isLoaded);

        // Unload the StartScreenScene
        SceneManager.UnloadSceneAsync("StartScreenScene");

        // Load the "Loading" scene in order to get access to the toolbox in DDOL
        SceneManager.LoadScene("Loading");
        yield return new WaitUntil(() => SceneManager.GetSceneByName("Loading").isLoaded);

        // Get a StoryObject.
        StoryObject[] stories = Resources.LoadAll<StoryObject>("Stories");
        story = stories[1];

        gm = GameObject.Find("GameManager").GetComponent<GameManager>();

        // Start the game with the chosen story.
        gm.StartGame(null, story);

        yield return new WaitUntil(() => SceneManager.GetSceneByName("NPCSelectScene").isLoaded); // Wait for scene to load.
        
        // Keep removing 1 character which is not the culprit, until there are not enough characters remaining.
        while (gm.EnoughCharactersRemaining())
        {
            // Set this bool to true once a character has been removed.
            bool removedCharacter = false;
            foreach (CharacterInstance c in gm.currentCharacters)
            {
                // Set a character to not active if it is not a culprit, is active and the bool removedCharacter is false.
                if (!c.isCulprit && c.isActive && !removedCharacter)
                {
                    c.isActive = false;
                    removedCharacter = true;
                }
            }
        }
        
        // Start the game cycle with not enough characters remaining.
        yield return new WaitUntil(() => SceneManager.GetSceneByName("NPCSelectScene").isLoaded); // Wait for scene to load.
        
        // Set this to maxValue in order to make sure that no more questions can be asked.
        gm.numQuestionsAsked = int.MaxValue;
        
        // Start dialogue with a character, then go back to NpcSelect scene in order to apply the changes of the variables.
        CharacterInstance character = gm.currentCharacters[0];
        gm.StartDialogue(character);
        yield return new WaitUntil(() => SceneManager.GetSceneByName("DialogueScene").isLoaded); // Wait for scene to load.
        
        // Waiting for the DialogueManager to appear.
        yield return new WaitUntil(() => GameObject.Find("DialogueManager") != null);
        
        // Get the DialogueManager.
        dm = GameObject.Find("DialogueManager").GetComponent<DialogueManager>();
        
        // End the dialogue.
        dm.currentObject = new TerminateDialogueObject();
        dm.currentObject.Execute();
        
        yield return new WaitUntil(() => SceneManager.GetSceneByName("EpilogueScene").isLoaded); // Wait for scene to load.
        
        // Waiting for the EpilogueManager to appear.
        yield return new WaitUntil(() => GameObject.Find("EpilogueManager") != null);
        
    }
    
    /// <summary>
    /// Move the toolbox under loading as a child, then remove all scenes. This ensures that the toolbox
    /// gets removed before a new test starts.
    /// </summary>
    [TearDown]
    public void TearDown()
    {
        // Move toolbox and DDOLs to Loading to unload
        if (GameObject.Find("Toolbox") != null)
            SceneManager.MoveGameObjectToScene(GameObject.Find("Toolbox"), SceneManager.GetSceneByName("Loading"));

        SceneManager.MoveGameObjectToScene(GameObject.Find("DDOLs"), SceneManager.GetSceneByName("Loading"));

        SceneController.sc.UnloadAdditiveScenes();
    }
    
    // Input parameters for testing different inputs.
    static bool[] bools = new bool[] { true, false };
    static int[]  ints  = new int[] { 0, 1, 2 };

    /// <summary>
    /// Check whether the transition between culprit selection and epilogue works intended by looking at the following:
    /// - if culprit is chosen:
    /// Check if hasWon is set to true, check if the gameState is epilogue and check if we are currently in the DialogueScene.
    /// - if innocent person is chosen:
    /// Check if hasWon is set to false, check if the gameState is epilogue and check if we are currently in the DialogueScene.
    /// </summary>
    [UnityTest]
    public IEnumerator CulpritSelectEpilogueTransition([ValueSource(nameof(bools))] bool hasChosenCulprit)
    { 
        // Find the gameObjects that holds the PortraitButtons as children.
        GameObject go = GameObject.Find("PortraitContainer");
        int culpritIndex = -1;
        int counter = 0;
        if (hasChosenCulprit)
        {
            // Find the index of the GameObject that corresponds with the culprit.
            foreach (CharacterInstance c in gm.currentCharacters.Where(c => c.isActive).ToList())
            {
                if (c.isCulprit)
                {
                    culpritIndex = counter;
                    break;
                }
                counter++;
            }
            
            // Invoke the onClick of the culprit.
            go.transform.GetChild(culpritIndex).GetComponent<GameButton>().onClick.Invoke();
        }
        else
        {
            // Find the index of the GameObject that corresponds with the culprit.
            foreach (CharacterInstance c in gm.currentCharacters.Where(c => c.isActive).ToList())
            {
                if (!c.isCulprit)
                {
                    culpritIndex = counter;
                    break;
                }
                counter++;
            }
            
            // Invoke the onClick of the culprit.
            go.transform.GetChild(culpritIndex).GetComponent<GameButton>().onClick.Invoke();
        }
        
        yield return new WaitUntil(() => SceneManager.GetSceneByName("DialogueScene").isLoaded); // Wait for scene to load.
        
        // Waiting for the DialogueManager to appear.
        yield return new WaitUntil(() => GameObject.Find("DialogueManager") != null);
        
        // Get the DialogueManager.
        dm = GameObject.Find("DialogueManager").GetComponent<DialogueManager>();
        
        // Check if the chosen character is the culprit when hasChosenCulprit is set to true,
        // else check if the chosen character is not the culprit.
        if (hasChosenCulprit)
            Assert.IsTrue(dm.currentRecipient.isCulprit);
        else
            Assert.IsFalse(dm.currentRecipient.isCulprit);
        
        // Check if the gameState is switched to epilogue.
        Assert.AreEqual(GameManager.GameState.Epilogue, gm.gameState);
        
        yield return null;
    }
    
    /// <summary>
    /// Check if the transition from the losing scenario switches from innocent person to culprit.
    /// </summary>
    [UnityTest]
    public IEnumerator EndDialogueToStartDialogueEpilogueTest()
    {
        // Find the gameObjects that holds the PortraitButtons as children.
        GameObject go = GameObject.Find("PortraitContainer");
        int culpritIndex = -1;
        int counter = 0;
  
        // Find the index of the GameObject that corresponds with the culprit.
        foreach (CharacterInstance c in gm.currentCharacters.Where(c => c.isActive).ToList())
        {
            if (!c.isCulprit)
            {
                culpritIndex = counter;
                break;
            }
            counter++;
        }
        
        // Invoke the onClick of the culprit.
        go.transform.GetChild(culpritIndex).GetComponent<GameButton>().onClick.Invoke();
        
        yield return new WaitUntil(() => SceneManager.GetSceneByName("DialogueScene").isLoaded); // Wait for scene to load.
        
        // Waiting for the DialogueManager to appear.
        yield return new WaitUntil(() => GameObject.Find("DialogueManager") != null);
        
        // Get the DialogueManager.
        dm = GameObject.Find("DialogueManager").GetComponent<DialogueManager>();

        // Check if the currentRecipient is not the culprit
        Assert.IsFalse(dm.currentRecipient.isCulprit);
        
        // Skip dialogue until first reflection moment
        while (GameObject.Find("InputField") == null)
        {
            yield return new WaitForSeconds(1);
                
            if (GameObject.Find("Skip Dialogue Button") != null)
                GameObject.Find("Skip Dialogue Button").GetComponent<Button>().onClick.Invoke();
        }
        
        // Get the DialogueManager.
        dm = GameObject.Find("DialogueManager").GetComponent<DialogueManager>();
        
        // Check if the currentRecipient is the culprit
        Assert.IsTrue(dm.currentRecipient.isCulprit);

        yield return null;
    }
    
    /// <summary>
    /// Check if the correct gameState and scene are loaded after the dialogue of the epilogue ends.
    /// </summary>
    [UnityTest]
    public IEnumerator EndDialogueEpilogueTest([ValueSource(nameof(bools))] bool hasWon)
    {
        // Find the gameObjects that holds the PortraitButtons as children.
        GameObject go = GameObject.Find("PortraitContainer");
        int culpritIndex = -1;
        int counter = 0;
        
        // Check if certain properties hold when hasWon is set to true or false.
        if (hasWon)
        {
            // Find the index of the GameObject that corresponds with the culprit.
            foreach (CharacterInstance c in gm.currentCharacters.Where(c => c.isActive).ToList())
            {
                if (c.isCulprit)
                {
                    culpritIndex = counter;
                    break;
                }
                counter++;
            }

            // Invoke the onClick of the culprit.
            go.transform.GetChild(culpritIndex).GetComponent<GameButton>().onClick.Invoke();
        }
        else
        {
            // Find the index of the GameObject that corresponds with the culprit.
            foreach (CharacterInstance c in gm.currentCharacters.Where(c => c.isActive).ToList())
            {
                if (!c.isCulprit)
                {
                    culpritIndex = counter;
                    break;
                }
                counter++;
            }

            // Invoke the onClick of the culprit.
            go.transform.GetChild(culpritIndex).GetComponent<GameButton>().onClick.Invoke();
        }
        
        yield return new WaitUntil(() => SceneManager.GetSceneByName("DialogueScene").isLoaded); // Wait for scene to load.
        
        // Waiting for the DialogueManager to appear.
        yield return new WaitUntil(() => GameObject.Find("DialogueManager") != null);
        
        // Get the EpilogueManager.
        var em = GameObject.Find("EpilogueManager").GetComponent<EpilogueManager>();
        
        // End the Epilogue.
        em.EndEpilogue(hasWon);

        yield return new WaitUntil(() => SceneManager.GetSceneByName("GameOverScene").isLoaded); // Wait for scene to load.
        
        // Waiting for the DialogueManager to appear.
        yield return new WaitUntil(() => GameObject.Find("GameOverManager") != null);
        
        if (hasWon)
        {
            // Check if the GameWinScene is loaded.
            bool inGameWinScene = SceneManager.GetSceneByName("GameOverScene").isLoaded;
            Assert.IsTrue(inGameWinScene);
    
            // Check if we are in the correct gameState.
            Assert.AreEqual(GameManager.GameState.GameWon, gm.gameState);
        }
        else
        {
            // Check if the GameOverScene is loaded.
            bool inGameOverScene = SceneManager.GetSceneByName("GameOverScene").isLoaded;
            Assert.IsTrue(inGameOverScene);
    
            // Check if we are in the correct gameState.
            Assert.AreEqual(GameManager.GameState.GameLoss, gm.gameState);
        }
        
        yield return null;
    }
}
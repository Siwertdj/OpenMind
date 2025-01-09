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
    private StoryObject story;
    private GameManager gm;

    /// <summary>
    /// Set up the game so that each test starts at the NPCSelectScene with the chosen story.
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
        var dm = GameObject.Find("DialogueManager").GetComponent<DialogueManager>();

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

    /*/// <summary>
    /// Check if the transition from the losing scenario works as intended when the dialogue switches from innocent person to culprit.
    /// </summary>
    [UnityTest]
    public IEnumerator EndDialogueToStartDialogueEpilogueTest()
    {
        // Set the finalChosenCulprit in GameManager.
        CharacterInstance innocentPerson = gm.GetRandomVictimNoCulprit();
        gm.FinalChosenCuplrit = innocentPerson;
        // Start the epilogue dialogue.
        gm.StartEpilogueDialogue(innocentPerson);
        
        yield return new WaitUntil(() => SceneManager.GetSceneByName("DialogueScene").isLoaded); // Wait for scene to load.
        
        // Waiting for the DialogueManager to appear.
        yield return new WaitUntil(() => GameObject.Find("DialogueManager") != null);
        
        // Get the DialogueManager.
        var dm = GameObject.Find("DialogueManager").GetComponent<DialogueManager>();

        // End the dialogue and add a speakingObject to the responses list in order to start dialogue with a new person.
        dm.currentObject = new TerminateDialogueObject();
        List<string> text = new List<string>(){"hello"};
        GameObject[] background = new GameObject[1]{gm.story.hintBackground};
        dm.currentObject.Responses.Add(new ContentDialogueObject(text, null, background));
        dm.currentObject.Execute();
        
        // Check if the DialogueObjects in the responses list of the currentObject
        Assert.GreaterOrEqual(1, dm.currentObject.Responses.Count);

        // Wait for new dialogue with culprit to unload and load.
        yield return new WaitUntil(() => !SceneManager.GetSceneByName("DialogueScene").isLoaded); // Wait for scene to load.
        
        yield return new WaitUntil(() => SceneManager.GetSceneByName("DialogueScene").isLoaded); // Wait for scene to load.
        
        // Waiting for the DialogueManager to appear.
        yield return new WaitUntil(() => GameObject.Find("DialogueManager") != null);
        
        // Get the DialogueManager.
        dm = GameObject.Find("DialogueManager").GetComponent<DialogueManager>();
        
        // Check if the dialogue switches to the culprit.
        Assert.AreEqual(gm.GetCulprit().characterName, dm.currentRecipient.characterName);

        yield return null;
    }*/
    
    /*
    /// <summary>
    /// Check if the correct gameState and scene are loaded after the dialogue of the epilogue ends.
    /// </summary>
    [UnityTest]
    public IEnumerator EndDialogueEpilogueTest([ValueSource(nameof(bools))] bool hasWon)
    {
        // Set the hasWon variable from GameManager to true.
        gm.hasWon = hasWon;
        
        // Check if certain properties hold when hasWon is set to true or false.
        if (gm.hasWon)
        {
            // Set the finalChosenCulprit in GameManager.
            CharacterInstance culprit = gm.GetCulprit();
            gm.FinalChosenCuplrit = culprit;
            // Start the epilogue dialogue.
            gm.StartEpilogueDialogue(culprit);
        
            yield return new WaitUntil(() => SceneManager.GetSceneByName("DialogueScene").isLoaded); // Wait for scene to load.
        
            // Waiting for the DialogueManager to appear.
            yield return new WaitUntil(() => GameObject.Find("DialogueManager") != null);
            
            // Get the DialogueManager.
            var dm = GameObject.Find("DialogueManager").GetComponent<DialogueManager>();
            
            // End the dialogue.
            dm.currentObject = new TerminateDialogueObject();
            dm.currentObject.Execute();
        
            // Check if the DialogueObjects in the responses list of the currentObject
            Assert.AreEqual(0, dm.currentObject.Responses.Count);
            
            yield return new WaitUntil(() => SceneManager.GetSceneByName("GameWinScene").isLoaded); // Wait for scene to load.
            
            // TODO: swap gameState in EndDialogue to be before scene transition.
            // Check if the GameWinScene is loaded.
            bool inGameWinScene = SceneManager.GetSceneByName("GameWinScene").isLoaded;
            Assert.IsTrue(inGameWinScene);
        
            // Check if we are in the correct gameState.
            Assert.AreEqual(GameManager.GameState.GameWon, gm.gameState);
        }
        else
        {
            // Set the finalChosenCulprit in GameManager.
            CharacterInstance innocentPerson = gm.GetRandomVictimNoCulprit();
            gm.FinalChosenCuplrit = innocentPerson;
            // Start the epilogue dialogue.
            gm.StartEpilogueDialogue(innocentPerson);
        
            yield return new WaitUntil(() => SceneManager.GetSceneByName("DialogueScene").isLoaded); // Wait for scene to load.
        
            // Waiting for the DialogueManager to appear.
            yield return new WaitUntil(() => GameObject.Find("DialogueManager") != null);
            
            // Get the DialogueManager.
            var dm = GameObject.Find("DialogueManager").GetComponent<DialogueManager>();
            
            // End the dialogue.
            dm.currentObject = new TerminateDialogueObject();
            dm.currentObject.Execute();
        
            // Check if the DialogueObjects in the responses list of the currentObject
            Assert.AreEqual(0, dm.currentObject.Responses.Count);
            
            yield return new WaitUntil(() => SceneManager.GetSceneByName("GameOverScene").isLoaded); // Wait for scene to load.
            
            // Check if the GameOverScene is loaded.
            bool inGameOverScene = SceneManager.GetSceneByName("GameOverScene").isLoaded;
            Assert.IsTrue(inGameOverScene);
        
            // Check if we are in the correct gameState.
            Assert.AreEqual(GameManager.GameState.GameLoss, gm.gameState);
        }

        yield return null;
    }*/
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    /*/// <summary>
    /// Check if the transition from CulpritSelect to Epilogue GameState is done correctly.
    /// </summary>
    [UnityTest]
    public IEnumerator CulpritSelectToEpilogueGameStateTest()
    {
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
        
        // Set this to maxValue in order to make sure that no more questions can be asked.
        gm.numQuestionsAsked = int.MaxValue;
        
        // Start dialogue with a character, then go back to NpcSelect scene in order to apply the changes of the variables.
        CharacterInstance character = gm.currentCharacters[0];
        gm.StartDialogue(character);
        yield return new WaitUntil(() => SceneManager.GetSceneByName("DialogueScene").isLoaded); // Wait for scene to load.

        // Waiting for the DialogueManager to appear.
        yield return new WaitUntil(() => GameObject.Find("DialogueManager") != null);
        
        // Get the DialogueManager.
        var dm = GameObject.Find("DialogueManager").GetComponent<DialogueManager>();
 
        // End the NpcDialogue.
        dm.currentObject = new TerminateDialogueObject();
        dm.currentObject.Execute();
        
        Assert.AreEqual(GameManager.GameState.CulpritSelect, gm.gameState);
        
        // Start the game cycle
        yield return new WaitUntil(() => SceneManager.GetSceneByName("NPCSelectScene").isLoaded);
        // Waiting for the DialogueManager to appear.
        yield return new WaitUntil(() => GameObject.Find("SelectionManager") != null);
        
        // Set the finalChosenCulprit in GameManager.
        CharacterInstance innocentPerson = gm.GetRandomVictimNoCulprit();
        gm.FinalChosenCuplrit = innocentPerson;
        
        // Start the epilogue dialogue.
        gm.StartEpilogueDialogue(innocentPerson);
        
        Assert.AreEqual(GameManager.GameState.Epilogue, gm.gameState);
    }*/
    
    /*/// <summary>
    /// Check if the transition from the Epilogue to GameWon GameState is done correctly.
    /// </summary>
    [UnityTest]
    public IEnumerator EpilogueToGameWonGameStateTest()
    {
        // Start the game cycle
        yield return new WaitUntil(() => SceneManager.GetSceneByName("NPCSelectScene").isLoaded);
        // Waiting for the DialogueManager to appear.
        yield return new WaitUntil(() => GameObject.Find("SelectionManager") != null);
        
        // Set the finalChosenCulprit in GameManager.
        CharacterInstance culprit = gm.GetCulprit();
        gm.FinalChosenCuplrit = culprit;
        
        // Chosen the correct culprit
        gm.hasWon = true;
        
        // Start the epilogue dialogue.
        gm.StartEpilogueDialogue(culprit);
        
        yield return new WaitUntil(() => SceneManager.GetSceneByName("DialogueScene").isLoaded); // Wait for scene to load.
        
        // Waiting for the DialogueManager to appear.
        yield return new WaitUntil(() => GameObject.Find("DialogueManager") != null);
            
        // Get the DialogueManager.
        var dm = GameObject.Find("DialogueManager").GetComponent<DialogueManager>();
            
        // End the epilogue dialogue
        dm.currentObject = new TerminateDialogueObject();
        dm.currentObject.Execute();
        
        // Check if we are in the correct gameState.
        Assert.AreEqual(GameManager.GameState.GameWon, gm.gameState);
    }*/
}
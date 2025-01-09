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

public class GameOverManagerPlayTests
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
    
    /*
    /// <summary>
    /// Checks if the "RestartStoryScene" correctly resets the variables.
    /// </summary>
    [UnityTest]
    public IEnumerator RestartStoryTest()
    {
        // Start the game cycle.
        yield return new WaitUntil(() => SceneManager.GetSceneByName("NPCSelectScene").isLoaded); // Wait for scene to load.

        // Start dialogue.
        gm.StartDialogue(gm.currentCharacters[0]);
        yield return new WaitUntil(() => SceneManager.GetSceneByName("DialogueScene").isLoaded); // Wait for scene to load.

        // Call RestartStoryScene to check if values get reset.
        gm.RestartStoryScene();
        yield return new WaitUntil(() => SceneManager.GetSceneByName("NPCSelectScene").isLoaded); // Wait for scene to load.

        // Check if we are in the NpcSelect gameState and if only 2 scenes exist,
        // namely NpcSelectScene and Loading.
        // TODO: currently the gameState is not reset correctly.
        Assert.AreEqual(GameManager.GameState.NpcSelect, gm.gameState);
        Assert.AreEqual(2, SceneManager.loadedSceneCount);
        Assert.IsTrue(SceneManager.GetSceneByName("Loading").isLoaded);
        Assert.IsTrue(SceneManager.GetSceneByName("NPCSelectScene").isLoaded);

        yield return null;
    }*/
    
    
    /*/// <summary>
    /// Checks if the "RetryStoryScene" resets all characters to be active.
    /// </summary>
    [UnityTest]
    public IEnumerator RetryStoryTest()
    {
        gm.RetryStoryScene();

        // True if all characters are active.
        bool actual = gm.currentCharacters.Count(c => c.isActive) == gm.currentCharacters.Count();

        // Check if all characters are active.
        Assert.IsTrue(actual);

        yield return null;
    }*/
    
    /*
   /// <summary>
   /// Check if the transition from GameLoss to NpcSelect GameState is done correctly, by retrying the game.
   /// </summary>
   [UnityTest]
   public IEnumerator RetryGameStateTest()
   {
       // Set the gamestate to gameloss
       gm.gameState = GameManager.GameState.GameLoss;

       // Retry game
       gm.RetryStoryScene();

       // Waiting for the NpcSelectScene to appear
       yield return new WaitUntil(() => SceneManager.GetSceneByName("NPCSelectScene").isLoaded);
       // Waiting for the DialogueManager to appear.
       yield return new WaitUntil(() => GameObject.Find("SelectionManager") != null);

       Assert.AreEqual(GameManager.GameState.NpcSelect, gm.gameState);
   }
   */
    
    /*
    /// <summary>
    /// Check if the transition from GameWin to NpcSelect GameState is done correctly, by restarting the game.
    /// </summary>
    [UnityTest]
    public IEnumerator RestartGameStateTest()
    {
        // Set the gamestate to GameWon
        gm.gameState = GameManager.GameState.GameWon;

        // Restart game
        gm.RestartStoryScene();

        // Waiting for the NpcSelectScene to appear
        yield return new WaitUntil(() => SceneManager.GetSceneByName("NPCSelectScene").isLoaded);
        // Waiting for the DialogueManager to appear.
        yield return new WaitUntil(() => GameObject.Find("SelectionManager") != null);

        Assert.AreEqual(GameManager.GameState.NpcSelect, gm.gameState);
    }
    */
}
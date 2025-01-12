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
    private GameOverManager gom;

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
        
        // Find the gameObjects that holds the PortraitButtons as children.
        GameObject go = GameObject.Find("PortraitContainer");
        int culpritIndex = -1;
        int counter = 0;
        
        
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
        
        
        yield return new WaitUntil(() => SceneManager.GetSceneByName("DialogueScene").isLoaded); // Wait for scene to load.
        
        // Waiting for the DialogueManager to appear.
        yield return new WaitUntil(() => GameObject.Find("DialogueManager") != null);
        
        // Get the DialogueManager.
        dm = GameObject.Find("DialogueManager").GetComponent<DialogueManager>();
        
        var em = GameObject.Find("EpilogueManager").GetComponent<EpilogueManager>();
        
        em.EndEpilogue(true);

        yield return new WaitUntil(() => SceneManager.GetSceneByName("GameOverScene").isLoaded); // Wait for scene to load.
        
        // Waiting for the DialogueManager to appear.
        yield return new WaitUntil(() => GameObject.Find("GameOverManager") != null);
        
        // Get the GameOverManager.
        gom = GameObject.Find("GameOverManager").GetComponent<GameOverManager>();
    }
    
    /*
    /// <summary>
    /// Set up the game so that each test starts at the GameOverScene.
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
        
        // Get the GameManager.
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();

        // Start the game with the chosen story.
        gm.StartGame(null, story);
        
        // Load the GameOverScene.
        SceneManager.LoadScene("GameOverScene");
        yield return new WaitUntil(() => SceneManager.GetSceneByName("GameOverScene").isLoaded); // Wait for scene to load.
        
        // Waiting for the GameOverManager to appear.
        yield return new WaitUntil(() => GameObject.Find("GameOverManager") != null);

        // Get the GameOverManager.
        gom = GameObject.Find("GameOverManager").GetComponent<GameOverManager>();
        
        gom.StartGameOver(gm, false, gm.currentCharacters, gm.GetCulprit().id, story);
    }*/

    /// <summary>
    /// Move the toolbox under loading as a child, then remove all scenes. This ensures that the toolbox
    /// gets removed before a new test starts.
    /// </summary>
    [TearDown]
    public void TearDown()
    {
        //TODO: see why loading gets unloaded.
        // Move toolbox and DDOLs to Loading to unload
        if (GameObject.Find("Toolbox") != null)
            SceneManager.MoveGameObjectToScene(GameObject.Find("Toolbox"), SceneManager.GetSceneByName("Loading"));

        SceneManager.MoveGameObjectToScene(GameObject.Find("DDOLs"), SceneManager.GetSceneByName("Loading"));

        SceneController.sc.UnloadAdditiveScenes();
    }
    
    /// <summary>
    /// Checks if the "RestartStoryScene" correctly resets the variables.
    /// </summary>
    [UnityTest]
    public IEnumerator RestartStoryTest()
    {
        // Holds the characters prior to calling retry.
        List<CharacterInstance> charactersPrior = new List<CharacterInstance>();
        charactersPrior.AddRange(gm.currentCharacters);
        
        // Restart the game.
        gom.Restart();
        
        yield return new WaitUntil(() => SceneManager.GetSceneByName("NPCSelectScene").isLoaded); // Wait for scene to load.
        
        // Get the new GameManager.
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        
        // Check if the new list has the same amount of characters as before.
        bool actual = gm.currentCharacters.Count(c => c.isActive) == charactersPrior.Count();
        Assert.IsTrue(actual);
        
        // Check if we are in the NpcSelect gameState and if the following 2 scenes exist,
        // namely NpcSelectScene and Loading.
        Assert.AreEqual(GameManager.GameState.NpcSelect, gm.gameState);
        Assert.IsTrue(SceneManager.GetSceneByName("Loading").isLoaded);
        Assert.IsTrue(SceneManager.GetSceneByName("NPCSelectScene").isLoaded);

        yield return null;
    }
    
    /// <summary>
    /// Checks if the "RetryStoryScene" resets all characters to be active.
    /// </summary>
    [UnityTest]
    public IEnumerator RetryStoryTest()
    {
        // Holds the characters prior to calling retry.
        List<CharacterInstance> charactersPrior = new List<CharacterInstance>();
        charactersPrior.AddRange(gm.currentCharacters);
        
        // Retry the game.
        gom.Retry();
        
        yield return new WaitUntil(() => SceneManager.GetSceneByName("NPCSelectScene").isLoaded); // Wait for scene to load.
        
        // Get the new GameManager.
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        
        // Check if the new list has the same amount of characters as before.
        bool actual = gm.currentCharacters.Count(c => c.isActive) == charactersPrior.Count();
        Assert.IsTrue(actual);
        
        bool hasSameCharacters = true;
        // Check if all characters are active and if the same characters are used.
        foreach (CharacterInstance character in gm.currentCharacters)
        {
            if (charactersPrior.All(c => c.characterName != character.characterName))
                hasSameCharacters = false;
        }
        
        // Check if the bool hasSameCharacters returns true.
        Assert.IsTrue(hasSameCharacters);

        // Check if we are in the NpcSelect gameState and if the following 2 scenes exist,
        // namely NpcSelectScene and Loading.
        Assert.AreEqual(GameManager.GameState.NpcSelect, gm.gameState);
        Assert.IsTrue(SceneManager.GetSceneByName("Loading").isLoaded);
        Assert.IsTrue(SceneManager.GetSceneByName("NPCSelectScene").isLoaded);
        
        yield return null;
    }
    
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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DialogueManagerPlayTest
{
    private GameManager     gm;
    private DialogueManager dm;
    private StoryObject     story;
    
    #region Setup
    
    /// <summary>
    /// Set up the game so that each test starts at the DialogueScene, talking to an NPC.
    /// </summary>
    [UnitySetUp]
    public IEnumerator SetUp()
    {
        // Load "Loading" scene
        SceneManager.LoadScene("Loading");
        yield return new WaitUntil(() => SceneManager.GetSceneByName("Loading").isLoaded);

        // Set global variables
        story = Resources.LoadAll<StoryObject>("Stories")[0];
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        
        // Start NPC select scene
        gm.StartGame(null, story);
        yield return new WaitUntil(() => SceneManager.GetSceneByName("NPCSelectScene").isLoaded);
        
        // Start dialogue scene
        gm.StartDialogue(gm.currentCharacters[0]);
        yield return new WaitUntil(() => SceneManager.GetSceneByName("DialogueScene").isLoaded);

        // Set global variable
        dm = GameObject.Find("DialogueManager").GetComponent<DialogueManager>();
    }

    /// <summary>
    /// Move the toolbox under loading as a child, then remove all scenes. This ensures that the toolbox
    /// gets removed before a new test starts.
    /// </summary>
    [TearDown]
    public void TearDown()
    {
        SceneManager.MoveGameObjectToScene(GameObject.Find("Toolbox"), SceneManager.GetSceneByName("Loading"));
        SceneController.sc.UnloadAdditiveScenes();
    }
    
    #endregion
    
    /// <summary>
    /// Tests if responses get correctly loaded after dialogue has been completed.
    /// </summary>
    [UnityTest]
    public IEnumerator OnDialogueCompleteTest()
    {
        // Simulate that dialogue gets completed.
        dm.OnDialogueComplete();
        
        // There should be at least one response.
        Assert.IsTrue(dm.currentObject.Responses.Count > 0);
        yield return null;
    }
    
    /// <summary>
    /// Check if the ReplaceBackground method works as intended.
    /// </summary>
    [UnityTest]
    public IEnumerator ReplaceBackgroundTest()
    {
        // Get current background.
        var backgroundField = new [] { GameObject.Find("BackgroundField") };

        // Replace the background.
        dm.ReplaceBackground(backgroundField);
        
        // Transform current background
        var parent = backgroundField[0].transform;

        // Check if the background has changed.
        Assert.AreEqual(parent.childCount, GameObject.Find("BackgroundField").transform.childCount);

        yield return null;
    }

    /// <summary>
    /// Tests if the buttons to ask questions get correctly loaded in after completing the dialogue.
    /// </summary>
    [UnityTest]
    public IEnumerator PromptButtonsTest()
    {
        // Complete the dialogue.
        dm.OnDialogueComplete();

        // Get number of question and back buttons.
        int numQuestionButtons = GameObject.FindGameObjectsWithTag("Button").Where(b => b.name == "questionButton").Count();
        int numBackButtons = GameObject.FindGameObjectsWithTag("Button").Where(b => b.name == "backButton").Count();

        // There should be more than zero question buttons, while there should only be one back button.
        Assert.IsTrue(numQuestionButtons > 0);
        Assert.IsTrue(numBackButtons == 1);
        yield return null;
    }

    /// <summary>
    /// Check if the back button works as intended when there are enough characters and questions left.
    /// </summary>
    [UnityTest]
    public IEnumerator BackButtonTest()
    {
        // Complete the dialogue and move to the BackButton screen.
        dm.OnDialogueComplete();

        // Check if we are currently in the gameState NpcDialogue
        Assert.AreEqual(GameManager.GameState.NpcDialogue, gm.gameState);
        bool inDialogueScene = SceneManager.GetSceneByName("DialogueScene").isLoaded;
        Assert.IsTrue(inDialogueScene);
        
        // Return to the NpcSelect scene by pressing the BackButton.
        Button backButton = GameObject.Find("backButton").GetComponent<Button>();
        backButton.onClick.Invoke();
        yield return new WaitForSeconds(3); // Wait for it to load
        
        // Check if we are currently in the gameState NpcDialogue
        Assert.AreEqual(GameManager.GameState.NpcSelect, gm.gameState);
        bool inNpcSelectScene = SceneManager.GetSceneByName("NPCSelectScene").isLoaded;
        Assert.IsTrue(inNpcSelectScene);

        yield return null;
    }
}
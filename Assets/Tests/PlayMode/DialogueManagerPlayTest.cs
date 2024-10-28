using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DialogueManagerPlayTest
{
    private GameManager     gm;
    private DialogueManager dm;
    
    #region Setup
    
    /// <summary>
    /// SetUp which is run for every test (currently WaitForSeconds is used, this will probably be changed in the
    /// future. Refer to #testing channel for the correct implementation (sanders dingetje met WaitUntil)).
    /// </summary>
    [UnitySetUp]
    public IEnumerator SetUp()
    {
        // Load scene
        SceneManager.LoadScene("StartScreenScene");
        yield return new WaitUntil(() => SceneManager.GetSceneByName("StartScreenScene").isLoaded); // Wait for scene to load
        yield return new WaitForSeconds(1);
        Button newGameButton = GameObject.Find("NewGameButton").GetComponent<Button>();
        newGameButton.onClick.Invoke();
        yield return new WaitForSeconds(1);
        Button noButton = GameObject.Find("NoButton").GetComponent<Button>();
        noButton.onClick.Invoke();
        yield return new WaitForSeconds(1);
        Button storyAButton = GameObject.Find("StoryA_Button").GetComponent<Button>();
        storyAButton.onClick.Invoke();
        yield return new WaitForSeconds(2);

        gm = GameObject.Find("GameManager").GetComponent<GameManager>();

        gm.StartDialogue(gm.currentCharacters[0]);
        yield return new WaitUntil(() => SceneManager.GetSceneByName("DialogueScene").isLoaded);

        dm = GameObject.Find("DialogueManager").GetComponent<DialogueManager>();
    }

    [TearDown]
    public void TearDown()
    {
        SceneManager.MoveGameObjectToScene(GameObject.Find("Toolbox"), SceneManager.GetSceneByName("Loading"));
        SceneController.sc.UnloadAdditiveScenes();
    }
    
    #endregion
    
    [UnityTest]
    public IEnumerator OnDialogueCompleteTest()
    {
        var expected = dm.currentObject.Responses[0];
        
        dm.OnDialogueComplete();

        var actual = dm.currentObject;
        
        Assert.AreEqual(expected, actual);
        
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
        yield return new WaitForSeconds(3); // Wait for it to load
        
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

    static bool[] bools = new bool[] { true, false };
    /// <summary>
    /// Check if the ReplaceBackground method works as intended.
    /// </summary>
    [UnityTest]
    public IEnumerator ReplaceBackgroundTest([ValueSource(nameof(bools))] bool newBackground)
    {
        // Get current background.
        var backgroundField = GameObject.Find("BackgroundField");
        var b1 = backgroundField.transform.GetChild(0);

        // Get different background.
        //var b2 = new();
        if (newBackground)
        {
            
        }
        else
        {
            
        }
        
        // Replace the background.
        //dm.ReplaceBackground(b2);
        
        // Get current background after replacing the background.
        backgroundField = GameObject.Find("BackgroundField");
        var b3 = backgroundField.transform.GetChild(0);
        
        if (newBackground)
        {
            // Check if the background has changed.
            Assert.AreNotEqual(b1, b3);
        }
        else
        {
            // Check if background has not changed.
            Assert.AreEqual(b1, b3);
        }

        yield return null;
    }
}
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
    [OneTimeSetUp]
    public void CreateLoadingScene()
    {
        // hoi
    }
    
    [UnityTest]
    public IEnumerator OnDialogueCompleteTest()
    {
        var d = GameObject.Find("DialogueManager");
        var dm = d.GetComponent<DialogueManager>();

        var expected = dm.currentObject.Responses[0];
        
        dm.OnDialogueComplete();

        var actual = dm.currentObject;
        
        Assert.AreEqual(expected, actual);
        
        yield return null;
    }

    /// <summary>
    /// Check if the back button works as intended when there are enough characters and questions left.
    /// </summary>
    /*[UnityTest]
    public IEnumerator BackButtonTest()
    {
        // Load scene
        SceneManager.LoadScene("Loading");
        yield return new WaitForSeconds(3); // Wait for it to load
        
        // Get GameManager object
        var g = GameObject.Find("GameManager");
        var gm = g.GetComponent<GameManager>();

        // Start the game
        //gm.StartGame();
        yield return new WaitForSeconds(3); // Wait for it to load
        
        // Start the dialogue with a character
        var character = gm.currentCharacters[0];
        gm.StartDialogue(character);
        yield return new WaitForSeconds(3); // Wait for it to load
        
        // Get DialogueManager object
        var d = GameObject.Find("DialogueManager");
        var dm = d.GetComponent<DialogueManager>();
        
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
        // Load scene
        SceneManager.LoadScene("Loading");
        yield return new WaitForSeconds(3); // Wait for it to load
        
        // Get GameManager object
        var g = GameObject.Find("GameManager");
        var gm = g.GetComponent<GameManager>();

        // Start the game
        //gm.StartGame();
        yield return new WaitForSeconds(3); // Wait for it to load
        
        // Start the dialogue with a character
        var character = gm.currentCharacters[0];
        gm.StartDialogue(character);
        yield return new WaitForSeconds(3); // Wait for it to load
        
        // Get DialogueManager object
        var d = GameObject.Find("DialogueManager");
        var dm = d.GetComponent<DialogueManager>();
        
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
    }*/
    
}
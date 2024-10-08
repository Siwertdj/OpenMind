using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

public class GameManagerPlayTest
{
    /// <summary>
    /// Checks if the "HasQuestionsLeft" method works.
    /// </summary>
    [UnityTest]
    public IEnumerator HasQuestionsLeftTest()
    {
        // Load scene
        SceneManager.LoadScene("Loading");

        // Get GameManager object
        var g = GameObject.Find("GameManager");
        var gm = g.GetComponent<GameManager>();

        // Set up expected and actual values
        var expected = gm.numQuestionsAsked <= 1;
        var actual = gm.HasQuestionsLeft();

        // Check if they are equal
        Assert.AreEqual(expected, actual);
        
        yield return null;
    }

    /// <summary>
    /// Checks if the "GetCulpritTest" method works.
    /// </summary>
    [UnityTest]
    public IEnumerator GetCulpritTest()
    {
        // Load scene
        SceneManager.LoadScene("Loading");
        yield return new WaitForSeconds(3); // Wait for it to load
        
        // Get "GameManager" object
        var g = GameObject.Find("GameManager");
        var gm = g.GetComponent<GameManager>();

        // Set up expected and actual values
        var expected = gm.currentCharacters.Find(c => c.isCulprit);
        var actual = gm.GetCulprit();

        // Check if they are equal
        Assert.AreEqual(expected, actual);
        
        yield return null;
    }

    /// <summary>
    /// Checks if the "EnoughCharacters" method works.
    /// </summary>
    [UnityTest]
    public IEnumerator EnoughCharactersTest()
    {
        // Load scene
        SceneManager.LoadScene("Loading");
        yield return new WaitForSeconds(3); // Wait for it to load
        
        // Get "GameManager" object
        var g = GameObject.Find("GameManager");
        var gm = g.GetComponent<GameManager>();

        // Set up expected and actual values
        var expected = gm.currentCharacters.Count(c => c.isActive) > 2;
        var actual = gm.EnoughCharactersRemaining();

        // Check if they are equal
        Assert.AreEqual(expected, actual);
        
        yield return null;
    }

    /// <summary>
    /// Checks if the "RetryStoryScene" method works.
    /// </summary>
    [UnityTest]
    public IEnumerator RetryStoryTest()
    {
        // Load scene
        SceneManager.LoadScene("Loading");
        yield return new WaitForSeconds(3); // Wait for it to load
        
        // Get "GameManager" object
        var g = GameObject.Find("GameManager");
        var gm = g.GetComponent<GameManager>();
        
        gm.RetryStoryScene();
        
        // Set up actual value
        var actual = gm.currentCharacters.Count(c => c.isActive) == gm.currentCharacters.Count();

        // Check if it holds
        Assert.IsTrue(actual);
        
        yield return null;
    }

    /// <summary>
    /// Checks if the "GetRandomVictimNoCulprit" method works.
    /// </summary>
    [UnityTest]
    public IEnumerator ChooseVictimTest()
    {
        // Load scene
        SceneManager.LoadScene("Loading");
        yield return new WaitForSeconds(3); // Wait for it to load

        // Get "GameManager" object
        var g = GameObject.Find("GameManager");
        var gm = g.GetComponent<GameManager>();

        // Get victim
        var victim = gm.GetRandomVictimNoCulprit();

        // Check if it actually returned a victim
        Assert.IsTrue(victim != null);
        
        yield return null;
    }

    /// <summary>
    /// Checks if the "EndCycle" method works.
    /// </summary>
    [UnityTest]
    public IEnumerator EndCycleTest()
    {
        // Load scene
        SceneManager.LoadScene("Loading");
        yield return new WaitForSeconds(3); // Wait for it to load

        // Get "GameManager" object
        var g = GameObject.Find("GameManager");
        var gm = g.GetComponent<GameManager>();

        // End cycle
        gm.EndCycle();
        
        // Get current scene
        var scene = SceneManager.GetActiveScene().name;

        // See if it's still equal to the "main" scene of the game
        // No scene should be switched, because it's an additive scene
        Assert.AreEqual("Loading", scene);
        
        yield return null;
    }

    /// <summary>
    /// Checks if the "StartDialogue" method works.
    /// </summary>
    [UnityTest]
    public IEnumerator StartDialogueTest()
    {
        // Load scene
        SceneManager.LoadScene("Loading");
        yield return new WaitForSeconds(3); // Wait for it to load

        // Get "GameManager" object
        var g = GameObject.Find("GameManager");
        var gm = g.GetComponent<GameManager>();

        // Get character to start dialogue with
        var character = gm.currentCharacters[0];
        gm.StartDialogue(character);

        // Get current scene
        var scene = SceneManager.GetActiveScene().name;

        // See if it's still equal to the "main" scene of the game
        // No scene should be switched, because it's an additive scene
        Assert.AreEqual("Loading", scene);
        
        yield return null;
    }
}

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
        yield return new WaitForSeconds(3); // Wait for it to load
        
        // Get GameManager object
        var g = GameObject.Find("GameManager");
        var gm = g.GetComponent<GameManager>();

        // Set up expected and actual values
        var expected = gm.numQuestionsAsked <= 1;
        var actual = gm.HasQuestionsLeft();

        yield return new WaitForSeconds(1);

        // Check if they are equal
        Assert.AreEqual(expected, actual);
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

        yield return new WaitForSeconds(1);
        
        // Check if they are equal
        Assert.AreEqual(expected, actual);
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

        yield return new WaitForSeconds(1);
        
        // Check if they are equal
        Assert.AreEqual(expected, actual);
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

        yield return new WaitForSeconds(1);

        // Check if it holds
        Assert.IsTrue(actual);
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

        yield return new WaitForSeconds(1);

        // Check if it actually returned a victim
        Assert.IsTrue(victim != null);
    }
}

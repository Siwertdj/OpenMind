using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

public class GameManagerPlayTest
{
    [UnityTest]
    public IEnumerator HasQuestionsLeftTest()
    {
        SceneManager.LoadScene("Loading");

        yield return new WaitForSeconds(3);
        
        var g = GameObject.Find("GameManager");
        var gm = g.GetComponent<GameManager>();

        var expected = gm.numQuestionsAsked <= 1;
        var actual = gm.HasQuestionsLeft();

        yield return new WaitForSeconds(1);

        Assert.AreEqual(expected, actual);
    }

    [UnityTest]
    public IEnumerator GetCulpritTest()
    {
        SceneManager.LoadScene("Loading");

        yield return new WaitForSeconds(3);
        
        var g = GameObject.Find("GameManager");
        var gm = g.GetComponent<GameManager>();

        var expected = gm.currentCharacters.Find(c => c.isCulprit);
        var actual = gm.GetCulprit();

        yield return new WaitForSeconds(1);
        
        Assert.AreEqual(expected, actual);
    }

    [UnityTest]
    public IEnumerator EnoughCharactersTest()
    {
        SceneManager.LoadScene("Loading");

        yield return new WaitForSeconds(3);
        
        var g = GameObject.Find("GameManager");
        var gm = g.GetComponent<GameManager>();

        var expected = gm.currentCharacters.Count(c => c.isActive) > 2;
        var actual = gm.EnoughCharactersRemaining();

        yield return new WaitForSeconds(1);
        
        Assert.AreEqual(expected, actual);
    }

    [UnityTest]
    public IEnumerator RetryStoryTest()
    {
        SceneManager.LoadScene("Loading");

        yield return new WaitForSeconds(3);
        
        var g = GameObject.Find("GameManager");
        var gm = g.GetComponent<GameManager>();
        
        gm.RetryStoryScene();
        
        var actual = gm.currentCharacters.Count(c => c.isActive) == gm.currentCharacters.Count();

        yield return new WaitForSeconds(1);

        Assert.IsTrue(actual);
    }
}

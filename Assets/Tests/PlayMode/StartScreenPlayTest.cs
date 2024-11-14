using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

public class StartScreenPlayTest : MonoBehaviour
{
    private StartMenuManager sm;
    
    #region Setup and TearDown

    [UnitySetUp]
    public IEnumerator Setup()
    {
        SceneManager.LoadScene("StartScreenScene");
        yield return new WaitUntil(() => SceneManager.GetSceneByName("StartScreenScene").isLoaded);

        sm = GameObject.Find("StartMenuManager").GetComponent<StartMenuManager>();
        Debug.Log(sm);
    }

    #endregion
    
    /// <summary>
    /// Checks if the scene is set up correctly.
    /// </summary>
    [UnityTest]
    public IEnumerator StartScreenStartTest()
    {
        // Checks to see if the right buttons are active from the start
        Assert.IsTrue(GameObject.Find("MainMenuOptions").activeSelf);
        Assert.IsTrue(GameObject.Find("NewGameButton").activeSelf);
        yield return null;
    }

    /// <summary>
    /// Checks if the skipprologue prompt is set up correctly
    /// </summary>
    /// <returns></returns>
    [UnityTest]
    public IEnumerator SkipPrologueTest()
    {
        sm.OpenSkipProloguePrompt();
        // Checks to see if the right buttons are active
        Assert.IsTrue(GameObject.Find("SkipPrologueWindow").activeSelf);
        yield return null;
    }

    /// <summary>
    /// Checks if the prologue is loaded correctly
    /// </summary>
    /// <returns></returns>
    [UnityTest]
    public IEnumerator PrologueTest()
    {
        sm.StartPrologue();
        yield return new WaitUntil(() => SceneManager.GetSceneByName("PrologueScene").isLoaded);
        var s = SceneManager.GetActiveScene();
        Assert.IsTrue(s.name == "PrologueScene");
        
        yield return null;
    }
    
    /// <summary>
    /// Checks if storyscene is loaded properly
    /// </summary>
    /// <returns></returns>
    [UnityTest]
    public IEnumerator ChooseStoryTest()
    {
        sm.SkipPrologue();
        yield return new WaitUntil(() => SceneManager.GetSceneByName("StorySelectScene").isLoaded);
        Debug.Log("hoi");
        var s = SceneManager.GetActiveScene();
        Assert.IsTrue(s.name == "StorySelectScene");
        
        yield return null;
    }
}

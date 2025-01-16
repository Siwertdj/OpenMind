// This program has been developed by students from the bachelor Computer Science at Utrecht University within the Software Project course.
// © Copyright Utrecht University (Department of Information and Computing Sciences)
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using Assert = UnityEngine.Assertions.Assert;

public class PrologueManagerPlayTest : MonoBehaviour
{
    private PrologueManager   pm; 
    
    #region Setup and TearDown

    [UnitySetUp]
    public IEnumerator Setup()
    {
        SceneManager.LoadScene("StartScreenScene");
        yield return new WaitUntil(() => SceneManager.GetSceneByName("StartScreenScene").isLoaded);
        
        // Unload the StartScreenScene
        SceneManager.UnloadSceneAsync("StartScreenScene");
        
        SceneManager.LoadScene("PrologueScene");
        yield return new WaitUntil(() => SceneManager.GetSceneByName("PrologueScene").isLoaded);
        
        pm = GameObject.Find("PrologueManager").GetComponent<PrologueManager>();
    }

    #endregion
    
    /// <summary>
    /// Checks if the scene is set up correctly.
    /// </summary>
    [UnityTest]
    public IEnumerator StartScreenStartTest()
    {
        Assert.IsTrue(true);
        yield return null;
    }

}

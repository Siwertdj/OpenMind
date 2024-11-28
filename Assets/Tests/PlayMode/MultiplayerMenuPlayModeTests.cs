// This program has been developed by students from the bachelor Computer Science at Utrecht University within the Software Project course.
// © Copyright Utrecht University (Department of Information and Computing Sciences)
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class MultiplayerMenuEditTests : MonoBehaviour
{
    private MultiplayerMenuManager mm;
    
    #region Setup and Teardown

    [UnitySetUp]
    public IEnumerator Setup()
    {
        SceneManager.LoadScene("MultiplayerScreenScene");
        yield return new WaitUntil(() =>
            SceneManager.GetSceneByName("MultiplayerScreenScene").isLoaded);

        mm = GameObject.Find("MultiplayerMenuManager").GetComponent<MultiplayerMenuManager>();
    }
    
    #endregion


    /// <summary>
    /// Tests if the menu starts up correctly.
    /// </summary>
    [UnityTest]
    public IEnumerator MultiplayerStartTest()
    {
        // check if the correct canvas is active
        Assert.IsTrue(GameObject.Find("MultiplayerMenuOptions").activeSelf);
        Assert.IsTrue(GameObject.Find("HostMenuOptions") == null);
        Assert.IsTrue(GameObject.Find("JoinMenuOptions") == null);
        Assert.IsTrue(GameObject.Find("ChooseStory") == null);
        yield return null;
    }

    /// <summary>
    /// tests if the host menu is loaded correctly.
    /// </summary>
    [UnityTest]
    public IEnumerator HostGameTest()
    {
        mm.OpenHostMenu();
        // check if the correct canvas is active
        Assert.IsTrue(GameObject.Find("MultiplayerMenuOptions") == null);
        Assert.IsTrue(GameObject.Find("HostMenuOptions").activeSelf);
        Assert.IsTrue(GameObject.Find("JoinMenuOptions") == null);
        Assert.IsTrue(GameObject.Find("ChooseStory") == null);
        yield return null;
    }
    
    /// <summary>
    /// tests if the join menu is loaded correctly
    /// </summary>
    [UnityTest]
    public IEnumerator JoinGameTest()
    {
        mm.OpenJoinMenu();
        // check if the correct canvas is active
        Assert.IsTrue(GameObject.Find("MultiplayerMenuOptions") == null);
        Assert.IsTrue(GameObject.Find("HostMenuOptions") == null);
        Assert.IsTrue(GameObject.Find("JoinMenuOptions").activeSelf);
        Assert.IsTrue(GameObject.Find("ChooseStory") == null);
        yield return null;
    }
    
    /// <summary>
    /// Tests if the choose story menu is loaded correctly.
    /// </summary>
    [UnityTest]
    public IEnumerator ChooseStoryTest()
    {
        mm.OpenHostMenu();
        mm.StartAsHost();
        // check if the correct canvas is active
        Assert.IsTrue(GameObject.Find("MultiplayerMenuOptions") == null);
        Assert.IsTrue(GameObject.Find("HostMenuOptions") == null);
        Assert.IsTrue(GameObject.Find("JoinMenuOptions") == null);
        Assert.IsTrue(GameObject.Find("ChooseStory").activeSelf);
        yield return null;
    }

    /// <summary>
    /// Checks if the settings are loaded correctly.
    /// </summary>
    [UnityTest]
    public IEnumerator SettingsTest()
    {
        mm.OpenSettings();
        yield return new WaitUntil(() => SceneManager.GetSceneByName("SettingsScene").isLoaded);
        var s = SceneManager.GetActiveScene();
        
        Assert.IsTrue(SceneManager.GetSceneByName("SettingsScene").isLoaded);
        yield return null;
    }
    
}

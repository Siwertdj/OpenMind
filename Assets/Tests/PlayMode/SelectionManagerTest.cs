using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using NUnit.Framework;
using UnityEngine.SocialPlatforms.Impl;
using JetBrains.Annotations;

public class SelectionManagerTest
{
    private StoryObject story;
    private GameManager gm;
    /// <summary>
    /// Set up the game so that each test starts at the NPCSelectScene with the chosen story.
    /// </summary>
    [UnitySetUp]
    public IEnumerator SetUp()
    {
        /*
        GameObject tb = GameObject.Find("Toolbox");
        while (tb != null)
        {
            GameObject.Destroy(tb);
            tb = GameObject.Find("Toolbox");
        }
        */

        // Load StartScreenScene in order to put the SettingsManager into DDOL
        SceneManager.LoadScene("StartScreenScene");
        yield return new WaitUntil(() => SceneManager.GetSceneByName("StartScreenScene").isLoaded);

        // Move debugmanager and copyright back to startscreenscene so that 
        SceneManager.MoveGameObjectToScene(GameObject.Find("DebugManager"), SceneManager.GetSceneByName("StartScreenScene"));
        SceneManager.MoveGameObjectToScene(GameObject.Find("Copyright"), SceneManager.GetSceneByName("StartScreenScene"));

        // Unload the StartScreenScene
        SceneManager.UnloadSceneAsync("StartScreenScene");

        // Load the "Loading" scene in order to get access to the toolbox in DDOL
        SceneManager.LoadScene("Loading");
        yield return new WaitUntil(() => SceneManager.GetSceneByName("Loading").isLoaded);

        // Put toolbox as parent of SettingsManager
        GameObject.Find("SettingsManager").transform.SetParent(GameObject.Find("Toolbox").transform);

        // Get a StoryObject.
        StoryObject[] stories = Resources.LoadAll<StoryObject>("Stories");
        story = stories[0];

        gm = GameObject.Find("GameManager").GetComponent<GameManager>();

        // Start the game with the chosen story.
        gm.StartGame(null, story);

        yield return new WaitUntil(() => SceneManager.GetSceneByName("NPCSelectScene").isLoaded); // Wait for scene to load.
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


    /// <summary>
    /// Test if gamestate is set correctly
    /// </summary>
    [UnityTest]
    public IEnumerator GameStateTest()
    {
        Assert.AreEqual(GameManager.GameState.NpcSelect, gm.gameState);
        yield return null;
    }


    /// <summary>
    /// Test if enough options are generated.
    /// </summary>
    [UnityTest]
    public IEnumerator OptionsTest()
    {
        GameObject parent = GameObject.Find("Layout");
        int numOfCharacters = gm.currentCharacters.Count;

        int c = 0;
        for (int i = 0; i < 8;  i++)
        {
            if (parent.transform.GetChild(i).childCount > 0) c++;
        }

        Assert.AreEqual(numOfCharacters, c);

        yield return null;
    }

}

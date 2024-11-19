using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using TMPro.Examples;
using Unity.Collections.LowLevel.Unsafe;

public class StorySelectionManagerPlayTest
{
    private GameManager           gm;
    private StorySelectionManager sm;
    //private TimelineManager       tm; 
    
    #region Setup and Teardown
    
    [UnitySetUp]
    public IEnumerator Setup()
    {
        
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
        
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        
        gm.StartGame(null, Resources.LoadAll<StoryObject>("Stories")[0]);

        SceneManager.LoadScene("StorySelectScene");
        yield return new WaitUntil(() => SceneManager.GetSceneByName("StorySelectScene").isLoaded);
        
        sm = GameObject.Find("StorySelectionManager").GetComponent<StorySelectionManager>();
    }
    
    [TearDown]
    public void TearDown()
    {
        SceneManager.MoveGameObjectToScene(GameObject.Find("Toolbox"), SceneManager.GetSceneByName("StorySelectScene"));
        SceneController.sc.UnloadAdditiveScenes();
    }
    
    #endregion

    /// <summary>
    /// Checks if the StorySelectionManager is correctly set up. 
    /// </summary>
    [UnityTest]
    public IEnumerator StartStorySelectionManagerTest()
    {
        // Check if there are stories that can be selected
        Assert.IsNotEmpty(sm.stories); 
        yield return null;
    }
    
    /// <summary>
    /// Checks if the story in gamemanager becomes story A when A is selected
    /// </summary>
    [UnityTest]
    public IEnumerator ChooseStoryATest()
    {
        sm.StoryASelected();
        Assert.AreEqual(sm.stories[0].storyID, gm.story.storyID);
        yield return null;
    }
    
    /// <summary>
    /// Checks if the story in gamemanager becomes story B when B is selected
    /// </summary>
    [UnityTest]
    public IEnumerator ChooseStoryBTest()
    {
        sm.StoryBSelected();
        
        /*TimelineManager tm  = GameObject.Find("IntroStoryScene").GetComponent<TimelineManager>();
        tm.StartGame();*/
        
        Assert.AreEqual(sm.stories[1].storyID, gm.story.storyID);
        yield return null;
    }
    
    /// <summary>
    /// Checks if the story in gamemanager becomes story C when C is selected
    /// </summary>
    [UnityTest]
    public IEnumerator ChooseStoryCTest()
    {
        sm.StoryCSelected();
        Assert.AreEqual(sm.stories[2].storyID, gm.story.storyID);
        yield return null;
    }
    
}
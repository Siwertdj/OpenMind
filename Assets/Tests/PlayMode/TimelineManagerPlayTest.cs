using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;
using Assert = UnityEngine.Assertions.Assert;

public class TimelineManagerPlayTest
{
    private GameManager     gm;
    private TimelineManager tm;
    
    #region Setup and Teardown
    
    [UnitySetUp]
    public IEnumerator Setup()
    {
        // Start new test with clean slate. 
        foreach (var obj in GameObject.FindObjectsOfType<GameObject>())
        {
            Object.DestroyImmediate(obj);
        }
        
        // Load StartScreenScene in order to put the SettingsManager into DDOL
        SceneManager.LoadScene("StartScreenScene");
        yield return new WaitUntil(() => SceneManager.GetSceneByName("StartScreenScene").isLoaded);
        
        // Move debugmanager and copyright back to startscreenscene so that 
        SceneManager.MoveGameObjectToScene(GameObject.Find("DebugManager"),
            SceneManager.GetSceneByName("StartScreenScene"));
        SceneManager.MoveGameObjectToScene(GameObject.Find("Copyright"),
            SceneManager.GetSceneByName("StartScreenScene"));
        
        // Unload the StartScreenScene
        SceneManager.UnloadSceneAsync("StartScreenScene");
        
        // Load the "Loading" scene in order to get access to the toolbox in DDOL
        SceneManager.LoadScene("Loading");
        yield return new WaitUntil(() => SceneManager.GetSceneByName("Loading").isLoaded);
        
        // Put toolbox as parent of SettingsManager
        GameObject.Find("SettingsManager").transform
            .SetParent(GameObject.Find("Toolbox").transform);
        
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        
        gm.StartGame(null, Resources.LoadAll<StoryObject>("Stories")[0]);
        
        SceneManager.LoadScene("IntroStoryScene");
        yield return new WaitUntil(() => SceneManager.GetSceneByName("IntroStoryScene").isLoaded);
        
        tm = GameObject.Find("TimelineManager").GetComponent<TimelineManager>();
    }
    
    [TearDown]
    public void TearDown()
    {
        SceneManager.MoveGameObjectToScene(GameObject.Find("Toolbox"),
            SceneManager.GetSceneByName("IntroStoryScene"));
        SceneController.sc.UnloadAdditiveScenes();
    }
    
    #endregion
    
    /// <summary>
    /// Checks some basic properties of the introduction. 
    /// </summary>
    [UnityTest]
    public IEnumerator StoryIntroSetUpTest()
    {
        // Lists containing necessary elements should not be empty
        Assert.AreNotEqual(0, tm.backgrounds.Length);
        Assert.AreNotEqual(0, tm.storyText.Length);
        //Assert.AreNotEqual(0, tm.textMessages.Length);
        Assert.AreNotEqual(0, tm.messageLocations.Length);
        Assert.AreNotEqual(0, tm.typingTexts.Length);
        yield return null;
    }
    
    /// <summary>
    /// Checks if the introduction can be played. 
    /// </summary>
    [UnityTest]
    public IEnumerator PlayIntroTest()
    {
        tm.currentTimeline = tm.introStoryA; 
        tm.ContinueCurrentTimeline();
        Assert.AreEqual(PlayState.Playing,tm.currentTimeline.state);
        // When the introduction is playing the continuebutton should not be visible.
        Assert.IsFalse(tm.continueButton.activeSelf);
        yield return null;
    }
    
    /// <summary>
    /// Checks if the introduction can be paused. 
    /// </summary>
    [UnityTest]
    public IEnumerator PauseTutorialTest()
    {
        tm.currentTimeline = tm.introStoryA; 
        tm.PauseCurrentTimeline();
        Assert.AreEqual(PlayState.Paused,tm.currentTimeline.state);
        // When the introduction is playing the continuebutton should be visible.
        Assert.IsTrue(tm.continueButton.activeSelf);
        yield return null;
    }
    
    /// <summary>
    /// Checks if the background is actually updated.  
    /// </summary>
    [UnityTest]
    public IEnumerator ChangeBackgroundTest()
    {
        tm.currentTimeline = tm.introStoryA; 
        Sprite background = tm.background.sprite;
        tm.ChangeBackground();
        // Check whether the text changed. 
        Assert.AreNotEqual(background, tm.background.sprite);
        yield return null;
    }
}
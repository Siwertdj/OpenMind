using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;
using Assert = UnityEngine.Assertions.Assert;

public class IntroductionManagerPlayTest
{
    private GameManager     gm;
    private IntroductionManager im;
    
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
        
        im = GameObject.Find("IntroductionManager").GetComponent<IntroductionManager>();
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
        Assert.AreNotEqual(0, im.backgrounds.Length);
        Assert.AreNotEqual(0, im.storyText.Length);
        //Assert.AreNotEqual(0, tm.textMessages.Length);
        Assert.AreNotEqual(0, im.messageLocations.Length);
        yield return null;
    }
    
    /// <summary>
    /// Checks if the introduction can be played. 
    /// </summary>
    [UnityTest]
    public IEnumerator PlayIntroTest()
    {
        im.currentTimeline = im.introStoryA; 
        im.ContinueCurrentTimeline();
        Assert.AreEqual(PlayState.Playing,im.currentTimeline.state);
        // When the introduction is playing the continuebutton should not be visible.
        Assert.IsFalse(im.continueButton.activeSelf);
        yield return null;
    }
    
    /// <summary>
    /// Checks if the introduction can be paused. 
    /// </summary>
    [UnityTest]
    public IEnumerator PauseTutorialTest()
    {
        im.currentTimeline = im.introStoryA; 
        im.PauseCurrentTimeline();
        Assert.AreEqual(PlayState.Paused,im.currentTimeline.state);
        // When the introduction is playing the continuebutton should be visible.
        Assert.IsTrue(im.continueButton.activeSelf);
        yield return null;
    }
    
}
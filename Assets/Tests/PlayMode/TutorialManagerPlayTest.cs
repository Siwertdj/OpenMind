using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;
using Assert = UnityEngine.Assertions.Assert;

public class TutorialManagerPlayTest
{
    private GameManager     gm;
    private TutorialManager tm; 
        
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

        SceneManager.LoadScene("TutorialScene");
        yield return new WaitUntil(() => SceneManager.GetSceneByName("TutorialScene").isLoaded);
        
        tm = GameObject.Find("TutorialManager").GetComponent<TutorialManager>();
    }
    
    [TearDown]
    public void TearDown()
    {
        SceneManager.MoveGameObjectToScene(GameObject.Find("Toolbox"), SceneManager.GetSceneByName("TutorialScene"));
        SceneController.sc.UnloadAdditiveScenes();
    }
    
    #endregion
    
    /// <summary>
    /// Checks if the tutorial is correctly set up when it is started. 
    /// </summary>
    [UnityTest]
    public IEnumerator StartTutorialTest()
    {
        // Check whether the tutorial contains text. 
        Assert.AreNotEqual(0, tm.tutorialText.Length);
        tm.StartTutorial();
        // When the tutorial is started it should start at the beginning. 
        Assert.AreEqual(0,tm.TutorialTimeline.time);
        // When the tutorial is started it should be playing. 
        Assert.AreEqual(PlayState.Playing,tm.TutorialTimeline.state);
        yield return null;
    }
    
    /// <summary>
    /// Checks if the tutorial can be played. 
    /// </summary>
    [UnityTest]
    public IEnumerator PlayTutorialTest()
    {
        tm.PlayTutorial();
        Assert.AreEqual(PlayState.Playing,tm.TutorialTimeline.state);
        // When the tutorial is playing the continuebutton should not be visible.
        Assert.IsFalse(tm.continueButton.IsActive());
        yield return null;
    }
    
    /// <summary>
    /// Checks if the tutorial can be paused. 
    /// </summary>
    [UnityTest]
    public IEnumerator PauseTutorialTest()
    { 
        tm.PauseTutorial();
        Assert.AreEqual(PlayState.Paused,tm.TutorialTimeline.state);
        // When the tutorial is playing the continuebutton should be visible.
        Assert.IsTrue(tm.continueButton.IsActive());
        yield return null;
    }
    
    /// <summary>
    /// Checks if the tutorial can be played. 
    /// </summary>
    [UnityTest]
    public IEnumerator UpdateTutorialTest()
    {
        string text = tm.text.text;
        tm.PlayTutorial();
        tm.UpdateTutorialText(); // Default text is the same as the first text, so we need to update 
        tm.UpdateTutorialText(); // the text twice in order to properly test this aspect.
        Assert.AreEqual(PlayState.Paused,tm.TutorialTimeline.state);
        // Check whether the text changed. 
        Assert.AreNotEqual(text, tm.text.text);
        yield return null;
    }
    
}

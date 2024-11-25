using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public class StorySelectionManagerPlayTest
{
    private GameManager           gm;
    private StorySelectionManager sm;
    
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
        //SceneManager.UnloadSceneAsync("StartScreenScene");
        
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
    /// Checks if the story becomes story A when A is selected
    /// </summary>
    [UnityTest]
    public IEnumerator ChooseStoryATest()
    {
        sm.StoryASelected(); // This method also loads the introduction scene.
        yield return new WaitUntil(() => SceneManager.GetSceneByName("IntroStoryScene").isLoaded);
        // In TimelineManager the introduction is determined by the StorySelect scene.
        TimelineManager tm = GameObject.Find("TimelineManager").GetComponent<TimelineManager>();
        // We therefore check if the loaded introduction is indeed the correct one. 
        Assert.AreEqual(tm.introStoryA,tm.currentTimeline);
        //Assert.AreEqual(sm.stories[0].storyID, gm.story.storyID);
        yield return null;
    }
    
    /// <summary>
    /// Checks if the story becomes story B when B is selected
    /// </summary>
    [UnityTest]
    public IEnumerator ChooseStoryBTest()
    {
        // This test works exactly the same as ChooseStoryATest
        sm.StoryBSelected();
        yield return new WaitUntil(() => SceneManager.GetSceneByName("IntroStoryScene").isLoaded);
         
        TimelineManager tm = GameObject.Find("TimelineManager").GetComponent<TimelineManager>();
        
        Assert.AreEqual(tm.introStoryB,tm.currentTimeline);
        yield return null;
    }
    
    /// <summary>
    /// Checks if the story becomes story C when C is selected
    /// </summary>
    [UnityTest]
    public IEnumerator ChooseStoryCTest()
    {
        // This test works exactly the same as ChooseStoryATest
        sm.StoryCSelected();
        yield return new WaitUntil(() => SceneManager.GetSceneByName("IntroStoryScene").isLoaded);
        
        TimelineManager tm = GameObject.Find("TimelineManager").GetComponent<TimelineManager>();
        
        Assert.AreEqual(tm.introStoryC, tm.currentTimeline);
        yield return null;
    }
    
}
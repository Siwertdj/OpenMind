using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

/// <summary>
/// A class that tests scene controller properties:
/// - Make sure the transition graph can be read, that it throws no errors when reading
/// - UnloadAdditiveScenes should unload all scenes except the loading scene
/// - An invalid scene transition should give an error
/// - A valid scene transition should actual transition the scene 
/// </summary>
public class SceneControllerTests
{
    
    [OneTimeSetUp]
    public void CreateLoadingScene()
    {
        SceneManager.LoadScene("Loading", LoadSceneMode.Additive);
    }
    
    //all possible scene names
    private static SceneController.SceneName[] sceneNames = 
        Enum.GetValues(typeof(SceneController.SceneName)).Cast<SceneController.SceneName>().ToArray();
    
    //exclude npcselect and notebook, because loading these gives errors
    private static SceneController.SceneName[] filteredSceneNames() => Array.FindAll(sceneNames,
        sn => sn != SceneController.SceneName.NPCSelectScene &&
              sn != SceneController.SceneName.NotebookScene);
    
    private static SceneController.TransitionType[] transitionTypes =
        Enum.GetValues(typeof(SceneController.TransitionType)).Cast<SceneController.TransitionType>().ToArray();
    
    
    /// <summary>
    /// Tests whether the scene graph reading works and no errors are thrown.
    /// Also tests whether the right scene gets loaded from SceneController.StartScene, since both properties are tested with this method.
    /// </summary>
    [UnityTest, Order(1)]
    public IEnumerator TestSceneGraphReading([ValueSource(nameof(filteredSceneNames))] SceneController.SceneName sceneName)
    {
        //creates scene graph
        SceneController.sc.StartScene(sceneName);
        yield return new WaitForSeconds(0.1f);
        
        //checks if the start scene is loaded
        Assert.IsTrue(SceneManager.GetSceneByName(sceneName.ToString()).isLoaded);
        SceneManager.UnloadSceneAsync(sceneName.ToString());
    }
    
    /// <summary>
    /// Tests whether unloading all scenes unloads all scenes
    /// </summary>
    [UnityTest, Order(2)]
    public IEnumerator TestUnloadAdditiveScenes()
    {
        //load 2 scenes
        SceneManager.LoadScene("StartScreenScene", LoadSceneMode.Additive);
        SceneManager.LoadScene("GameWinScene", LoadSceneMode.Additive);
        
        yield return new WaitUntil(() =>
            SceneManager.GetSceneByName("StartScreenScene").isLoaded &&
            SceneManager.GetSceneByName("GameWinScene").isLoaded);
        
        //check if the 3 scenes are loaded, including the loading scene, an init test scene is created when running tests, so this scene counts as well, making the total count 4
        Assert.AreEqual(4, SceneManager.loadedSceneCount);
        
        //unload all scenes
        SceneController.sc.UnloadAdditiveScenes();
        
        //check if more than 1 scene is loaded, the loading scene should still be loaded
        Assert.AreEqual(1, SceneManager.loadedSceneCount);
    }
    
    /// <summary>
    /// Tests whether an invalid scene transition throws an error.
    /// Tests whether transitioning from an unloaded scene throws an error
    /// </summary>
    [UnityTest, Order(3)]
    public IEnumerator TestInvalidSceneTransition(
        [ValueSource(nameof(filteredSceneNames))] SceneController.SceneName from,
        [ValueSource(nameof(filteredSceneNames))] SceneController.SceneName to,
        [ValueSource(nameof(transitionTypes))] SceneController.TransitionType tt
        )
    {
        if (typeof(SceneController).GetField("sceneGraph", BindingFlags.NonPublic | BindingFlags.Instance) is null)
            SceneController.sc.StartScene(from);
        else
            SceneManager.LoadScene(from.ToString());
        
        yield return new WaitUntil(() => SceneManager.GetSceneByName(from.ToString()).isLoaded);
        
        List<List<(int, SceneController.TransitionType)>> sceneGraph =
            typeof(SceneController).GetField("sceneGraph", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(SceneController.sc) as List<List<(int, SceneController.TransitionType)>>;
        Dictionary<string, int> sceneToID =
            typeof(SceneController).GetField("sceneToID", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(SceneController.sc) as Dictionary<string, int>;
        
        
        
        
        //check if an error is thrown when transitioning from an unloaded scene
        if (!SceneManager.GetSceneByName(to.ToString()).isLoaded)
        {
            Task task1 = SceneController.sc.TransitionScene(to, from, tt);
            yield return new WaitUntil(() => task1.IsCompleted);
            LogAssert.Expect(LogType.Error,
                $"Cannot make a transition from the scene {to}, since it's not loaded.");
        }
        
        //checks whether the scene is invalid, if it valid, nothing is tested
        int fromID = sceneToID[from.ToString()];
        int toID = sceneToID[to.ToString()];
        if (!sceneGraph[fromID].Contains((toID, tt)))
        {
            //invalid transition
            LogAssert.Expect(LogType.Error,
                $"Current scene {from} cannot make a {tt}-transition to {to}");
            
            Task task2 = SceneController.sc.TransitionScene(from, to, tt);
            yield return new WaitUntil(() => task2.IsCompleted);
        }
        
        SceneManager.UnloadSceneAsync(from.ToString());
    }
    
    /// <summary>
    /// Tests whether a valid scene transition results in a new scene
    /// </summary>
    [UnityTest, Order(3)]
    public IEnumerator TestValidSceneTransition(
        [ValueSource(nameof(filteredSceneNames))] SceneController.SceneName from,
        [ValueSource(nameof(filteredSceneNames))] SceneController.SceneName to,
        [ValueSource(nameof(transitionTypes))] SceneController.TransitionType tt
    )
    {
        if (typeof(SceneController).GetField("sceneGraph", BindingFlags.NonPublic | BindingFlags.Instance) is null)
            SceneController.sc.StartScene(from);
        else
            SceneManager.LoadScene(from.ToString());
        
        yield return new WaitUntil(() => SceneManager.GetSceneByName(from.ToString()).isLoaded);
        
        List<List<(int, SceneController.TransitionType)>> sceneGraph =
            typeof(SceneController).GetField("sceneGraph", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(SceneController.sc) as List<List<(int, SceneController.TransitionType)>>;
        Dictionary<string, int> sceneToID =
            typeof(SceneController).GetField("sceneToID", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(SceneController.sc) as Dictionary<string, int>;
        
        //check if the transition is valid, if not, nothing will be tested
        int fromID = sceneToID[from.ToString()];
        int toID = sceneToID[to.ToString()];
        if (sceneGraph[fromID].Contains((toID, tt)))
        {
            Task task = SceneController.sc.TransitionScene(from, to, tt);
            yield return new WaitUntil(() => task.IsCompleted);
            
            switch (tt)
            {
                case SceneController.TransitionType.Additive:
                    Assert.IsTrue(SceneManager.GetSceneByName(from.ToString()).isLoaded);
                    Assert.IsTrue(SceneManager.GetSceneByName(to.ToString()).isLoaded);
                    SceneManager.UnloadSceneAsync(from.ToString());
                    break;
                
                case SceneController.TransitionType.Transition:
                    Assert.IsFalse(SceneManager.GetSceneByName(from.ToString()).isLoaded);
                    Assert.IsTrue(SceneManager.GetSceneByName(to.ToString()).isLoaded);
                    break;
                
                case SceneController.TransitionType.Unload:
                    Assert.IsFalse(SceneManager.GetSceneByName(from.ToString()).isLoaded);
                    Assert.IsTrue(SceneManager.GetSceneByName(to.ToString()).isLoaded);
                    break;
                
                default:
                    throw new Exception($"There is no test for this type of transition yet: {tt}");
            }
        }
        
        SceneManager.UnloadSceneAsync(to.ToString());
    }
}

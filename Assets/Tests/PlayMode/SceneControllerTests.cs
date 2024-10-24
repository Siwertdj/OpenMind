using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
public class SceneControllerTests : MonoBehaviour
{
    [OneTimeSetUp]
    public void CreateLoadingScene()
    {
        SceneManager.LoadScene("Loading");
    }
    
    
    //all possible scene names
    static SceneController.SceneName[] sceneNames = Enum.GetValues(typeof(SceneController.SceneName)).Cast<SceneController.SceneName>().ToArray();

    /// <summary>
    /// Tests whether the scene graph reading works and no errors are thrown.
    /// Also tests whether the right scene gets loaded from SceneController.StartScene, since both properties are tested with this method.
    /// </summary>
    [UnityTest]
    public IEnumerator TestSceneGraphReading([ValueSource(nameof(sceneNames))] SceneController.SceneName sceneName)
    {
        //creates scene graph
        SceneController.sc.StartScene(sceneName);
        
        //checks if the start scene is loaded
        Assert.IsTrue(SceneManager.GetSceneByName(sceneName.ToString()).IsValid());
        yield return null;
    }
    
    /// <summary>
    /// Tests whether unloading all scenes unloads all scenes
    /// </summary>
    [UnityTest]
    public IEnumerator TestUnloadAdditiveScenes()
    {
        //load all scenes first
        foreach (var scene in sceneNames)
            SceneManager.LoadScene(scene.ToString(), LoadSceneMode.Additive);
        
        //unload all scenes
        SceneController.sc.UnloadAdditiveScenes();
        
        //check if more than 1 scene is loaded
        Assert.IsTrue(SceneManager.loadedSceneCount == 1);
        
        yield return null;
    }
}

// This program has been developed by students from the bachelor Computer Science at Utrecht University within the Software Project course.
// © Copyright Utrecht University (Department of Information and Computing Sciences)
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
using UnityEngine.EventSystems;
using Random = System.Random;

/// <summary>
/// A class that tests scene controller properties:
/// - Make sure the transition graph can be read, that it throws no errors when reading
/// - An invalid scene transition should give an error
/// - A valid scene transition should actual transition the scene 
/// </summary>
public class SceneControllerTests
{
    private GameManager     gm;
    private SceneController sc;
    
    //all possible scene names
    private static SceneController.SceneName[] sceneNames =
        Enum.GetValues(typeof(SceneController.SceneName)).Cast<SceneController.SceneName>().ToArray();
    
    private static SceneController.TransitionType[] transitionTypes =
        Enum.GetValues(typeof(SceneController.TransitionType)).Cast<SceneController.TransitionType>().ToArray();
    
    [OneTimeSetUp]
    public void LoadTestingScene()
    {
        SceneManager.LoadScene("TestingScene");
    }
    
    [OneTimeTearDown]
    public void UnloadTestingScene()
    {
        SceneManager.UnloadSceneAsync("TestingScene");
    }
    
    /// <summary>
    /// Sets up the unit tests:
    /// - Disables event systems
    /// - Disables audio listeners
    /// - Initialises Gamemanager.gm.currentCharacters so loading NPCSelectScene doesn't throw an error
    /// - Initialises Gamemanager.gm.story (through reflection) so loading NPCSelectScene doesn't throw an error
    /// - Initialises Gamemanager.gm.notebookData so loading NotebookScene doesn't throw an error
    /// </summary>
    [UnitySetUp]
    public IEnumerator SetupUnitTest()
    {
        SceneManager.LoadSceneAsync("Loading", LoadSceneMode.Additive);
        
        yield return new WaitUntil(() => SceneManager.GetSceneByName("Loading").isLoaded);
        
        DisbleAllEventAndAudioListeners();
        GameManager.gm.currentCharacters = new List<CharacterInstance>();
        CharacterData dummyData = ScriptableObject.CreateInstance<CharacterData>();
        dummyData.answers = Array.Empty<KeyValuePair>();
        CharacterInstance dummy = new CharacterInstance(dummyData);
        GameManager.gm.currentCharacters.AddRange(new[] {dummy, dummy, dummy, dummy});
        SetProperty("story", GameManager.gm, ScriptableObject.CreateInstance<StoryObject>());
        
        GameManager.gm.notebookData = new NotebookData();
        
        gm = GameManager.gm;
        sc = SceneController.sc;
        
        //unload the loading scene now
        SceneManager.UnloadSceneAsync("Loading");
    }
    
    [UnityTearDown]
    public IEnumerator TearDownUnitTests()
    {
        yield return new WaitUntil(() =>
        {
            SceneManager.UnloadSceneAsync(
                SceneManager.GetSceneAt(SceneManager.loadedSceneCount - 1));
            
            return SceneManager.loadedSceneCount == 1;
        });
    }
    
    /// <summary>
    /// Disables all event systems and audio systems to prevent the debug spam of having multiple of these systems.
    /// </summary>
    private void DisbleAllEventAndAudioListeners()
    {
        //disable event systems to prevent debug spam
        EventSystem[] eventSystems = UnityEngine.Object.FindObjectsOfType<EventSystem>();
        foreach(EventSystem eventSystem in eventSystems)
            eventSystem.enabled = false;
        
        //disable event systems to prevent debug spam
        AudioListener[] audioSystems = UnityEngine.Object.FindObjectsOfType<AudioListener>();
        foreach(AudioListener audioSource in audioSystems)
            audioSource.enabled = false;
    }
    
    private void GetValue<TV, TC>(string varName, TC instance, out TV variable) where TV : class
    {
        FieldInfo fieldInfo =
            typeof(TC).GetField(varName, BindingFlags.NonPublic | BindingFlags.Instance);
        variable = fieldInfo.GetValue(instance) as TV;
    }
    
    private void SetProperty<TV, TC>(string varName, TC instance, TV value)
    {
        PropertyInfo propertyInfo = typeof(TC).GetProperty(varName);
        propertyInfo.SetValue(instance, value, null);
    }
    
    /// <summary>
    /// Tests whether the scene graph reading works and no errors are thrown.
    /// Also tests whether the right scene gets loaded from SceneController.StartScene, since both properties are tested with this method.
    /// </summary>
    [UnityTest, Order(1)]
    public IEnumerator TestSceneGraphReading([ValueSource(nameof(sceneNames))] SceneController.SceneName sceneName)
    {
        //creates scene graph
        sc.StartScene(sceneName);
        int i = 0;
        const int timeout = 1000;
        bool finished = true;
        
        //wait for the scene to load or a timeout. If the timeout is hit, assume the scene never loaded.
        yield return new WaitUntil(() =>
        {
            i = i++;
            if (i > timeout)
            {
                finished = false;
                return true;
            }
            
            return SceneManager.GetSceneByName(sceneName.ToString()).isLoaded;
        });
        
        Assert.IsTrue(finished);
    }
    
    /// <summary>
    /// Tests whether an invalid scene transition throws an error.
    /// Tests whether transitioning from an unloaded scene throws an error
    /// </summary>
    [UnityTest, Order(3)]
    public IEnumerator TestSceneTransitionValidity(
        [ValueSource(nameof(sceneNames))] SceneController.SceneName from,
        [ValueSource(nameof(sceneNames))] SceneController.SceneName to,
        [ValueSource(nameof(transitionTypes))] SceneController.TransitionType tt
        )
    {
        GetValue("sceneGraph", sc, out object value1);
        GetValue("sceneToID", sc, out object value2);
        if (value1 is null || value2 is null)
            sc.StartScene(from);
        else
            SceneManager.LoadScene(from.ToString(), LoadSceneMode.Additive);
        
        yield return new WaitUntil(() => SceneManager.GetSceneByName(from.ToString()).isLoaded);
        
        //reassign Gamemanager.gm, since loading the Loading scene overwrites it
        if (from == SceneController.SceneName.Loading)
            GameManager.gm = gm;
        
        List<List<(int, SceneController.TransitionType)>> sceneGraph;
        Dictionary<string, int> sceneToID;
        
        GetValue("sceneGraph", sc, out sceneGraph);
        GetValue("sceneToID", sc, out sceneToID);
        
        //check if an error is thrown when transitioning from an unloaded scene
        if (!SceneManager.GetSceneByName(to.ToString()).isLoaded)
        {
            LogAssert.Expect(LogType.Error,
                $"Cannot make a transition from the scene {to}, since it's not loaded.");
            
            Task task1 = sc.TransitionScene(to, from, tt);
            yield return new WaitUntil(() => task1.IsCompleted);
            
            Assert.IsFalse(DidTransitionHappen(from, to));
        }

        //checks whether the scene is invalid, if it is valid, check if the right transition happened
        int fromID = sceneToID[from.ToString()];
        int toID = sceneToID[to.ToString()];
        if (sceneGraph[fromID].Contains((toID, tt)))
        {
            Debug.Log(SceneManager.GetSceneByName(to.ToString()).isLoaded);
            
            if (tt == SceneController.TransitionType.Unload)
            {
                //if the transition is an unload, make sure the to is loaded
                SceneManager.LoadScene(to.ToString(), LoadSceneMode.Additive);
                yield return new WaitUntil(
                    () => SceneManager.GetSceneByName(to.ToString()).isLoaded);
            }
            
            //do the transition
            Task task = sc.TransitionScene(from, to, tt);
            yield return new WaitUntil(() => task.IsCompleted);
            
            //the transition has different results based on the type of transition
            switch (tt)
            {
                case SceneController.TransitionType.Additive:
                    Assert.IsTrue(SceneManager.GetSceneByName(from.ToString()).isLoaded);
                    Assert.IsTrue(SceneManager.GetSceneByName(to.ToString()).isLoaded);
                    break;
                
                case SceneController.TransitionType.Transition:
                    //edge case for when to == from
                    if (to != from)
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
        else
        {
            //invalid transition
            LogAssert.Expect(LogType.Error,
                $"Current scene {from} cannot make a {tt}-transition to {to}");
            
            Task task2 = sc.TransitionScene(from, to, tt);
            yield return new WaitUntil(() => task2.IsCompleted);
            
            Assert.IsFalse(DidTransitionHappen(from, to));
        }
    }
    
    /// <summary>
    /// Tests whether a transition occured
    /// Assumes:
    /// from was active
    /// to was not active
    /// Loading was active
    /// </summary>
    private bool DidTransitionHappen(SceneController.SceneName from, SceneController.SceneName to)
    {
        //if from == to, from should be loaded, which means to is also loaded and no transition happened
        if (from == to)
            return !SceneManager.GetSceneByName(from.ToString()).isLoaded;
        
        bool transitionHappened = false; 
        transitionHappened |= SceneManager.GetSceneByName(to.ToString()).isLoaded;
        
        //if from is not active, a transition happened
        transitionHappened |= !SceneManager.GetSceneByName(from.ToString()).isLoaded;
        return transitionHappened;
    }
}

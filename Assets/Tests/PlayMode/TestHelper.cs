using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestHelper : MonoBehaviour
{
    public static TestHelper th;
    private       bool       isRunning;
    
    private void Awake()
    {
        th = this;
        DontDestroyOnLoad(gameObject);
    }
    
    /// <summary>
    /// Can be used as a coroutine to load a scene
    /// </summary>
    private IEnumerator LoadSceneCoroutine(string scene, TaskCompletionSource<bool> tcs)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
        
        while (!asyncLoad.isDone)
            yield return null;
        
        tcs.SetResult(true);
    }
    
    public Task LoadScene(string scene)
    {
        var tcs = new TaskCompletionSource<bool>();
        
        StartCoroutine(LoadSceneCoroutine(scene, tcs));
        
        return tcs.Task;
    }
    
    public bool Await(Action action, Func<bool> completeCondition)
    {
        if (!isRunning)
        {
            isRunning = true;
            action();
        }
        
        return completeCondition();
    }
    
    #region helperFunctions
    
    /// <summary>
    /// -1 for not counting the test scene
    /// returns the amount of loaded scenes
    /// </summary>
    public int LoadedSceneCount()
    {
        int count = 0;
        for (int i = 0; i < SceneManager.sceneCount; i++)
            if (SceneManager.GetSceneAt(i).name != "TestScene" && SceneManager.GetSceneAt(i).isLoaded)
                count++;

        return count;
    }
    
    /// <summary>
    /// Returns the scene at index i, ignoring the test scene.
    /// So inputting "-1" would result in the test scene.
    /// </summary>
    public Scene GetSceneAt(int i)
    {
        return SceneManager.GetSceneAt(i + 1);
    }
    
    #endregion
}

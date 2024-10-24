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
    
    #endregion
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController sc;
    private bool notebookOn = false;

    public void Awake()
    {
        sc = this;
    }

    public void UnloadAdditiveScenes()
    {
        //Get the story scene
        Scene loadingScene = SceneManager.GetSceneByName("Loading");

        // Unload all loaded scenes that are not the story scene
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene loadedScene = SceneManager.GetSceneAt(i);
            if (loadedScene != loadingScene) SceneManager.UnloadSceneAsync(loadedScene.name);
        }
    }
    
    
    public void UnloadDialogueScene()
    {
        string sceneName = "DialogueScene";
        if (SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            SceneManager.UnloadSceneAsync(sceneName);
        }
        else
        {
            Debug.Log("Dialogue scene not loaded");
        }
    }
    
    public void ToggleCompanionHintScene()
    {
        string sceneName = "Companion Hint";
        if (SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            SceneManager.UnloadSceneAsync(sceneName);
        }
        else
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        }
    }

    public void ToggleNPCSelectScene()
    {
        string sceneName = "NPCSelectScene";
        if (SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            SceneManager.UnloadSceneAsync((sceneName));
        }
        else
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        }
    }

    
    public void ToggleGameOverScene()
    {
        if (SceneManager.GetSceneByName("GameOverScene").isLoaded)
        {
            SceneManager.UnloadSceneAsync("GameOverScene");
        }
        else
        {
            SceneManager.LoadScene("GameOverScene", LoadSceneMode.Additive);
        }
    }
    

    public void ToggleNotebookScene()
    {
        if (notebookOn)
        {

            SceneManager.UnloadSceneAsync("NotebookScene");
            notebookOn = false;
        }
        else
        {
            SceneManager.LoadScene("NotebookScene", LoadSceneMode.Additive);
            notebookOn= true;
        }
    }
}

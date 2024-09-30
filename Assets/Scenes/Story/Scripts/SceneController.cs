using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController sc;
    private bool notebookOn = false;

    // possibly a bad solution
    public AsyncOperation DialogueSceneOp;

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

    public void ToggleDialogueScene()
    {
        string sceneName = "DialogueScene";

        if (SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            SceneManager.UnloadSceneAsync(sceneName);
        }
        else
        {
            DialogueSceneOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
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
    public void ToggleGameWinScene()
    {
        if (SceneManager.GetSceneByName("GameWinScene").isLoaded)
        {
            SceneManager.UnloadSceneAsync("GameWinScene");
        }
        else
        {
            SceneManager.LoadScene("GameWinScene", LoadSceneMode.Additive);
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

public abstract class AbstractScene : MonoBehaviour
{
    //read from a file
    private static List<List<int>> sceneGraph;

    //inferred from reading the same file, what scene is matched to what id doesn't matter, as long as they are all assigned to a unique ID
    private static Dictionary<string, int> sceneToID;

    //the scene which loaded the current scene
    private AbstractScene parentScene = null;
    
    
    //an abstract method so the scene can receive variables, this can be left empty when override if a scene doesn't need variables
    protected abstract void GetVariables(params object[] args);

    //read the scene graph from the file and assign both vars described above
    private void ReadSceneGraph()
    {
        //some file reading code
    }

    //transitions to a new scene
    //conditions: current = true, means current is loaded. current = false, means current is unloaded
    //pre conditions: current
    //post conditions: current = !target_pre && target_post
    private void Transitioning(string currentScene, string targetScene)
    {
        //target scene is loaded, meaning we want to unload the current scene
        if (SceneManager.GetSceneByName(targetScene).isLoaded)
        {
            SceneManager.UnloadSceneAsync(currentScene);
        }
        else
        {
            SceneManager.LoadScene(targetScene, LoadSceneMode.Additive);
        }
    }
    
    //if a scene really wants to determine the transition itself, this method can be directly called to override the transition code
    protected void TransitionScene<T1, T2>(Action<string, string> loadCode, params object[] args)
        where T1 : AbstractScene
        where T2 : AbstractScene
    {
        string currentScene = typeof(T1).Name;
        string targetScene = typeof(T2).Name;
        
        //some extra checks will be made about the validity of the variables and the file contents
        //for example, making a new scene, but forgetting to put it into the scene graph file, should result in an error here.
        
        //checks, does currentScene point to nextScene in the graph?
        int currentSceneID = sceneToID[currentScene];
        int targetSceneID = sceneToID[targetScene];
        if (!sceneGraph[currentSceneID].Contains(targetSceneID))
            //invalid transition
            throw new Exception();
        
        //if it's an unload (the target scene is loaded), check if the target scene is the parent scene of this scene
        //otherwise with unloading a scene, a scene can be selected in a such a way as to always allow an unload.
        if (SceneManager.GetSceneByName(targetScene).isLoaded &&
            SceneManager.GetSceneAt(SceneManager.sceneCount - 2).name != targetScene)
            throw new Exception();
        
        loadCode(currentScene, targetScene);
        //assign variables to the target scene
        FindObjectOfType<T2>().GetVariables(args);
    }

    
    //args is the data to transfer
    protected void TransitionScene<T1, T2>(params object[] args)
        where T1 : AbstractScene
        where T2 : AbstractScene
        => TransitionScene<T1, T2>(Transitioning, args);

    
    //the function to be called when loading the first cycle
    public void StartScene<T>(params object[] args) where T : AbstractScene
    {
        ReadSceneGraph();
        
        string currentScene = typeof(T).Name;
        SceneManager.LoadScene(currentScene, LoadSceneMode.Additive);
        GetVariables(args);
    }
}

//an example of a scene class below:
public class DialogueScene : AbstractScene
{
    private string characterToTalkTo;
    //this scene should get a variable about who to talk to
    protected override void GetVariables(params object[] args)
    {
        //assign variables
        characterToTalkTo = args[0].ToString();
    }

    //a method that loads a new scene with some variables, assuming CompanionHint is currently unloaded
    private void SomeMethod1()
    {
        TransitionScene<DialogueScene, CompanionHint>(1, 2);
    }
}

public class CompanionHint : AbstractScene
{
    protected override void GetVariables(params object[] args)
    {
        
    }
}
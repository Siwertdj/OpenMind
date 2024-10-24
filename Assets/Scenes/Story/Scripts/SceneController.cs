using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;
using File = System.IO.File;

public class SceneController : MonoBehaviour
{
    public enum SceneName
    {
        StartScreenScene,
        NPCSelectScene,
        DialogueScene,
        GameOverScene,
        GameWinScene,
        Loading,
        NotebookScene,
        EpilogueScene
    }

    public enum TransitionType
    {
        Transition,
        Additive,
        Unload
    }
    
    public static SceneController sc;
    
    //read from a file
    private List<List<(int, TransitionType)>> sceneGraph;

    //inferred from reading the same file, what scene is matched to what id doesn't matter, as long as they are all assigned to a unique ID
    private Dictionary<string, int> sceneToID;

    private const string TransitionGraphLocation = "Transition Graph/Transition Graph";
    private string GetTransitionGraphFilePath() => Path.Combine(Application.dataPath, "../Assets/Resources/") + TransitionGraphLocation;

    public void Awake()
    {
        // Initializes static instance of SceneController.
        sc = this;
    }

    /// <summary>
    /// Unloads all scenes(as all are opened additively), other than the 'Loading' scene.
    /// </summary>
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

    //read the scene graph from the file and assign both vars described above
    private void ReadSceneGraph()
    {
        // Load the scene graph file
        TextAsset file = (TextAsset)Resources.Load(TransitionGraphLocation);

        // Check if the file exists
        if (file == null)
        {
            Debug.LogError("Couldn't read the scene graph, the file on filepath " +
                $"Assets/Resources/{TransitionGraphLocation} was not found.");
            return;
        }

        // Split into lines
        string[] fileGraphContentLines = Regex.Split(file.text, "\r\n|\r|\n");

        sceneGraph = new List<List<(int, TransitionType)>>(fileGraphContentLines.Length);
        sceneToID = new Dictionary<string, int>();
        
        //example: NPCSelectScene --> DialogueScene(T), NotebookScene(A), GameOverScene(T), GameWonScene(T)
        const string arrowSeparator = " --> ";
        const string sceneSeparator = ", ";
        foreach(string fileGraphContentLine in fileGraphContentLines)
            sceneToID.Add(fileGraphContentLine.Split(arrowSeparator)[0], sceneToID.Count);
        
        for (int i = 0; i < fileGraphContentLines.Length; i++)
        {
            string[] fromTo = fileGraphContentLines[i].Split(arrowSeparator);
            string[] tos = fromTo[1].Split(sceneSeparator);
            
            sceneGraph.Add(new List<(int, TransitionType)>());

            foreach (string to in tos)
            {
                string toScene = to.Substring(0, to.Length - 3);
                foreach (TransitionType enumValue in Enum.GetValues(typeof(TransitionType)))
                    if (enumValue.ToString()[0] == to[^2])
                    {
                        sceneGraph[i].Add((sceneToID[toScene], enumValue));
                        break;
                    }
            }
        }
    }

    //transitions to a new scene
    //conditions: current = true, means current is loaded. current = false, means current is unloaded
    //pre conditions: current
    //post conditions: current = !target_pre && target_post
    private async Task Transitioning(string currentScene, string targetScene, TransitionType transitionType)
    {
        Debug.Log($"Transitioning from {currentScene} to {targetScene}");

        switch (transitionType)
        {
            case TransitionType.Additive:
                await LoadScene(targetScene);
                break;
            
            case TransitionType.Unload:
                SceneManager.UnloadSceneAsync(currentScene);
                break;
            
            case TransitionType.Transition:
                SceneManager.UnloadSceneAsync(currentScene);
                await LoadScene(targetScene);
                break;
        }
    }

    #region Async Scene Loading Helper Functions
    /// <summary>
    /// Converts SceneManager.LoadSceneAsync() from an AsyncOperation to a Task so that it is awaitable.
    /// </summary>
    /// <param name="targetScene">The name of the scene to be loaded.</param>
    /// <returns></returns>
    private Task LoadScene(string targetScene)
    {
        // Create a TaskCompletionSource to return as a Task
        // TaskCompletionSource is used to define when an "await" is finished
        var tcs = new TaskCompletionSource<bool>();

        // Start the coroutine
        StartCoroutine(LoadSceneCoroutine(targetScene, tcs));

        // Return the task that will complete when the coroutine ends
        return tcs.Task;
    }

    /// <summary>
    /// Uses a coroutine to load a scene and wait for it to finish loading.
    /// </summary>
    /// <param name="targetScene">The name of the scene to be loaded.</param>
    /// <param name="tcs">A reference to the TaskCompletionSource.</param>
    private IEnumerator LoadSceneCoroutine(string targetScene, TaskCompletionSource<bool> tcs)
    {
        // The async operation which loads the scene, we can wait for this to complete
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(targetScene, LoadSceneMode.Additive);

        // Wait until the scene is fully loaded
        while (!asyncLoad.isDone)
            yield return null;

        // Mark the TaskCompletionSource as completed
        tcs.SetResult(true);
    }
    #endregion

    //if a scene really wants to determine the transition itself, this method can be directly called to override the transition code
    public async Task TransitionScene(SceneName from, SceneName to, TransitionType transitionType, Func<string, string, TransitionType, Task> loadCode)
    {
        string currentScene = from.ToString();
        string targetScene = to.ToString();
        
        //some extra checks will be made about the validity of the variables and the file contents
        //for example, making a new scene, but forgetting to put it into the scene graph file, should result in an error here.
        
        //checks, does currentScene point to nextScene in the graph?
        int currentSceneID = sceneToID[currentScene];
        int targetSceneID = sceneToID[targetScene];
        if (!sceneGraph[currentSceneID].Contains((targetSceneID, transitionType)))
            //invalid transition
            throw new Exception($"Current scene {currentScene} cannot make a {transitionType}-transition to {targetScene}");
        
        await loadCode(currentScene, targetScene, transitionType);
    }

    
    //args is the data to transfer
    public async Task TransitionScene(SceneName from, SceneName to, TransitionType transitionType) => await TransitionScene(from, to, transitionType, Transitioning);

    
    //the function to be called when loading the first cycle
    public void StartScene(SceneName start)
    {
        ReadSceneGraph();

        string currentScene = start.ToString();
        SceneManager.LoadScene(currentScene, LoadSceneMode.Additive);
    }
    
    //TODO: This method should be removed eventually
    //this method is not tested
    public void ToggleNotebookScene()
    {
        if (SceneManager.GetSceneByName("NotebookScene").isLoaded)
        {
            TransitionScene(SceneName.NotebookScene, SceneName.Loading, TransitionType.Unload);
            //SceneManager.UnloadSceneAsync("NotebookScene");
        }
        else
        {
            TransitionScene(SceneName.Loading, SceneName.NotebookScene, TransitionType.Additive);
            //SceneManager.LoadScene("NotebookScene", LoadSceneMode.Additive);
        }
    }
}

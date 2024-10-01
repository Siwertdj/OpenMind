using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;
using File = System.IO.File;

public class SceneController : MonoBehaviour
{
    public enum SceneName
    {
        NPCSelectScene,
        DialogueScene,
        GameOverScene,
        GameWinScene,
        Loading,
        NotebookScene
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

    private const string TransitionGraphLocation = "Transition Graph/Transition Graph.txt";
    private string GetTransitionGraphFilePath() => Path.Combine(Application.dataPath, "../") + TransitionGraphLocation;

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

    //read the scene graph from the file and assign both vars described above
    private void ReadSceneGraph()
    {
        //first check if the file exists
        if (!File.Exists(GetTransitionGraphFilePath()))
        {
            Debug.LogError($"Couldn't read the scene graph, the file on filepath {GetTransitionGraphFilePath()} was not found.");
            return;
        }

        string[] fileGraphContentLines = File.ReadAllLines(GetTransitionGraphFilePath());
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
    private void Transitioning(string currentScene, string targetScene, TransitionType transitionType)
    {
        switch (transitionType)
        {
            case TransitionType.Additive:
                SceneManager.LoadScene(targetScene, LoadSceneMode.Additive);
                break;
            
            case TransitionType.Unload:
                SceneManager.UnloadSceneAsync(currentScene);
                break;
            
            case TransitionType.Transition:
                SceneManager.LoadScene(targetScene, LoadSceneMode.Additive);
                SceneManager.UnloadSceneAsync(currentScene);
                break;
        }
    }
    
    //if a scene really wants to determine the transition itself, this method can be directly called to override the transition code
    public void TransitionScene(SceneName from, SceneName to, TransitionType transitionType, Action<string, string, TransitionType> loadCode)
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
            throw new Exception();
        
        //if it's an unload (the target scene is loaded), check if the target scene is the parent scene of this scene
        //otherwise with unloading a scene, a scene can be selected in a such a way as to always allow an unload.
        if (SceneManager.GetSceneByName(targetScene).isLoaded &&
            SceneManager.GetSceneAt(SceneManager.sceneCount - 2).name != targetScene)
            throw new Exception();
        
        loadCode(currentScene, targetScene, transitionType);
    }

    
    //args is the data to transfer
    public void TransitionScene(SceneName from, SceneName to, TransitionType transitionType) => TransitionScene(from, to, transitionType, Transitioning);

    
    //the function to be called when loading the first cycle
    public void StartScene(SceneName start)
    {
        ReadSceneGraph();

        string currentScene = start.ToString();
        SceneManager.LoadScene(currentScene, LoadSceneMode.Additive);
    }
    
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
    
    #region obsolete
    public void ToggleDialogueScene()
    {
        string sceneName = "DialogueScene";

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
    #endregion
}

// public abstract class AbstractScene : MonoBehaviour
// {
//     //read from a file
//     private static List<List<int>> sceneGraph;
//
//     //inferred from reading the same file, what scene is matched to what id doesn't matter, as long as they are all assigned to a unique ID
//     private static Dictionary<string, int> sceneToID;
//     
//     //an abstract method so the scene can receive variables, this can be left empty when override if a scene doesn't need variables
//     protected abstract void GetVariables(params object[] args);
//
//     //read the scene graph from the file and assign both vars described above
//     private void ReadSceneGraph()
//     {
//         //some file reading code
//     }
//
//     //transitions to a new scene
//     //conditions: current = true, means current is loaded. current = false, means current is unloaded
//     //pre conditions: current
//     //post conditions: current = !target_pre && target_post
//     private void Transitioning(string currentScene, string targetScene)
//     {
//         //target scene is loaded, meaning we want to unload the current scene
//         if (SceneManager.GetSceneByName(targetScene).isLoaded)
//         {
//             SceneManager.UnloadSceneAsync(currentScene);
//         }
//         else
//         {
//             SceneManager.LoadScene(targetScene, LoadSceneMode.Additive);
//         }
//     }
//     
//     //if a scene really wants to determine the transition itself, this method can be directly called to override the transition code
//     protected void TransitionScene<T1, T2>(Action<string, string> loadCode, params object[] args)
//         where T1 : AbstractScene
//         where T2 : AbstractScene
//     {
//         string currentScene = typeof(T1).Name;
//         string targetScene = typeof(T2).Name;
//         
//         //some extra checks will be made about the validity of the variables and the file contents
//         //for example, making a new scene, but forgetting to put it into the scene graph file, should result in an error here.
//         
//         //checks, does currentScene point to nextScene in the graph?
//         int currentSceneID = sceneToID[currentScene];
//         int targetSceneID = sceneToID[targetScene];
//         if (!sceneGraph[currentSceneID].Contains(targetSceneID))
//             //invalid transition
//             throw new Exception();
//         
//         //if it's an unload (the target scene is loaded), check if the target scene is the parent scene of this scene
//         //otherwise with unloading a scene, a scene can be selected in a such a way as to always allow an unload.
//         if (SceneManager.GetSceneByName(targetScene).isLoaded &&
//             SceneManager.GetSceneAt(SceneManager.sceneCount - 2).name != targetScene)
//             throw new Exception();
//         
//         loadCode(currentScene, targetScene);
//         //assign variables to the target scene
//         FindObjectOfType<T2>().GetVariables(args);
//     }
//
//     
//     //args is the data to transfer
//     protected void TransitionScene<T1, T2>(params object[] args)
//         where T1 : AbstractScene
//         where T2 : AbstractScene
//         => TransitionScene<T1, T2>(Transitioning, args);
//
//     
//     //the function to be called when loading the first cycle
//     public void StartScene<T>(params object[] args) where T : AbstractScene
//     {
//         ReadSceneGraph();
//         
//         string currentScene = typeof(T).Name;
//         SceneManager.LoadScene(currentScene, LoadSceneMode.Additive);
//         GetVariables(args);
//     }
// }
//
// //an example of a scene class below:
// public class DialogueScene : AbstractScene
// {
//     private string characterToTalkTo;
//     //this scene should get a variable about who to talk to
//     protected override void GetVariables(params object[] args)
//     {
//         //assign variables
//         characterToTalkTo = args[0].ToString();
//     }
//
//     //a method that loads a new scene with some variables, assuming CompanionHint is currently unloaded
//     private void SomeMethod1()
//     {
//         TransitionScene<DialogueScene, CompanionHint>(1, 2);
//     }
// }
//
// public class CompanionHint : AbstractScene
// {
//     protected override void GetVariables(params object[] args)
//     {
//         
//     }
// }
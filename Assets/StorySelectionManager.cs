using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StorySelectionManager : MonoBehaviour
{
    // Game Events
    [Header("Events")]
    public GameEvent onGameLoaded;
    
    public void Story1Selected()
    {
        Debug.Log("Button 1 clicked");
        StartStory(1);
    }
    
    public void Story2Selected()
    {
        Debug.Log("Button 2 clicked");
        StartStory(2);
    }
    
    public void Story3Selected()
    {
        Debug.Log("Button 3 clicked");
        StartStory(3);
    }

    void StartStory(int storyid)
    {
        // Based on the passed storyid, we start the game in different ways.
        // First we do all the behavior as normal, such as initiating the Story:
        StartCoroutine(GameLoader(storyid));
    }

    IEnumerator GameLoader(int storyid)
    {
        // The Application loads the Scene in the background as the current Scene runs. 
        // This way, we can pass on information to that next scene.
        
        // We load the scene as an AsyncOperation
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Loading", LoadSceneMode.Additive);
        
        // Within this while-loop, we wait until the scene is done loading. We check this every frame
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        
        // TODO: Double check if this is the right place to perform our operations AFTER the scene is loaded
        
        // When the GameManager is loaded, we now raise the proper event to pass our data along.
        switch (storyid)
        {
            case 1:
                onGameLoaded.Raise(this, GameManager.GameStory.Phone);  // pass correct story via event
                SceneManager.UnloadSceneAsync("StorySelectScene");  // unload this scene; no longer necessary
                break;
            case 2:
                onGameLoaded.Raise(this, GameManager.GameStory.Phone);  // pass correct story via event
                SceneManager.UnloadSceneAsync("StorySelectScene");  // unload this scene; no longer necessary
                break;
            case 3:
                onGameLoaded.Raise(this, GameManager.GameStory.Phone);  // pass correct story via event
                SceneManager.UnloadSceneAsync("StorySelectScene");  // unload this scene; no longer necessary
                break;
            default:
                Debug.LogError($"Couldn't load the story, Story-ID {storyid} was not found. Try another option.");
                SceneManager.UnloadSceneAsync("Loading");  // unload this scene; no longer necessary
                Destroy(GameObject.FindGameObjectWithTag("Toolbox"));  // TODO: Test this
                break;
        }
    }
}

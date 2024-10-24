using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// The manager class for the StorySelectScene
/// </summary>
public class StorySelectionManager : MonoBehaviour
{
    [Header("Data")] 
    [SerializeField] private StoryObject[] stories;
    
    // Game Events
    [Header("Events")]
    public GameEvent onIntroLoaded;
    
    /// <summary>
    /// Starts Story A
    /// </summary>
    public void StoryASelected()
    {
        StartIntro(0);
    }
    
    /// <summary>
    /// Starts Story B
    /// </summary>
    public void StoryBSelected()
    {
        StartIntro(1);
    }
    
    /// <summary>
    /// Starts Story C
    /// </summary>
    public void StoryCSelected()
    {
        StartIntro(2);
    }

    /// <summary>
    /// Starts the intro belonging to the story the player chose
    /// </summary>
    /// <param name="storyid">The story that the player chose</param>
    void StartIntro(int storyid)
    {
        StartCoroutine(LoadIntro(storyid));
    }
    
    /// <summary>
    /// Loads the appropriate intro scene
    /// </summary>
    /// <param name="storyid">The story that the player chose</param>
    IEnumerator LoadIntro(int storyid)
    {
        // Start the loadscene-operation
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("IntroStoryScene", LoadSceneMode.Additive);
        
        // Within this while-loop, we wait until the scene is done loading. We check this every frame
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        onIntroLoaded.Raise(this, stories[storyid]);
        
        // Finally, when the data has been sent, we then unload our currentscene
        SceneManager.UnloadSceneAsync("StorySelectScene");  // unload this scene; no longer necessary
    }
    
}

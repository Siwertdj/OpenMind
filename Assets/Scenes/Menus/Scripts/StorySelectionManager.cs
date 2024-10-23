using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StorySelectionManager : MonoBehaviour
{
    [Header("Data")] 
    [SerializeField] private StoryObject[] stories;
    
    // Game Events
    [Header("Events")]
    public GameEvent onIntroLoaded;
    
    public void StoryASelected()
    {
        StartIntro(0);
    }
    
    public void StoryBSelected()
    {
        StartIntro(1);
    }
    
    public void StoryCSelected()
    {
        StartIntro(2);
    }

    void StartIntro(int storyid)
    {
        
        StartCoroutine(LoadIntro(storyid));
        
    }
    
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

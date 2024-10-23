using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class TimelineManager : MonoBehaviour
{
    public PlayableDirector introStoryA;
    public PlayableDirector introStoryB;
    public PlayableDirector introStoryC;
    //public Button continueButton;
    public TMP_Text objectiveText;
    
    private PlayableDirector currentTimeline; 
    
    // GameEvent, necessary for passing the right story to Loading
    public GameEvent onGameLoaded;
    private StoryObject story;

    public void StartIntro(Component sender, params object[] data)
    {
        // depending on the chosen storyline, play the intro to the story
        if (data[0] is StoryObject storyObject)
        {
            // set story-variable
            story = storyObject;
            // depending on the chosen storyline, play the intro to the story
            switch (storyObject.storyID)
            {
                case 0:
                    currentTimeline = introStoryA;
                    break;
                case 1:
                    currentTimeline = introStoryB;
                    break;
                case 2:
                    currentTimeline = introStoryC;
                    break;
                default:
                    currentTimeline = introStoryA;
                    break;
            }
        }
        else
        {
            Debug.LogError("Error: Illegal data passed to Introduction-scene. Returning to StorySelectScene to retry.");
            // Return to StorySelectScene adn try again.
            SceneManager.LoadScene("StorySelectScene");
        }
        currentTimeline.Play();
    }

    public void PauseCurrentTimeline()
    {
        currentTimeline.Pause();
        //continueButton.gameObject.SetActive(true);
    }

    public void ContinueCurrentTimeline()
    {
        currentTimeline.Play();
        //continueButton.gameObject.SetActive(false);
    }

    public void StartGame()
    {
        StartCoroutine(LoadGame());
        
    }
    
    
    // TODO: This is duplicate code, also found in Loading.cs. Make this a global thing?
    IEnumerator LoadGame()
    {
        // Start the loadscene-operation
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Loading", LoadSceneMode.Additive);
        
        // Within this while-loop, we wait until the scene is done loading. We check this every frame
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        
        onGameLoaded.Raise(this, story);
        
        // Finally, when the data has been sent, we then unload our currentscene
        SceneManager.UnloadSceneAsync("IntroStoryScene");  // unload this scene; no longer necessary
    }
}

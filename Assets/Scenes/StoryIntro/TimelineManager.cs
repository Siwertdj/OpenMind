// This program has been developed by students from the bachelor Computer Science at Utrecht University within the Software Project course.
// © Copyright Utrecht University (Department of Information and Computing Sciences)
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

/// <summary>
/// Manager class for timelines.
/// </summary>
public class TimelineManager : MonoBehaviour
{
    public  PlayableDirector introStoryA;
    public  PlayableDirector introStoryB;
    public  PlayableDirector introStoryC;
    public  TMP_Text         objectiveText;
    private PlayableDirector currentTimeline;
    // GameEvent, necessary for passing the right story to Loading
    public GameEvent onGameLoaded;
    private StoryObject story;

    /// <summary>
    /// Starts the proper intro.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="data">The story that was chosen.</param>
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

    /// <summary>
    /// Pauses the timeline.
    /// </summary>
    public void PauseCurrentTimeline()
    {
        currentTimeline.Pause();
        //continueButton.gameObject.SetActive(true);
    }

    /// <summary>
    /// Continues the timeline.
    /// </summary>
    public void ContinueCurrentTimeline()
    {
        currentTimeline.Play();
        //continueButton.gameObject.SetActive(false);
    }

    /// <summary>
    /// Starts the game once the intro is over.
    /// </summary>
    public void StartGame()
    {
        StartCoroutine(LoadGame());
    }

    /// <summary>
    /// Loads the game.
    /// </summary>
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

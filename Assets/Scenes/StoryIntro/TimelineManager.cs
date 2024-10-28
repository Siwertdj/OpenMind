using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public Sprite[] backgrounds;
    public String[] storyText; 
    public TMP_Text text;
    public Image    background;
    public Button   continueButton;
    public Image    textBubble; 
    
    private PlayableDirector currentTimeline; 
    private int              backgroundIndex = 0;
    private int              textIndex       = -1; 
    
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
                    StoryA();
                    break;
                case 1:
                    StoryB();
                    break;
                case 2:
                    StoryC();
                    break;
                default:
                    StoryA();
                    break;
            }
        }
        else
        {
            Debug.LogError("Error: Illegal data passed to Introduction-scene. Returning to StorySelectScene to retry.");
            // Return to StorySelectScene adn try again.
            SceneManager.LoadScene("StorySelectScene");
        }
    }
    
    public void ChangeText()
    {
        PauseCurrentTimeline();
        textBubble.gameObject.SetActive(true);
        text.gameObject.SetActive(true);
        textIndex++;
        try
        {
            text.text = storyText[textIndex];
        }
        catch
        {
            textIndex = 0;
            text.text = storyText[textIndex];
            Debug.LogError("Error: No more text to speak.");
        }
        
    }
    
    private void StoryA()
    {
        currentTimeline = introStoryA;
        currentTimeline.Play();
        backgroundIndex = 0;
        background.sprite = backgrounds[backgroundIndex];
    }
    
    private void StoryB()
    {
        currentTimeline = introStoryB;
        currentTimeline.Play();
        text.text = "Story B";
    }
    
    private void StoryC()
    {
        currentTimeline = introStoryC;
        currentTimeline.Play();
        text.text = "Story C";
    }
    
    public void ChangeBackground()
    {
        backgroundIndex++;
        try
        {
            background.sprite = backgrounds[backgroundIndex];
        }
        catch
        {
            Debug.LogError("Error: No more available backgrounds.");
            backgroundIndex = 0;
            background.sprite = backgrounds[backgroundIndex];
        }
        if(backgroundIndex > 0) PauseCurrentTimeline();
    }
    
    
    public void PauseCurrentTimeline()
    {
        continueButton.gameObject.SetActive(true);
        currentTimeline.Pause();
    }

    public void ContinueCurrentTimeline()
    {
        continueButton.gameObject.SetActive(false);
        textBubble.gameObject.SetActive(false);
        text.gameObject.SetActive(false);
        currentTimeline.Play();
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

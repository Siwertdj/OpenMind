// This program has been developed by students from the bachelor Computer Science at Utrecht University within the Software Project course.
// © Copyright Utrecht University (Department of Information and Computing Sciences)
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Vector2 = System.Numerics.Vector2;

// hallo

/// <summary>
/// Manager class for timelines.
/// </summary>
public class TimelineManager : MonoBehaviour
{
    // PlayableDirectors manage the different timelines for the different stories
    public PlayableDirector introStoryA;
    public PlayableDirector introStoryB;
    public PlayableDirector introStoryC;
    
    public Sprite[]     backgrounds; // Stores all the used backgrounds for the introduction.
    public String[]     storyText;   // Stores all the used text for the introduction. 
    public GameObject[] TextMessages;
    public GameObject[] MessageLocations; 
    
    // The variables below are the UI components that we want to manipulate during the introduction
    public TMP_Text playerText;
    public  Image    background;
    public  Button   continueButton;
    public  Image    textBubble;
    // Variables to keep track of the state of the introduction within this code. 
    private PlayableDirector currentTimeline; 
    private int backgroundIndex = 0; // backgrounds[backgroundIndex] is the currently shown background.
    private int playerTextIndex = -1; // text[textIndex] is the currently shown text. 
    private int textMessageIndex = 0; 
    
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
                    StoryA();
                    break;
                case 1:
                    StoryA();
                    break;
                case 2:
                    StoryA();
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
    
    // This region contains methods that manipulate UI elements of the scene.
    #region UIManipulators
    
    /// <summary>
    /// Depending on the value of show, this method either hides of shows the text messages on the screen.
    /// </summary>
    /// <param name="show"> Determines whether to hide or to show the texts. </param>
    private void HideOrShowTexts(bool show)
    {
        foreach (GameObject location in MessageLocations)
        {
            location.SetActive(show);
        }
        if (!show) // If the messages need to be hidden, make sure old messages are hidden as well. 
        {
            foreach(GameObject message in TextMessages)
            {
                message.SetActive(false);
            }
        }
    }
    
    /// <summary>
    /// This method shows a new text on the screen and makes sure old texts are removed if necessary. 
    /// </summary>
    public void SendText()
    {
        PauseCurrentTimeline();
        background.sprite = backgrounds[3]; // Change the background to the phone background. 
        textMessageIndex++;
        if (textMessageIndex > 4) HideOrShowTexts(false); // More than 4 messages means that old ones need to be removed. 
        // Make sure the four most recent texts are shown on the screen. 
        for (int i = textMessageIndex; i < textMessageIndex + 4; i++)
        {
            TextMessages[i].transform.position = MessageLocations[i-textMessageIndex].transform.position; 
            TextMessages[i].SetActive(true);
        }
        HideOrShowTexts(true); // Show the new texts. 
    }
    
    /// <summary>
    /// This method changes and shows text the player is saying. 
    /// </summary>
    public void ChangePlayerText()
    {
        PauseCurrentTimeline();
        // Activate UI elements for the player text. 
        textBubble.gameObject.SetActive(true);
        playerText.gameObject.SetActive(true);
        playerTextIndex++; // Keep track of which text needs to be shown. 
        try
        {
            playerText.text = storyText[playerTextIndex];
        }
        catch
        {
            playerTextIndex = 0;
            playerText.text = storyText[playerTextIndex];
            Debug.LogError("Error: No more text to speak.");
        }
    }
    
    /// <summary>
    /// This method changes the background of the scene. 
    /// </summary>
    public void ChangeBackground()
    {
        HideOrShowTexts(false); // When the background is changed, the texts need to be hidden. 
        backgroundIndex++; // Keep track of the background that needs to be shown. 
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
        
        if (backgroundIndex > 0)
        {
            PauseCurrentTimeline(); // The first time the background is changed, the timeline does not have to be paused. 
        } 
    }

    #endregion
    
    // This region contains methods that directly manipulate the timeline
    #region TimelineManipulators

    /// <summary>
    /// Pauses the timeline.
    /// </summary>
    public void PauseCurrentTimeline()
    {
        continueButton.gameObject.SetActive(true);
        currentTimeline.Pause();
    }

    /// <summary>
    /// Continues the timeline.
    /// </summary>
    public void ContinueCurrentTimeline()
    {
        continueButton.gameObject.SetActive(false);
        textBubble.gameObject.SetActive(false);
        playerText.gameObject.SetActive(false);
        currentTimeline.Play();
    }

    #endregion

    // This region
    #region StoryLines

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
        playerText.text = "Story B";
    }
    
    private void StoryC()
    {
        currentTimeline = introStoryC;
        currentTimeline.Play();
        playerText.text = "Story C";
    }

    #endregion
    
    // This region contains methods that handle the starting of the game at the end of the introduction
    #region StartGame 
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
    #endregion
   
}

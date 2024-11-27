﻿// This program has been developed by students from the bachelor Computer Science at Utrecht University within the Software Project course.
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

/// <summary>
/// Manager class for the introduction.
/// </summary>
public class IntroductionManager : MonoBehaviour
{
    // PlayableDirectors manage the different timelines for the different stories
    public PlayableDirector introStoryA;
    public PlayableDirector introStoryB;
    public PlayableDirector introStoryC;
    
    public Sprite[]     backgrounds; // Stores all the used backgrounds for the introduction.
    public String[]     storyText;   // Stores all the used text for the introduction. 
    private GameObject[] messages; 
    public  GameObject[] messageLocations;
    public  TMP_Text     typingText;
    [SerializeField] private Transform    canvasTransform;
    [SerializeField] public TextMessage[] TextMessages;

    // The variables below are the UI components that we want to manipulate during the introduction
    [SerializeField] private DialogueAnimator dialogueAnimator;
    [SerializeField] private DialogueAnimator typingAnimation;
    
    public  Image    background;
    public  GameObject   continueButton;
    public Button sendButton; 
    
    // Variables to keep track of the state of the introduction within this code. 
    public PlayableDirector currentTimeline; // public for testing purposes
    private int backgroundIndex = 0; // backgrounds[backgroundIndex] is the currently shown background.
    private int playerTextIndex = -1; // text[textIndex] is the currently shown text. 
    private int textMessageIndex = 0;
    private int typeIndex = 0; 
    
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
            // Start the music
            SettingsManager.sm.SwitchMusic(story.storyIntroMusic, null);
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
            // Return to StorySelectScene and try again.
            SceneManager.LoadScene("StorySelectScene");
        }
    }
    
    // This region contains methods that regulate the different storylines. 
    #region StoryLines
    
    /// <summary>
    /// Method that prepares the scene to play storyline A. 
    /// </summary>
    private void StoryA()
    {
        messages = new GameObject[TextMessages.Length];
        for(int i = 0; i < TextMessages.Length; i++)
        {
            // Instantiate the TextMessages
            GameObject instantiatedMessage = Instantiate(TextMessages[i].message, canvasTransform);
            TMP_Text tmpText = instantiatedMessage.GetComponentInChildren<TMP_Text>();
            if (tmpText != null) // When the text component is null, it is an empty text
            {
                tmpText.text = TextMessages[i].messageContent; // Change the content to the correct content
            }
            // Make sure the messages end up at the correct location in the hierarchy. 
            // Otherwise, it might be the case that they will overlap with other game objects. 
            instantiatedMessage.transform.SetSiblingIndex(canvasTransform.childCount - 6);
            messages[i] = instantiatedMessage; // Add the instantiated message to the textMessage array
        }
        // Initialize the right timeline and indices for story A. 
        currentTimeline = introStoryA;
        currentTimeline.Play();
        backgroundIndex = 0;
        background.sprite = backgrounds[backgroundIndex];
    }
    
    /// <summary>
    /// Method that prepares the scene to play storyline B. 
    /// </summary>
    private void StoryB()
    {
        currentTimeline = introStoryB;
        currentTimeline.Play();
    }
    
    /// <summary>
    /// Method that prepares the scene to play storyline C. 
    /// </summary>
    private void StoryC()
    {
        currentTimeline = introStoryC;
        currentTimeline.Play();
    }
    
    #endregion
    
    // This region contains methods regarding introduction A.
    #region Introduction A
    /// <summary>
    /// Depending on the value of show, this method either hides of shows the text messages on the screen.
    /// </summary>
    /// <param name="show"> Determines whether to hide or to show the texts. </param>
    private void HideOrShowTexts(bool show)
    {
        foreach (GameObject location in messageLocations)
        {
            location.SetActive(show);
        }
        if (!show) // If the messages need to be hidden, make sure old messages are hidden as well. 
        {
            foreach(GameObject message in messages)
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
        sendButton.gameObject.SetActive(false);
        typingText.gameObject.SetActive(false);
        background.sprite = backgrounds[3]; // Change the background to the phone background. 
        textMessageIndex++;
        
        // Make sure the four most recent texts are shown on the screen. 
        HideOrShowTexts(false); // Old messages need to be removed. 
        for (int i = textMessageIndex; i < textMessageIndex + 4; i++)
        {
            //messages[i].transform.SetParent(messageLocations[i - textMessageIndex].transform);
            messages[i].transform.position = messageLocations[i-textMessageIndex].transform.position;
            messages[i].SetActive(true);
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
        dialogueAnimator.gameObject.SetActive(true);
        playerTextIndex++; // Keep track of which text needs to be shown. 
        try
        {
            dialogueAnimator.WriteDialogue(storyText[playerTextIndex]);
        }
        catch
        {
            playerTextIndex = 0;
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
    
    /// <summary>
    /// This method creates the animation that the player types in text and can then send it. 
    /// </summary>
    public void TypeAnimation()
    {
        continueButton.SetActive(false); //This button is not necessary now, because we have another button to continue. 
        PauseCurrentTimeline();
        // Reset the typing animation object
        typingAnimation.gameObject.SetActive(true);
        typingAnimation.CancelWriting();
        // Activate the UI elements for the typing animation
        sendButton.gameObject.SetActive(true);
        typingText.gameObject.SetActive(true);
        // Write the next message, '+ messageLocations.Length' is to account for the empty messages. 
        typingText.text = TextMessages[textMessageIndex + messageLocations.Length].messageContent;
        typingAnimation.WriteDialogue(TextMessages[textMessageIndex + messageLocations.Length].messageContent);
    }
    #endregion
    
    // This region contains methods that directly manipulate the timeline
    #region TimelineManipulators

    /// <summary>
    /// Pauses the timeline.
    /// </summary>
    public void PauseCurrentTimeline()
    {
        continueButton.SetActive(true);
        currentTimeline.Pause();
    }

    /// <summary>
    /// Continues the timeline.
    /// </summary>
    public void ContinueCurrentTimeline()
    {
        if (dialogueAnimator.IsOutputting)
        {
            dialogueAnimator.SkipDialogue();
        }
        else
        {
            continueButton.SetActive(false);
            dialogueAnimator.gameObject.SetActive(false);
            typingAnimation.gameObject.SetActive(false);
            currentTimeline.Play();
        }
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
        
        // Make sure tutorial is automatically loaded when the game starts. 
        GameObject tutorial = GameObject.Find("HelpButton");
        Button helpButton = tutorial.GetComponentInChildren<Button>();
        helpButton.onClick.Invoke();
    }
    #endregion
    
    
   
}
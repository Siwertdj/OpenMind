using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class CutsceneController : MonoBehaviour
{
    public PlayableDirector playableDirector;
    public GameObject imageWithGrid;                 
    public GameObject imageWithoutGrid;
    public Toggle imageToggler;
    public Button continueButton;
    
    private Transform checkmarkTransform; 
    
    /// <summary>
    /// This method is called when the GameObject this script belongs to is activated. 
    /// </summary>
    private void Start()
    {
        // Access the Checkmark GameObject via the Toggle's hierarchy
       checkmarkTransform = imageToggler.transform.Find("Background/Checkmark");
    }

    /// <summary>
    /// This method is called via a signal emitter when the timeline encounters a point where
    /// it has to be paused in order to wait for user interaction.
    /// This method also activates the continuebutton with which the timeline can be resumed. 
    /// </summary>
    public void PauseTimeline()
    {
        playableDirector.Pause();
        continueButton.gameObject.SetActive(true); // Make sure timeline can manually be resumed. 
    }

    /// <summary>
    /// This method is called via a signal receiver when continueButton is clicked and the
    /// timeline has to be resumed.
    /// </summary>
    public void ContinueTimeline()
    {
        // Make sure both images are removed from the screen. 
        imageWithoutGrid.SetActive(false);
        imageWithGrid.SetActive(false);
        // Disable continuebutton
        continueButton.gameObject.SetActive(false);
        // Resume timeline.
        playableDirector.Play();
    }
    
    /// <summary>
    /// This method is called when the toggler is clicked. Depending on the value of the
    /// toggler, isOn, a different image is shown. 
    /// </summary>
    public void OnToggleValueChanged(bool isOn)
    {
        imageWithoutGrid.SetActive(!isOn); // Either show or hide the image without grid
        imageToggler.isOn = isOn; // Change the value of the toggler
        checkmarkTransform.GameObject().SetActive(isOn); // Make sure the checkmark (dis)appears at the right time. 
        imageWithGrid.SetActive(isOn); // Either show or hide the image with grid
    }

    /// <summary>
    /// This method is called when the timeline reaches the end of the prologue.
    /// Then the introduction to the story can be loaded. 
    /// </summary>
    public void LoadChooseStory()
    {
        SceneManager.LoadScene("StorySelectScene");
    }

    
    
}

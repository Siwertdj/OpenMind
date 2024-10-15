using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.Events;

public class CutsceneController : MonoBehaviour
{
    public PlayableDirector playableDirector;
    public GameObject imageWithGrid;                 
    public GameObject imageWithoutGrid;
    public Toggle imageToggler;
    public Button continueButton;
    
    private Transform checkmarkTransform; 
    private void Start()
    {
        // Access the Checkmark GameObject via the Toggle's hierarchy
       checkmarkTransform = imageToggler.transform.Find("Background/Checkmark");
    }

    public void PauseTimeline()
    {
        playableDirector.Pause();
        continueButton.gameObject.SetActive(true);
    }

    public void ContinueTimeline()
    {
        // Make sure both images are removed from the screen. 
        imageWithoutGrid.SetActive(false);
        imageWithGrid.SetActive(false);
        // Disable button
        continueButton.gameObject.SetActive(false);
        // Resume timeline.
        playableDirector.Play();
    }
    
    public void OnToggleValueChanged(bool isOn)
    {
        imageWithoutGrid.SetActive(!isOn);
        imageToggler.isOn = isOn; 
        checkmarkTransform.GameObject().SetActive(isOn);
        imageWithGrid.SetActive(isOn);
    }

    public void StartIntro()
    {
        // load StoryIntroductionScene
    }

    
    
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class CutsceneController : MonoBehaviour
{
    public PlayableDirector playableDirector;
    public GameObject imageWithGrid;                 
    public GameObject imageWithoutGrid;
    public Toggle imageToggler;
    public Button continueButton; 
    private void Start()
    {
        //imageToggler.onValueChanged.AddListener(OnToggleValueChanged);
       //imageToggler.gameObject.SetActive(false);
    }

    public void PauseTimeline()
    {
        playableDirector.Pause();
        continueButton.gameObject.SetActive(true);
        //imageToggler.gameObject.SetActive(true);
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
        imageWithGrid.SetActive(isOn);
    }
    
}

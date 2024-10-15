using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class TimelineManager : MonoBehaviour
{
    public PlayableDirector introStoryA;
    public PlayableDirector introStoryB;
    public PlayableDirector introStoryC;
    public Button continueButton;
    
    private PlayableDirector currentTimeline; 

    private void Start()
    {
        // depending on the chosen storyline, play the intro to the story
        currentTimeline = introStoryA; 
        currentTimeline.Play();
    }

    public void PauseCurrentTimeline()
    {
        currentTimeline.Pause();
        continueButton.gameObject.SetActive(true);
    }

    public void ContinueCurrentTimeline()
    {
        currentTimeline.Play();
        continueButton.gameObject.SetActive(false);
    }

    public void StartGame()
    {
        //Debug.Log("intro end");
        //GameManager.gm.StartGame();
    }
}

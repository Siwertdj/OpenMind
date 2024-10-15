using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineManager : MonoBehaviour
{
    public PlayableDirector introStoryA;
    public PlayableDirector introStoryB;
    public PlayableDirector introStoryC;

    private void Start()
    {
        // depending on the chosen storyline, play the intro to the story
        introStoryA.Play();
    }
    
    public void StartGame()
    {
        //Debug.Log("intro end");
        //GameManager.gm.StartGame();
    }
}

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
    private void Start()
    {
        //imageToggler.onValueChanged.AddListener(OnToggleValueChanged);
       //imageToggler.gameObject.SetActive(false);
    }

    public void PauseTimeline()
    {
        playableDirector.Pause();
        imageToggler.gameObject.SetActive(true);
    }

    public void ContinueTimeline()
    {
        playableDirector.Play();
    }
    
    public void OnToggleValueChanged(bool isOn)
    {
        imageWithoutGrid.SetActive(!isOn);
        imageWithGrid.SetActive(isOn);
    }
    
}

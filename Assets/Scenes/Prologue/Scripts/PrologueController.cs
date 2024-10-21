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
    public Toggle imageToggler;
    public Button continueButton;
    public TMP_Text spokenText;
    public TMP_Text introText;
    public TMP_Text nameBoxText;
    public Image textBubbleImage;
    public Image backgroundImage;
    public Image illusionImage; 
    
    public Sprite[] backgrounds; 
    public string[] receptionistText;
    public Sprite[] illusions; 
    
    private int speakIndex;
    private int backgroundIndex; 
    private Transform checkmarkTransform;
    
    /// <summary>
    /// This method is called when the GameObject this script belongs to is activated. 
    /// </summary>
    private void Start()
    {
        // Access the Checkmark GameObject via the Toggle's hierarchy
       checkmarkTransform = imageToggler.transform.Find("Background/Checkmark");
       speakIndex = -1;
       backgroundIndex = 0;
    }

    public void updateIntroText()
    {
        introText.text = "Not everything is as it seems...";
    }

    public void changeBackground()
    {
        backgroundIndex++;
        try
        {
            backgroundImage.sprite = backgrounds[backgroundIndex];
        }
        catch
        {
            backgroundImage.sprite = backgrounds[0];
        }
    }
    
    public void activateDialog()
    {
        textBubbleImage.gameObject.SetActive(true);
        nameBoxText.gameObject.SetActive(true);
        spokenText.gameObject.SetActive(true);
        changeReceptionistText();
        PauseTimeline();
    }

    public void changeReceptionistText()
    {
        speakIndex++;
        try
        {
            spokenText.text = receptionistText[speakIndex];
        }
        catch
        {
            spokenText.text = "no text";
        }
    }
    
    public void deactivateDialog()
    {
        textBubbleImage.gameObject.SetActive(false);
        nameBoxText.gameObject.SetActive(false);
        spokenText.gameObject.SetActive(false);
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
        // Make sure toggler is removed from the screen. 
        imageToggler.gameObject.SetActive(false);
        // Disable continuebutton
        continueButton.gameObject.SetActive(false);
        // Resume timeline.
        playableDirector.Play();
    }

    public void activateCloudIllusion()
    {
        illusionImage.sprite = illusions[2];
    }

    public void activateGridIllusion()
    {
        imageToggler.gameObject.SetActive(true);
    }

    /// <summary>
    /// This method is called when the toggler is clicked. Depending on the value of the
    /// toggler, isOn, a different image is shown. 
    /// </summary>
    public void OnToggleValueChanged(bool isOn)
    {
        imageToggler.isOn = isOn;  
        checkmarkTransform.GameObject().SetActive(isOn);
        if (isOn) illusionImage.sprite = illusions[0];
        else
        {
            illusionImage.sprite = illusions[1];
        }
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

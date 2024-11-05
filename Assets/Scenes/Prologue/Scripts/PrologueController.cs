// This program has been developed by students from the bachelor Computer Science at Utrecht University within the Software Project course.
// © Copyright Utrecht University (Department of Information and Computing Sciences)
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

/// <summary>
/// Manager class for cutscenes.
/// </summary>
public class CutsceneController : MonoBehaviour
{
    public PlayableDirector playableDirector; // Enables us to manually pause and continue the timeline
    // The variables below are the UI components that we want to manipulate during the prologue scene
    public Image textBubbleImage; 
    public Image backgroundImage;
    public Image illusionImage; 
    public TMP_Text introText;
    public TMP_Text spokenText;
    public TMP_Text nameBoxText;
    public Toggle imageToggler;
    public Button continueButton;
    // The arrays below store data that is required at a later stadium of the prologue
    public Sprite[] backgrounds; // Stores all the background images
    public Sprite[] illusions; // Stores all the optical illusion images
    public string[] receptionistText; // Stores all the text spoken by the receptionist
    
    private int textIndex; // Index to keep track of the text that needs to be spoken
    private int backgroundIndex; // Index to keep track of the background that needs to be used
    private Transform checkmarkTransform; // This is the checkmark image on the toggler
    
    // These variables are necessary for the typewriter effect of the text
    private Coroutine typeWriterCoroutine;
    public float typingSpeed = 0.1f; // The speed of the typewriter effect in seconds. 
    private string text;
    
    /// <summary>
    /// This method is called when the scene is started this script belongs to is activated. 
    /// </summary>
    private void Start()
    {
       checkmarkTransform = imageToggler.transform.Find("Background/Checkmark"); // Access the Checkmark GameObject via the Toggle's hierarchy
       // Intialize indices
       textIndex = -1; 
       backgroundIndex = 0;
    }
    
    // This region contains methods that directly manipulate the timeline. These methods are called via signal emitters
    #region TimelineManipulators
    
    /// <summary>
    /// This method is called via a signal receiver when continueButton is clicked and the
    /// timeline has to be resumed.
    /// </summary>
    public void ContinueTimeline()
    {
        Debug.Log("Continue Timeline");
        imageToggler.gameObject.SetActive(false); // Make sure toggler is removed from the screen.
        continueButton.gameObject.SetActive(false);  // Disable continuebutton
        playableDirector.Play(); // Resume timeline.
        StopCoroutine(typeWriterCoroutine); // Makes sure player can continue when texteffect is not finished
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
    /// This method is called when the timeline reaches the end of the prologue.
    /// When this method is called, the StorySelect scene is loaded. 
    /// </summary>
    public void LoadSelectStory()
    {
        SceneManager.LoadScene("StorySelectScene");
    }
    
    #endregion
    
    // This region contains methods that (de)activate UI elements on the canvas.
    #region UIActivators
    /// <summary>
    /// This method makes sure the cloud illusion is shown. 
    /// </summary>
    public void ActivateCloudIllusion()
    {
        illusionImage.sprite = illusions[2]; // Sprite 2 is the cloud illusion. 
    }
    /// <summary>
    /// This method makes sure the UI for the receptionist dialog is activated. 
    /// </summary>
    public void ActivateDialog()
    {
        // Activate the UI for the spoken text
        textBubbleImage.gameObject.SetActive(true);
        nameBoxText.gameObject.SetActive(true);
        spokenText.gameObject.SetActive(true);
        
        UpdateText(); // Update the text that is shown
        PauseTimeline(); // Pause the timeline such that the player can read the text. 
    }
    
    /// <summary>
    /// This method makes sure the grid illusion is shown. 
    /// </summary>
    public void ActivateGridIllusion()
    {
        imageToggler.gameObject.SetActive(true); // Only the toggler needs to be activated. The image is shown via the timeline. 
    }
    /// <summary>
    /// This method makes sure the UI of the receptionist dialog is deactivated. 
    /// </summary>
    public void DeactivateDialog()
    {
        textBubbleImage.gameObject.SetActive(false);
        nameBoxText.gameObject.SetActive(false);
        spokenText.gameObject.SetActive(false);
    }
    
    #endregion

    // This region contains methods that manipulate UI elements on the canvas.
    #region UIManipulators
    /// <summary>
    /// This method is called when the toggler is clicked. Depending on the value of the toggler isOn,
    /// a different image is shown. 
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
    /// This method changes the background by using the backgrounds array. 
    /// </summary>
    public void UpdateBackground()
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
    /// <summary>
    /// This method updates the introduction text. Since there are only two lines of text, there is no array needed. 
    /// </summary>
    public void UpdateIntroText()
    {
        introText.text = "Not everything is as it seems...";
    }
    
    /// <summary>
    /// This method updates the text that is shown when the receptionist speaks by using the spokenText array. 
    /// </summary>
    public void UpdateText()
    {
        textIndex++;
        try
        {
            spokenText.text = receptionistText[textIndex];
        }
        catch
        {
            spokenText.text = "no text";
        }
        // TODO: Fix typewriter
        //typeText(); // Activate the effect for the text
    }
    #endregion
    
    // This region contains methods for creating the typewriter effect of the text.
    #region TypewriterEffect
    /// <summary>
    /// This method can be called when the typewriter effect is wanted for the text.
    /// This method uses the coroutine method below. 
    /// </summary>
    private void typeText()
    {
        text = spokenText.text;
        spokenText.text = "";
        typeWriterCoroutine = StartCoroutine(TypeWriterCoroutine());
    }
    
    /// <summary>
    /// Method that actually performs the typewriter effect. 
    /// </summary>
    /// TODO: Ensure this doesnt bug out and is consistent on different systems
    /// Issue: It types the same letter multiple times sometimes
    IEnumerator TypeWriterCoroutine()
    {
        for (int i = 0; i < text.Length; i++)
        {
            spokenText.text += text[i];
            yield return new WaitForSeconds(typingSpeed);
        }
    }
    
    #endregion
    
}

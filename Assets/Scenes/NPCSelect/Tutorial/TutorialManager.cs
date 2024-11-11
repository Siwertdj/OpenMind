using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private PlayableDirector TutorialTimeline;
    
    [SerializeField] private Button           HelpButton;
    [SerializeField] private GameObject       ContinueButton;
    
    [SerializeField] private Image            TextBox; // Background of the text. 
    [SerializeField] private string[]         TutorialText; // Contains the text that will be shown. 
    [SerializeField] private TMP_Text         Text; // The gameobject that will show the text on the screen. 
    private                  int              TextIndex = 0; // Keeps track of which text to show.  
    
    /// <summary>
    /// This method is called when the help button is clicked. 
    /// </summary>
    public void StartTutorial()
    {
        TutorialTimeline.time = 0; // When the button is clicked again, the tutorial has to play from the start. 
        TutorialTimeline.Play();
        HelpButton.gameObject.SetActive(false); // When tutorial is playing, hide the help button. 
    }
    
    /// <summary>
    /// This method resumes the tutorial timeline when it is paused. 
    /// </summary>
    public void PlayTutorial()
    {
        TutorialTimeline.Play();
        ContinueButton.SetActive(false);
    }
    
    /// <summary>
    /// This method pauses the tutorial timeline when it is playing. 
    /// </summary>
    public void PauseTutorial()
    {
        TutorialTimeline.Pause();
        ContinueButton.SetActive(true);
    }
    
    /// <summary>
    /// This method is called after the player clicks the screen such that new text will be shown. 
    /// </summary>
    public void UpdateText()
    {
        PauseTutorial(); // Pause timeline such that player has time to read the text. 
        try
        {
            Text.text = TutorialText[TextIndex];
        }
        catch
        {
            Debug.LogError("No more tutorial text.");
            Text.text = TutorialText[0];
            TextIndex = 0;
        }
        // Make sure text is visible. 
        TextBox.gameObject.SetActive(true);
        Text.gameObject.SetActive(true);
        
        TextIndex++; // Increase textindex in order to prepare for the next text. 
    }
}

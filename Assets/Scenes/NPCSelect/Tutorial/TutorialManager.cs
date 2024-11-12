using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private PlayableDirector TutorialTimeline;
    
    [SerializeField] private Button     helpButton;
    [SerializeField] private GameObject continueButton;
    
    [SerializeField] private Image    textBox;       // Background of the text. 
    [SerializeField] private string[] tutorialText;  // Contains the text that will be shown. 
    [SerializeField] private TMP_Text text;          // The gameobject that will show the text on the screen. 
    private int      textIndex = 0;                 // Keeps track of which text to show.  
    [SerializeField] private     TMP_Text question;
    [SerializeField] private GameObject[] objectives;
    private GameObject objective; 
    
    /// <summary>
    /// This method is called when the help button is clicked. 
    /// </summary>
    public void StartTutorial()
    {
        question.gameObject.SetActive(false);
        TutorialTimeline.time = 0; // When the button is clicked again, the tutorial has to play from the start. 
        TutorialTimeline.Play();
        helpButton.gameObject.SetActive(false); // When tutorial is playing, hide the help button. 
    }
    
    /// <summary>
    /// This method resumes the tutorial timeline when it is paused. 
    /// </summary>
    public void PlayTutorial()
    {
        TutorialTimeline.Play();
        continueButton.SetActive(false);
    }
    
    /// <summary>
    /// This method pauses the tutorial timeline when it is playing. 
    /// </summary>
    public void PauseTutorial()
    {
        TutorialTimeline.Pause();
        continueButton.SetActive(true);
    }
    
    /// <summary>
    /// This method is called after the player clicks the screen such that new text will be shown. 
    /// </summary>
    public void UpdateText()
    {
        PauseTutorial(); // Pause timeline such that player has time to read the text. 
        objective.gameObject.SetActive(false);
        try
        {
            text.text = tutorialText[textIndex];
        }
        catch
        {
            Debug.LogError("No more tutorial text.");
            text.text = tutorialText[0];
            textIndex = 0;
        }
        // Make sure text is visible. 
        textBox.gameObject.SetActive(true);
        text.gameObject.SetActive(true);
        
        textIndex++; // Increase textindex in order to prepare for the next text. 
    }
    
    public void ShowObjective()
    {
        PauseTutorial();
        objective = objectives[0]; // TODO: when there are more stories, the game has to know which objective needs to be shown
        objective.gameObject.SetActive(true);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] public PlayableDirector TutorialTimeline;
    
    [SerializeField] public Button continueButton; 
    
    [SerializeField] private Image    textBox;       // Background of the text. 
    [SerializeField] public string[] tutorialText;  // Contains the text that will be shown. 
    [SerializeField] public TMP_Text text;          // The gameobject that will show the text on the screen. 
    private int textIndex;                           // Keeps track of which text to show.  
    
    // Variables for showing the objective of the game. 
    [SerializeField] private string[] objectives;
    [SerializeField] private Image    objectiveBox;
    [SerializeField] private TMP_Text objectiveText;
    
    private Button tutorialButton;
    private Button notebookButton; 
    
    private void Start()
    {
        StartTutorial();
        GameObject tutorial = GameObject.Find("HelpButton");
        tutorialButton = tutorial.GetComponentInChildren<Button>();
        
        GameObject notebook = GameObject.Find("NotebookButton");
        notebookButton = notebook.GetComponentInChildren<Button>();
    }
    
    public void ActivateNotebookTutorial()
    {
        PauseTutorial();
        UpdateTutorialText();
        continueButton.enabled = false;                         // Only the notebook button can be clicked. 
        notebookButton.onClick.AddListener(UpdateTutorialText); // Temporarily add the ability to update the text to the notebookbutton. 
        //notebookButton.enabled = true;                          // Make sure notebook button can be clicked. 
    }
    
    private void DeactivateNotebookTutorial()
    {
        notebookButton.onClick.RemoveListener(UpdateTutorialText); // Remove the ability to update the text
        continueButton.enabled = true;                     // Make sure the player can tap the screen to continue again. 
        //notebookButton.enabled = false;                    // Make sure the player can not enter the notebook during the tutorial. 
    }
    
    /// <summary>
    /// This method is called when the help button is clicked.
    /// It makes sure game elements enter a 'tutorial mode'. 
    /// </summary>
    public void StartTutorial()
    {
        TutorialTimeline.time = 0;              // When the button is clicked again, the tutorial has to play from the start. 
        textIndex = 0;
        TutorialTimeline.Play();
     }
    
    /// <summary>
    /// This method resumes the tutorial timeline when it is paused. 
    /// </summary>
    public void PlayTutorial()
    {
        TutorialTimeline.Play();
        continueButton.gameObject.SetActive(false);
    }
    
    public void StopTutorial()
    {
        tutorialButton.onClick.Invoke();
    }
    
    /// <summary>
    /// This method pauses the tutorial timeline when it is playing. 
    /// </summary>
    public void PauseTutorial()
    {
        TutorialTimeline.Pause();
        continueButton.gameObject.SetActive(true);
    }
    
    /// <summary>
    /// This method is called after the player clicks the screen such that new text will be shown. 
    /// </summary>
    public void UpdateTutorialText()
    {
        PauseTutorial(); // Pause timeline such that player has time to read the text. 
        objectiveBox.gameObject.SetActive(false);
        DeactivateNotebookTutorial();
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
    
    /// <summary>
    /// This method is called at the start of the tutorial.
    /// When the player has already played one cycle, the objective does not need to be shown again. 
    /// </summary>
    public void ShowObjective()
    {
        text.gameObject.SetActive(false); // Make sure the tutorialtext is hidden. 
        PauseTutorial();
        objectiveText.text = objectives[0];
        // TODO: when there are more stories, the game has to know which objective needs to be shown
        objectiveBox.gameObject.SetActive(true);
    }
    
   }
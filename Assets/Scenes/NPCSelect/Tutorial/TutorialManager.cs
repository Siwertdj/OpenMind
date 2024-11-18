using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private PlayableDirector TutorialTimeline;
    
    [SerializeField] private GameObject notebookHighlight; // Arrow to indicate the notebook button.
    [SerializeField] private Button     helpButton;
    [SerializeField] private GameObject continueButtonParent; // GameObject that stores the continuebutton
    private                  Button     continueButton; 
    private                  Button     notebookButton;
    
    [SerializeField] private Canvas tutorialCanvas; 
    [SerializeField] private Image    textBox;       // Background of the text. 
    [SerializeField] private string[] tutorialText;  // Contains the text that will be shown. 
    [SerializeField] private TMP_Text text;          // The gameobject that will show the text on the screen. 
    [SerializeField] private     TMP_Text question;  // The question that is shown on top of the screen in the NPC select scene. 
    private int textIndex;                           // Keeps track of which text to show.  
    
    // Variables for showing the objective of the game. 
    [SerializeField] private GameObject[] objectives;
    private GameObject objective;
    private bool objectiveShown = false;
    
    /// <summary>
    /// This method is called at the start of the tutorial. It initializes the two buttons. 
    /// </summary>
    public void Start()
    {
        GameObject notebook = GameObject.Find("NotebookButton");
        notebookButton = notebook.GetComponentInChildren<Button>();
        notebookButton.enabled = false;
        continueButton = continueButtonParent.GetComponentInChildren<Button>();
    }
    
    
    /// <summary>
    /// This method is called when the help button is clicked.
    /// It makes sure game elements enter a 'tutorial mode'. 
    /// </summary>
    public void StartTutorial()
    {
        tutorialCanvas.gameObject.SetActive(true);
        question.gameObject.SetActive(false);   // Hide the question when the tutorial is playing, to keep the screen more clear. 
        TutorialTimeline.time = 0;              // When the button is clicked again, the tutorial has to play from the start. 
        textIndex = 0;
        TutorialTimeline.Play();
        helpButton.gameObject.SetActive(false); // When tutorial is playing, hide the help button. 
    }
    
    /// <summary>
    /// This method is called at the end of the tutorial to make sure all game elements regain
    /// their normal functionality and exit tutorial mode. 
    /// </summary>
    public void StopTutorial()
    {
        tutorialCanvas.gameObject.SetActive(false);
        notebookButton.enabled = true;
    }
    
    /// <summary>
    /// This method resumes the tutorial timeline when it is paused. 
    /// </summary>
    public void PlayTutorial()
    {
        TutorialTimeline.Play();
        continueButtonParent.SetActive(false);
    }
    
    /// <summary>
    /// This method pauses the tutorial timeline when it is playing. 
    /// </summary>
    public void PauseTutorial()
    {
        TutorialTimeline.Pause();
        continueButtonParent.SetActive(true);
    }
    
    /// <summary>
    /// This method is called after the player clicks the screen such that new text will be shown. 
    /// </summary>
    public void UpdateText()
    {
        PauseTutorial(); // Pause timeline such that player has time to read the text. 
        objective.gameObject.SetActive(false);
        notebookHighlight.SetActive(false);
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
        if (!objectiveShown)
        {
            PauseTutorial();
            objective = objectives[0]; // TODO: when there are more stories, the game has to know which objective needs to be shown
            objective.gameObject.SetActive(true);
            objectiveShown = true; // Make sure the objective is not shown again when the player clicks the 'help' button. 
        }
        // This method does nothing when the objective does not need to be shown. 
    }
    
    // This region contains methods for the tutorial of the notebook 
    #region NotebookTutorial
    
    /// <summary>
    /// This method is called when the notebook tutorial needs to be started
    /// It activates the notebook button and makes sure the right text is loaded. 
    /// </summary>
    public void ActivateNotebookTutorial()
    {
        PauseTutorial();
        UpdateText();
        continueButton.enabled = false;                 // Only the notebook button can be clicked. 
        notebookButton.onClick.AddListener(UpdateText); // Temporarily add the ability to update the text to the notebookbutton. 
        notebookButton.enabled = true;                  // Make sure notebook button can be clicked. 
        notebookHighlight.SetActive(true);
    }
    
    private void DeactivateNotebookTutorial()
    {
        notebookButton.onClick.RemoveListener(UpdateText); // Remove the ability to update the text
        continueButton.enabled = true;                     // Make sure the player can tap the screen to continue again. 
        notebookButton.enabled = false;                    // Make sure the player can not enter the notebook during the tutorial. 
    }
    
    #endregion
}

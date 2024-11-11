using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public  PlayableDirector TutorialTimeline;
    public  Button           HelpButton;
    public  GameObject       ContinueButton; 
    public  Image            TextBox; 
    public  string[]         TutorialText;
    public  TMP_Text         Text;
    private int              TextIndex = 0; 
    
    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    public void UpdateText()
    {
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
        TextBox.gameObject.SetActive(true);
        Text.gameObject.SetActive(true);
        TextIndex++; 
        PauseTutorial();
    }
    
    public void StartTutorial()
    {
        TutorialTimeline.time = 0;
        TutorialTimeline.Play();
        HelpButton.gameObject.SetActive(false);
    }
    
    public void PlayTutorial()
    {
        TutorialTimeline.Play();
        ContinueButton.SetActive(false);
    }
    
    public void PauseTutorial()
    {
        TutorialTimeline.Pause();
        ContinueButton.SetActive(true);
    }
}

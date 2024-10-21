using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuManager : MonoBehaviour
{
    //TODO: The name of this script is too generic. It only applies to the Start-menu.
    //TODO: Rename, or rewrite for it to be generic (e.g. through GameEvents)
    public GameObject ContinueButton;
    
    [Header("Canvases")] 
    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private GameObject skipPrologueCanvas;
    
    // Start is called before the first frame update
    void Start()
    {
        // Continue button is only clickable when there are saves to be loaded
        // If there are no saves, disable the button
        if (!FilePathConstants.DoesSaveFileLocationExist()) ContinueButton.SetActive(false);
        mainMenuCanvas.SetActive(true);
    }
    
    public void OpenSkipProloguePrompt()
    {
        // Change menu's
        mainMenuCanvas.SetActive(false);
        skipPrologueCanvas.SetActive(true);
        
    }

    public void StartPrologue()
    {
        SceneManager.LoadScene("PrologueScene");
    }
    
    public void SkipPrologue()
    {
        SceneManager.LoadScene("StorySelectScene");
    }
}

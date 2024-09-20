using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    public bool savesPresent;
    public GameObject ContinueButton;

    // Start is called before the first frame update
    void Start()
    {
        // Continue button is only clickable when there are saves to be loaded
        // If there are no saves, disable the button
        if (!savesPresent) ContinueButton.SetActive(false);
    }

    // Currently there is no way to save the game and thus now way to access any save-files
    // TODO: When accessing and loading save-files has been implemented link that functionality to this button
    public void ContinueButtonClick()
    {
        
    }

    // The "New Game"-button just uses GameManager.Start to start a new game
    public void NewGameButtonClick()
    {
        GameManager.gm.RestartStoryScene();
    }
}

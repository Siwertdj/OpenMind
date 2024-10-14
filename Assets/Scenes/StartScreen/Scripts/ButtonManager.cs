using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    //TODO: The name of this script is too generic. It only applies to the Start-menu.
    //TODO: Rename, or rewrite for it to be generic (e.g. through GameEvents)
    public bool savesPresent;
    public GameObject ContinueButton;

    // Start is called before the first frame update
    void Start()
    {
        // Continue button is only clickable when there are saves to be loaded
        // If there are no saves, disable the button
        if (!savesPresent) ContinueButton.SetActive(false);
    }
    
    public void NewGameButtonClick()
    {
        GameManager.gm.StartGame();
    }
}

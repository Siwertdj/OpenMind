using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    public bool savesPresent;
    public GameObject ContinueButton;
    public GameObject StartScreenButtons;
    public GameObject ChooseStoryButtons;

    // Start is called before the first frame update
    void Start()
    {
        // Continue button is only clickable when there are saves to be loaded
        // If there are no saves, disable the button
        if (!savesPresent) ContinueButton.SetActive(false);
    }
    
    public void NewGameButtonClick()
    {
        StartScreenButtons.SetActive(false);
        ChooseStoryButtons.SetActive(true);
    }
    
    public void Story1ButtonClick()
    {
        GameManager.gm.StartGame();
    }
    
    public void Story2ButtonClick()
    {
        GameManager.gm.StartGame();
    }
    
    public void Story3ButtonClick()
    {
        GameManager.gm.StartGame();
    }
}

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
    
    /// <summary>
    /// Deactivates the buttons on the startscreen and activates a new set of buttons that let the player choose which story they want to play.
    /// </summary>
    public void NewGameButtonClick()
    {
        StartScreenButtons.SetActive(false);
        ChooseStoryButtons.SetActive(true);
    }
    
    
    // TODO: Meegeven welke story is gekozen, nog niet duidelijk hoe we dat opslaan
    // TODO: Heeft de gekozen story invloed op de prologue, prologue afspelen voor of na het kiezen van een story?
    
    
    // The onclick methods for the three story buttons
    // Calls the startgame method of gm and tells it which story has been picked
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

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopUpManager : MonoBehaviour
{
    public  Canvas          popUpCanvas;
    public  GameObject      popUpButtonCanvas;
    public  TextMeshProUGUI popUpTitleText;
    public  TextMeshProUGUI popUpText;
    public  Image           background;
    private DateTime        startTime;
    private bool            closeOnReceivedNotebook;

    public void OpenPopUp(Component sender, params object[] data)
    {
        // set popup text
        string text = "no text found.";
        if (data[0] is string) 
            text = (string)data[0];
        popUpText.text = text;
        
        // set background color and opacity
        Color color = new Color(0,0,0, 0.9f);
        background.GetComponentInChildren<Image>().color = color;
        
        // set popup type
        if (data.Length > 1)
        {
            if ((bool)data[1])
            {
                // popup with button
                closeOnReceivedNotebook = false;
                popUpButtonCanvas.SetActive(true);
                popUpTitleText.text = "Error";
                startTime = DateTime.Now;
            }
            else
            {
                // popup without button, closes when notebook is received
                closeOnReceivedNotebook = true;
                popUpButtonCanvas.SetActive(false);
                popUpTitleText.text = "Notification";
            }
            
        }
        popUpCanvas.enabled = true;
    }

    public void ClosePopUp() 
    {
        // Make sure the player doesn't accidentally click the popup away before reading it.
        if (!closeOnReceivedNotebook && DateTime.Now.Subtract(startTime).Seconds >= 2)
        {
            popUpText.text = string.Empty;
            popUpCanvas.enabled = false;
            popUpButtonCanvas.SetActive(false);
        }
    }

    public void Update()
    {
        if (closeOnReceivedNotebook && MultiplayerManager.mm.playerReceivedNotebook)
        {
            popUpText.text = string.Empty;
            popUpCanvas.enabled = false;
            popUpButtonCanvas.SetActive(false);
        }
    }
}

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
    public  TextMeshProUGUI popUpText;
    public  Image           background;
    private DateTime        startTime;
    private bool            closeOnReceivedNotebook;

    public void OpenPopUp(Component sender, params object[] data)
    {
        startTime = DateTime.Now;
        
        string text = "no text found.";
        if (data[0] is string) 
            text = (string)data[0];
        popUpText.text = text;

        if (data.Length > 1 && data[1] is Color)
        {
            Color color = (Color)data[1];
            color.a = 0.75f; 
            background.GetComponentInChildren<Image>().color = color;
        }
        
        if (data.Length > 2)
        {
            closeOnReceivedNotebook = true;
            popUpButtonCanvas.SetActive(false);
        }
        else
        {
            closeOnReceivedNotebook = false;
            popUpButtonCanvas.SetActive(true);
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

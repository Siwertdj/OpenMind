using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopUpManager : MonoBehaviour
{
    public  Button          closePopUp;
    public  Canvas          popUpCanvas;
    public  TextMeshProUGUI popUpText;
    private DateTime        startTime;
    
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
            color.a = 0.9f;
            closePopUp.GetComponentInChildren<Image>().color = color;
        }

        popUpCanvas.enabled = true;
        closePopUp.interactable = true;
    }

    public void ClosePopUp() 
    {
        // Make sure the player doesn't accidentally click the popup away before reading it.
        if (DateTime.Now.Subtract(startTime).Seconds >= 1)
        {
            popUpText.text = string.Empty;
            popUpCanvas.enabled = false;
            closePopUp.interactable = false;
        }
    }
}

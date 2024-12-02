using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class PopUpManager : MonoBehaviour
{
    public Canvas popUpCanvas;
    public bool guaranteeIcon;

    public void OpenPopUp()
    {
        Debug.Log("popup");
        popUpCanvas.enabled = true;
        
    }

    public void ClosePopUp()
    {
        StartCoroutine(IconWait());
    }

    IEnumerator IconWait()
    {
        if (guaranteeIcon)
            yield return new WaitForSeconds(2);


        Debug.Log("close popup");
        popUpCanvas.enabled = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class PopUpManager : MonoBehaviour
{
    public Canvas popUpCanvas;
    public float guaranteeLoadDuration;

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
        if (popUpCanvas.enabled)
            yield return new WaitForSeconds(guaranteeLoadDuration);


        Debug.Log("close popup");
        popUpCanvas.enabled = false;
    }
}

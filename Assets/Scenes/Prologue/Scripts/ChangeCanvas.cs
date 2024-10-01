using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class ChangeCanvas : MonoBehaviour
{
    // Array that stores all the canvases that are part of the prologue. 
    public GameObject[] Canvases;
    
    /// <summary>
    /// This method switches between canvases. Each canvas of the array canvases is turned
    /// off, except for the canvas at location index. 
    /// </summary>
    /// <param name="index"></param>
    public void Change(int index)
    {
        foreach (GameObject canvas in Canvases)
        {
            canvas.SetActive(false);
        }
        Canvases[index].SetActive(true);
    }

}

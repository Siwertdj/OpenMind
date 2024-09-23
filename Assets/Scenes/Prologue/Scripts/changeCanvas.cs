using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class changeCanvas : MonoBehaviour
{

    public GameObject[] canvases;

    public void nextCanvas(int index)
    {
        foreach (GameObject canvas in canvases)
        {
            canvas.SetActive(false);
        }
        canvases[index].SetActive(true);
    }

}

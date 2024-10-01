using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeImage : MonoBehaviour
{
    // 
    public GameObject BlackNWhiteImage;
    public GameObject ColorImage;
    private bool color = false;

    public float DisplayTime = 2f; // Determines how many seconds the image is visible. 
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    public void Change()
    {
        if (color)
        {
            BlackNWhiteImage.SetActive(false);
            ColorImage.SetActive(true);
        }
        else
        {
            BlackNWhiteImage.SetActive(true);
            ColorImage.SetActive(false);
        }
        color = !color; 
    }

    public void Start()
    {
        StartCoroutine(ShowAndHide());
    }
    
    private IEnumerator ShowAndHide()
    {
        ColorImage.SetActive(true);
        yield return new WaitForSeconds(DisplayTime);
        ColorImage.SetActive(false);
    }

}

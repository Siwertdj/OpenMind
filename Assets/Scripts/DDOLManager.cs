using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDOLManager : MonoBehaviour
{
    private void Awake()
    {
        //if (GameObject.FindGameObjectsWithTag("DDOLManager").Length > 1)
        if (FindObjectsOfType<DDOLManager>().Length > 1)
        {
            Destroy(gameObject);   
        }
        else
        {
            // Make this group-object DDOL, and therefore also its children become DDOL
            DontDestroyOnLoad(this);
            // Set all its children to be active
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(true);
            }
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDOLManager : MonoBehaviour
{
    private void Awake()
    {
        if(FindObjectsOfType<DDOLManager>().Length > 1)
            Destroy(this);
        else
        {
            DontDestroyOnLoad(this);
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(true);
            }
        }
    }
}

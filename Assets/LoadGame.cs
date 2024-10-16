using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadGame : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("prologue end");
        GameManager.gm.StartGame();
    }
}

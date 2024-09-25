using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameWinManager : MonoBehaviour
{
    public void Restart()
    {
        GameManager.gm.RestartStoryScene();
    }

    public void Quit()
    {
        GameManager.gm.EndGame();
    }
}

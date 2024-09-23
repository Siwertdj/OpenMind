using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameWinManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Restart()
    {
        GameManager.gm.RestartStoryScene();
    }

    public void Quit()
    {
        GameManager.gm.EndGame();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EpilogueManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    

    /// <summary>
    /// Load the GameWinScene or GameOverScene depending on the choice the player made.
    /// </summary>
    void LoadEndScreen()
    {
        SceneController.sc.ToggleEpilogueScene();
        // Load the GameWinScene if hasWon = true, else load the GameOverScene
        if (GameManager.gm.hasWon)
            SceneController.sc.ToggleGameWinScene();
        else
            SceneController.sc.ToggleGameOverScene();
    }
}

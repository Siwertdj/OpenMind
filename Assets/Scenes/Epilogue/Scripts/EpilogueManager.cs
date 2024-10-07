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
        // If the player has won, start the GameWin selectionType, else start the GameOver selectionType
        CharacterInstance culprit = GameManager.gm.GetCulprit();
        
        // If the player has won, the target scene is 'GameWin', else 'GameOver'.
        SceneController.SceneName targetScene =
            GameManager.gm.hasWon
                ? SceneController.SceneName.GameWinScene
                : SceneController.SceneName.GameOverScene;
                
        // Transition to the right scene.
        SceneController.sc.TransitionScene(
            SceneController.SceneName.EpilogueScene,
            targetScene, 
            SceneController.TransitionType.Transition);
    }
}

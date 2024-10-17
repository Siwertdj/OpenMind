using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    public bool savesPresent;
    public GameObject ContinueButton;

    // Start is called before the first frame update
    void Start()
    {
        // Continue button is only clickable when there are saves to be loaded
        // If there are no saves, disable the button
        if (!savesPresent) ContinueButton.SetActive(false);
    }
    
    public async void NewGameButtonClick()
    {
        if (!GameManager.gm.skipPrologue)
        {
            SceneController.sc.TransitionScene(
                SceneController.SceneName.StartScreenScene,
                SceneController.SceneName.PrologueScene,
                SceneController.TransitionType.Transition);
        }
        else
        {
            await SceneController.sc.TransitionScene(
                SceneController.SceneName.StartScreenScene,
                SceneController.SceneName.PrologueScene,
                SceneController.TransitionType.Transition);
            GameManager.gm.StartGame();
        }
    }
}

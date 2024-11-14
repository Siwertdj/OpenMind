// This program has been developed by students from the bachelor Computer Science at Utrecht University within the Software Project course.
// © Copyright Utrecht University (Department of Information and Computing Sciences)
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenuManager : MonoBehaviour
{
    /// <summary>
    /// Closes the GameMenu-scene, and calls the UIManager.CloseMenu()-method.
    /// </summary>
    public async void ReturnToGame()
    {
        // Close the GameMenu, and return to the active scene ( Dialogue or NPCSelect),
        // which we choose by getting the activescene. 
        // If GameMenu was open while we were not in one of these scenes, it should be an illegal 
        // transition.
        await SceneController.sc.TransitionScene(
            SceneController.SceneName.GameMenuScene, 
            SceneController.sc.GetSceneName(SceneManager.GetActiveScene()), 
            SceneController.TransitionType.Unload);
        
        
        GameManager.gm.GetComponent<UIManager>().CloseMenu();
    }

    /// <summary>
    /// Calls Save.SaveGame() to save the game.
    /// </summary>
    public void SaveGame()
    {
        Save.Saver.SaveGame();
    }

    /// <summary>
    /// Calls Load.LoadGame() to load the game, then calls ReturnToGame() to close the GameMenu.
    /// </summary>
    public void LoadGame()
    {
        // Load Game
        Load.Loader.LoadButtonPressed();
        // Call ReturnToGame(), so the menu closes, the buttons return, and the game is unpaused.
        ReturnToGame();
    }

    /// <summary>
    /// Additively loads the SettingsMenu-scene
    /// </summary>
    public void OpenSettings()
    {
        // Load SettingsMenu-scene, so it loads on top of all other scenes.
        // _ = throws away the await so we dont get an error
        _ = SceneController.sc.TransitionScene(
            SceneController.SceneName.GameMenuScene, 
            SceneController.SceneName.SettingsScene, 
            SceneController.TransitionType.Additive);
    }

    /// <summary>
    /// Single-Loads the StartScreen-scene. THis unloads all additive scenes, and destroys the DDOL-objects.
    /// </summary>
    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("StartScreenScene");
        // Destroy all DontDestroyOnLoad-objects
        // TODO: This doesnt want to work
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("DDOL"))
        {
            Destroy(obj);
        }
    }
}

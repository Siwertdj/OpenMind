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
        // Close the GameMenu.
        await SceneController.sc.TransitionScene(
            SceneController.SceneName.GameMenuScene, 
            SceneController.SceneName.Loading, 
            SceneController.TransitionType.Unload);
        
        // After that is done, we call UIManager to finish the operation.
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
        // Call ReturnToGame(), so the menu closes, the buttons return, and the game is unpaused.
        ReturnToGame();
        // Load Game
        Load.Loader.LoadButtonPressed();
    }

    /// <summary>
    /// Additively loads the SettingsMenu-scene
    /// </summary>
    public void OpenSettings()
    {
        // Load SettingsMenu-scene, so it loads on top of all other scenes.
        // _ = throws away the await so we dont get an error
        _ = SceneController.sc.TransitionScene(
            SceneController.SceneName.Loading, 
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
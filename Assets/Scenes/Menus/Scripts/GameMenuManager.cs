// This program has been developed by students from the bachelor Computer Science at Utrecht University within the Software Project course.
// © Copyright Utrecht University (Department of Information and Computing Sciences)
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenuManager : MonoBehaviour
{
    public GameButton saveButton; // savebutton reference to change text when clicked
    public GameEvent startLoadIcon; // Event to raise to start loading anim

    /// <summary>
    /// Closes the GameMenu-scene, and calls the UIManager.CloseMenu()-method.
    /// </summary>
    public async void ReturnToGame()
    {
        // Close the GameMenu, and return to the active scene (Dialogue or NPCSelect),
        // which we choose by getting the activescene. 
        // If GameMenu was open while we were not in one of these scenes, it should be an illegal 

        Scene activeScene = SceneManager.GetSceneByName("NPCSelectScene");
        if (!activeScene.isLoaded)
        {
            activeScene = SceneManager.GetSceneByName("DialogueScene");
            if (!activeScene.isLoaded)
            {
                Debug.LogError("GameMenu can not be closed, as Dialogue and NPCSelect are not loaded.");
                return;
            }
        }
        
        // transition.
        await SceneController.sc.TransitionScene(
            SceneController.SceneName.GameMenuScene, 
            SceneController.SceneName.Loading, 
            SceneController.TransitionType.Unload,
            false);
        
        // After that is done, we call UIManager to finish the operation.
        GameManager.gm.GetComponent<UIManager>().CloseMenu();        
    }

    /// <summary>
    /// Calls Save.SaveGame() to save the game and changes button for feedback.
    /// </summary>
    public void SaveGame()
    {
        Save.Saver.SaveGame();

        // grey out save button and change text to show player game was saved
        saveButton.interactable = false;
        var saveButtonTextbox = saveButton.GetComponentInChildren<TextMeshProUGUI>();
        saveButtonTextbox.text = "Game Saved!";
    }

    /// <summary>
    /// Calls Load.LoadGame() to load the game, then calls ReturnToGame() to close the GameMenu.
    /// </summary>
    public void LoadGame()
    {
        // Call ReturnToGame(), so the menu closes, the buttons return, and the game is unpaused.
        ReturnToGame();

        // Start loading animation
        startLoadIcon.Raise(this);

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
            SceneController.TransitionType.Additive,
            true);
    }

    /// <summary>
    /// Single-Loads the StartScreen-scene. THis unloads all additive scenes, and destroys the DDOL-objects.
    /// </summary>
    public void ReturnToMainMenu()
    {
        // Store DDOLs to be destroyed after transition
        var DDOLs = GameObject.FindGameObjectWithTag("DDOLManager");
        var toolbox = GameObject.FindGameObjectWithTag("Toolbox");

        // Transition to start screen scene
        SceneManager.LoadScene("StartScreenScene");

        // TODO: Instead of using SceneManager, this should use the SceneController
        // _ = SceneController.sc.TransitionScene(
        //    SceneController.SceneName.Loading,
        //    SceneController.SceneName.StartScreenScene,
        //    SceneController.TransitionType.Transition, true);

        // Destroy toolbox
        Destroy(toolbox);
    }
}
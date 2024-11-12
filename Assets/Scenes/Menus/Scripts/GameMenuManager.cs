// This program has been developed by students from the bachelor Computer Science at Utrecht University within the Software Project course.
// © Copyright Utrecht University (Department of Information and Computing Sciences)
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenuManager : MonoBehaviour
{
    public void ReturnToGame()
    {
        SceneManager.UnloadSceneAsync("GameMenuScene");
        
        GameManager.gm.GetComponent<UIManager>().CloseMenu();
    }

    public void SaveGame()
    {
        Save.Saver.SaveGame();
    }

    public void LoadGame()
    {
        // Load Game
        Load.Loader.LoadButtonPressed();
        // Call ReturnToGame(), so the menu closes, the buttons return, and the game is unpaused.
        ReturnToGame();
    }

    public void OpenSettings()
    {
        // Load SettingsMenu-scene, so it loads on top of all other scenes.
        SceneManager.LoadScene("SettingsScene", LoadSceneMode.Additive);
    }

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

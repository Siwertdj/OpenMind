// This program has been developed by students from the bachelor Computer Science at Utrecht University within the Software Project course.
// © Copyright Utrecht University (Department of Information and Computing Sciences)
using UnityEngine;
using UnityEngine.SceneManagement;

public class MultiplayerMenuManager : MonoBehaviour
{
    [Header("Canvases")] 
    [SerializeField] private GameObject multiplayerCanvas;
    [SerializeField] private GameObject hostCanvas;
    [SerializeField] private GameObject joinCanvas;

    /// <summary>
    /// Activates the buttons for hosting a game.
    /// </summary>
    public void OpenHostMenu()
    {
        multiplayerCanvas.SetActive(false);
        hostCanvas.SetActive(true);
    }

    /// <summary>
    /// Activates the buttons for joining a game.
    /// </summary>
    public void OpenJoinMenu()
    {
        multiplayerCanvas.SetActive(false);
        joinCanvas.SetActive(true);
    }

    /// <summary>
    /// Opens the settings menu.
    /// </summary>
    public void OpenSettings()
    {
        SceneManager.LoadScene("SettingsScene", LoadSceneMode.Additive);
    }

    /// <summary>
    /// Goes back to the main menu.
    /// </summary>
    public void ReturnMain()
    {
        SceneManager.UnloadSceneAsync("MultiplayerScreenScene");
    }

    public void BackToMultiplayer()
    {
        hostCanvas.SetActive(false);
        joinCanvas.SetActive(false);
        multiplayerCanvas.SetActive(true);
    }
    
    public void BackToMultiplayerFromJoin()
    {
        joinCanvas.SetActive(false);
        multiplayerCanvas.SetActive(true);
    }
}

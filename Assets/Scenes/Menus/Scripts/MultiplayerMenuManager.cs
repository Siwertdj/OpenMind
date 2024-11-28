// This program has been developed by students from the bachelor Computer Science at Utrecht University within the Software Project course.
// © Copyright Utrecht University (Department of Information and Computing Sciences)

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MultiplayerMenuManager : MonoBehaviour
{
    [Header("Canvases")] 
    [SerializeField] private GameObject multiplayerCanvas;
    [SerializeField] private GameObject hostCanvas;
    [SerializeField] private GameObject joinCanvas;
    [SerializeField] private GameObject storyCanvas;


    [Header("Paramaters")] 
    [SerializeField] private float maxPlayers = 10;
    [SerializeField] private TextMeshProUGUI maxPlayersText;


    private string classCode;
    

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
    
    /// <summary>
    /// Goes back to the main multiplayer menu from the host or join menu.
    /// </summary>
    public void BackToMultiplayer()
    {
        hostCanvas.SetActive(false);
        joinCanvas.SetActive(false);
        multiplayerCanvas.SetActive(true);
    }


    #region Host

    /// <summary>
    /// Activates the buttons for hosting a game.
    /// </summary>
    public void OpenHostMenu()
    {
        multiplayerCanvas.SetActive(false);
        hostCanvas.SetActive(true);
    }
    
    /// <summary>
    /// Sets the max players a session can have using a slider.
    /// </summary>
    /// <param name="num">The value of the slider.</param>
    public void SetMaxPlayers(float num)
    {
        maxPlayers = num;
    }

    /// <summary>
    /// Starts the game as the host.
    /// The host has to choose a story first
    /// </summary>
    public void StartAsHost()
    {
        hostCanvas.SetActive(false);
        storyCanvas.SetActive(true);
    }
    
    //TODO: the following methods are used to start the game as host
    //TODO: Implement when networking is ready
    
    public void StoryA()
    {
        
    }

    public void StoryB()
    {
        
    }

    public void StoryC()
    {
        
    }

    #endregion


    #region Join

    /// <summary>
    /// Activates the buttons for joining a game.
    /// </summary>
    public void OpenJoinMenu()
    {
        multiplayerCanvas.SetActive(false);
        joinCanvas.SetActive(true);
    }

    /// <summary>
    /// Sets the class code to the input from the player.
    /// </summary>
    /// <param name="code">The contents of the input field.</param>
    public void SetCode(string code)
    {
        classCode = code;
    }

    // TODO: this is where the player joins a game using a class code
    // TODO: implement when networking is ready
    /// <summary>
    /// 
    /// </summary>
    public void JoinGame()
    {
        
    }
    

    #endregion
    
    public void Update()
    {
        maxPlayersText.text = maxPlayers.ToString();
    }
}

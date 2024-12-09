﻿// This program has been developed by students from the bachelor Computer Science at Utrecht University within the Software Project course.
// © Copyright Utrecht University (Department of Information and Computing Sciences)
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// The manager class for the startscreen
/// </summary>
public class StartMenuManager : MonoBehaviour
{
    //TODO: The name of this script is too generic. It only applies to the Start-menu.
    //TODO: Rename, or rewrite for it to be generic (e.g. through GameEvents)
    public GameObject ContinueButton;
    public GameObject LoadingScreenManager;
    
    [Header("Canvases")] 
    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private GameObject skipPrologueCanvas;
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject popUpScreen;
    
    [Header("Events")]
    public GameEvent onGameLoaded;
    public GameEvent startLoadIcon;
    public GameEvent doPopUp;

    [Header("Copyright canvas")]
    public Canvas copyright;
    
    [Header("Resources")]
    [SerializeField] AudioClip startMenuMusic;
    private float startMenuMusicFadeInSpeed = 0f;
    
    /// <summary>
    /// Makes sure the continuebutton is only clickable when a save exists.
    /// If there are no saves, disable the button.
    /// </summary>
    void Start()
    {
        if (!FilePathConstants.DoesSaveFileLocationExist()) ContinueButton.SetActive(false);
        mainMenuCanvas.SetActive(true);
        
        // Keep the copyright text on the screen in all scenes
        DontDestroyOnLoad(copyright);

        // Make popup and loading available at any point in the game
        DontDestroyOnLoad(loadingScreen);
        DontDestroyOnLoad(popUpScreen);

        doPopUp.Raise(this, "Test test test");

        
        SettingsManager.sm.SwitchMusic(startMenuMusic, startMenuMusicFadeInSpeed);
    }

    /// <summary>
    /// Activates the prompt which asks the player to skip the prologue
    /// </summary>
    public void OpenSkipProloguePrompt()
    {
        // Change menu's
        mainMenuCanvas.SetActive(false);
        skipPrologueCanvas.SetActive(true);
        
    }

    /// <summary>
    /// Gets savedata and loads a game using that data
    /// </summary>
    public void ContinueGame()
    {
        SaveData saveData = Load.Loader.GetSaveData();
        StartCoroutine(LoadGame(saveData));
    }
    
    /// <summary>
    /// Starts an operation to load the game using saveData then unloads the current scene
    /// </summary>
    /// <param name="saveData">The data that needs to be loaded</param>
    /// <returns></returns>
    IEnumerator LoadGame(SaveData saveData)
    {
        // Start the loadscene-operation
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Loading", LoadSceneMode.Additive);
        
        // Within this while-loop, we wait until the scene is done loading. We check this every frame
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        
        onGameLoaded.Raise(this, saveData);
        
        // Finally, when the data has been sent, we then unload our currentscene
        SceneManager.UnloadSceneAsync("StartScreenScene");
    }

    /// <summary>
    /// Starts the prologuescene
    /// </summary>
    public void StartPrologue()
    {
        SceneManager.LoadScene("PrologueScene");
    }
    
    /// <summary>
    /// Skips the prologuescene and starts the storyselectscene instead
    /// </summary>
    public void SkipPrologue()
    {
        SceneManager.LoadScene("StorySelectScene");
    }

    public void OpenSettings()
    {
        SceneManager.LoadScene("SettingsScene", LoadSceneMode.Additive);
    }

    public void OpenMultiplayer()
    {
        SceneManager.LoadScene("MultiplayerScreenScene", LoadSceneMode.Additive);
    }
}
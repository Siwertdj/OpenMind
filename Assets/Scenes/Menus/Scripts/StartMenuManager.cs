using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuManager : MonoBehaviour
{
    //TODO: The name of this script is too generic. It only applies to the Start-menu.
    //TODO: Rename, or rewrite for it to be generic (e.g. through GameEvents)
    public GameObject ContinueButton;
    
    [Header("Canvases")] 
    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private GameObject skipPrologueCanvas;
    
    [Header("Events")]
    public GameEvent onGameLoaded;

    public Canvas copyright;
    
    // Start is called before the first frame update
    void Start()
    {
        // Continue button is only clickable when there are saves to be loaded
        // If there are no saves, disable the button
        if (!FilePathConstants.DoesSaveFileLocationExist()) ContinueButton.SetActive(false);
        mainMenuCanvas.SetActive(true);
        
        // Keep the copyright text on the screen in all scenes
        DontDestroyOnLoad(copyright);
    }
    
    public void OpenSkipProloguePrompt()
    {
        // Change menu's
        mainMenuCanvas.SetActive(false);
        skipPrologueCanvas.SetActive(true);
        
    }

    public void ContinueGame()
    {
        SaveData saveData = gameObject.GetComponent<Loading>().GetSaveData();
        StartCoroutine(LoadGame(saveData));
    }
    
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
        SceneManager.UnloadSceneAsync("StartScreenScene");  // unload this scene; no longer necessary
    }

    public void StartPrologue()
    {
        SceneManager.LoadScene("PrologueScene");
    }
    
    public void SkipPrologue()
    {
        SceneManager.LoadScene("StorySelectScene");
    }
}

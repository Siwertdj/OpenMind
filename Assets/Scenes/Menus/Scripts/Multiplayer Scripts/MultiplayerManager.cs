using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = System.Random;

public class MultiplayerManager : MonoBehaviour
{
    public static MultiplayerManager mm;
    
    public GameEvent       onGameLoaded;
    public NetworkSettings settings;
    public GameEvent       doPopup;
    
    private Host            host;
    private Client          client;
    private MultiplayerInit init;
    private bool            isSeedInitialized;
    private bool            isStoryInitialized;
    
    private Action<NotebookData> notebookAction;
    
    void Awake()
    {
        mm = this;
        DontDestroyOnLoad(gameObject);
        init = new MultiplayerInit();
    }

    public void HostGame(int storyID)
    {
        init.story = FindStory(storyID);
        
        // Create and activate the host
        host = gameObject.AddComponent<Host>();
        
        // Assign the settings
        host.AssignSettings(doPopup, settings);
        
        // Create a seed
        init.seed = new Random().Next(int.MaxValue);
        
        // Let clients connect to the game
        host.Lobby(storyID, init.seed);
    }
    
    private StoryObject FindStory(int storyID) => Resources.LoadAll<StoryObject>("Stories").First(s => s.storyID == storyID);
    
    public string GetClassCode() => host.CreateClassroomCode();

    public void JoinGame(string classCode)
    {
        // Create the client
        client = gameObject.AddComponent<Client>();
        
        // Assign the settings
        client.AssignSettings(doPopup, settings);
        
        // Connect to the host using the code
        client.EnterClassroomCode(classCode, AssignSeed, AssignStory);
    }
    
    private void AssignSeed(int seed)
    {
        init.seed = seed;
        isSeedInitialized = true;
        if (isStoryInitialized)
            StartGame();
    }
    
    private void AssignStory(int story)
    {
        
        isStoryInitialized = true;
        if (isSeedInitialized)
            StartGame();
    }

    public void StartGame()
    {
        StartCoroutine(LoadGame());
    }
    
    IEnumerator LoadGame()
    {
        // Start the loadscene-operation
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Loading", LoadSceneMode.Additive);
        
        // Within this while-loop, we wait until the scene is done loading. We check this every frame
        while (!asyncLoad.isDone)
            yield return null;
        
        onGameLoaded.Raise(this, init);
        
        // Finally, when the data has been sent, we then unload our currentscene
        SceneManager.UnloadSceneAsync("MultiplayerScreenScene");  // unload this scene; no longer necessary
    }
    
    public void SendNotebook()
    {
        notebookAction = receivedNotebook =>
        {
            GameManager.gm.multiplayerNotebookData = receivedNotebook;
        };
        
        client.SendNotebookData(notebookAction, 
            GameManager.gm.notebookData,
            GameManager.gm.currentCharacters);
    }
}

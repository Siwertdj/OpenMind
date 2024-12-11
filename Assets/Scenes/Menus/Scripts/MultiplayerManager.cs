using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MultiplayerManager : MonoBehaviour
{
    public static MultiplayerManager mm;
    private       Host               host;
    private       Client             client;
    public        NetworkSettings    settings;
    public        GameEvent          doPopup;

    private Action<int> seedAction;
    private Action<int> storyAction;

    private Action<NotebookData> notebookAction;
    
    void Awake()
    {
        mm = this;
        DontDestroyOnLoad(gameObject);
        
        seedAction = i => Debug.Log("seed action: " + i);
        storyAction = i => Debug.Log("story action: " + i);
    }

    public void HostGame(int storyID)
    {
        // Create and activate the host
        host = gameObject.AddComponent<Host>();
        
        // Assign the settings
        host.AssignSettings(doPopup, settings);
        
        // Create a seed
        int seed = DateTime.Now.Ticks.GetHashCode();
        
        // Let clients connect to the game
        host.Lobby(storyID, seed);
        
        // Start the notebook exchange
        host.ActivateNotebookExchange();
    }

    public string GetClassCode()
    {
        // Create a classcode using the local IP address
        return host.CreateClassroomCode();
    }

    public void JoinGame(string classCode)
    {
        // Create the client
        client = gameObject.AddComponent<Client>();
        
        // Assign the settings
        client.AssignSettings(doPopup, settings);
        
        // Connect to the host using the code
        client.EnterClassroomCode(classCode, seedAction, storyAction);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("PrologueScene");
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

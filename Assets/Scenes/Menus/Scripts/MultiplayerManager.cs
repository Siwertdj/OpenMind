using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = System.Random;

public class MultiplayerManager : MonoBehaviour
{
    public static MultiplayerManager mm;
    private       Host               host;
    private       Client             client;
    private       int                seed;
    private       int                storyID;
    
    void Awake()
    {
        mm = this;
        DontDestroyOnLoad(gameObject);
    }

    public void HostGame(int storyID)
    {
        this.storyID = storyID;
        
        // Create and activate the host
        host = gameObject.AddComponent<Host>();
        host.Activate();
        
        // Create a seed
        seed = DateTime.Now.Ticks.GetHashCode();
        
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
        // Connect to the host using the code
        client.EnterClassroomCode(classCode);
    }

    public void StartGame()
    {
        //SceneManager.LoadScene("PrologueScene");
        //SceneManager.LoadScene("StorySelectScene");
    }
}

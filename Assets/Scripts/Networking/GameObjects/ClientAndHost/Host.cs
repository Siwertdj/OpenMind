// This program has been developed by students from the bachelor Computer Science at Utrecht University within the Software Project course.
// © Copyright Utrecht University (Department of Information and Computing Sciences)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class Host : MonoBehaviour
{
    public static Host h;
    
    private IPAddress                  ownIP;
    private DataListener               listener;
    private int                        seed;
    private int                        storyID;
    private List<List<NetworkPackage>> notebooks = new ();
    

    void Awake()
    {
        h = this;
    }
    
    public void Activate()
    {
        ownIP = IPConnections.GetOwnIps()[0];
        listener = new DataListener(ownIP, IPConnections.Port);
    }

    public string CreateClassroomCode()
    {
        IPv4Converter converter = new IPv4Converter();
        return converter.ConvertToCode(ownIP);
    }

    public void Lobby(int story, int seed)
    {
        this.seed = seed;
        this.storyID = story;

        StartCoroutine(listener.DisplayAnyDebugs(0f));
        
        listener.AddOnAcceptConnectionsEvent(OnConnectionAccepted);
        listener.AddResponseTo("Seed", SendSeed);
        listener.AddResponseTo("StoryID", SendStoryID);
        
        StartCoroutine(listener.AcceptIncomingConnections(3f));
        StartCoroutine(listener.ListenForIncomingData(0.1f));
        StartCoroutine(listener.IsDisconnected(1f));
    }

    private void OnConnectionAccepted(object obj)
    {
        Debug.Log($"Host: Connected with client {((Socket)obj).RemoteEndPoint}");
    }

    private NetworkPackage SendSeed(List<NetworkPackage> arg)
    {
        Debug.Log("Host sending seed.");
        return NetworkPackage.CreatePackage(seed);
    }
    private NetworkPackage SendStoryID(List<NetworkPackage> arg)
    {
        Debug.Log("Host sending story id.");
        return NetworkPackage.CreatePackage(storyID);
    }

    #region Notebook
    public void ActivateNotebookExchange()
    {
        listener.AddOnDataReceivedEvent("NotebookData", ReceiveNotebookFromClient);
    }

    void ReceiveNotebookFromClient(object o)
    {
        notebooks.Add((List<NetworkPackage>)o);
        listener.AddResponseTo("NotebookData", ReturnAnotherNotebook);
    }

    private List<NetworkPackage> ReturnAnotherNotebook(List<NetworkPackage> data)
    {
        if (notebooks.Count == 0)
        {
            Debug.Log("No notebooks to return.");
            return null;
        }
        else if (notebooks.Count == 1)
        {
            if (notebooks[0] != data)
                return notebooks[0];
            else
            {
                Debug.Log("Only the clients notebook is in the list.");
                return null;
            }
        }
        else
        {
            int endList = notebooks.Count - 1;
            
            if (notebooks[endList] != data)
                return notebooks[endList];
            else 
                return notebooks[endList - 1];
        }
    }
    #endregion
}

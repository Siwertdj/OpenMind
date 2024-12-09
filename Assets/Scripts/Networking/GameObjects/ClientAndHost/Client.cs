// This program has been developed by students from the bachelor Computer Science at Utrecht University within the Software Project course.
// © Copyright Utrecht University (Department of Information and Computing Sciences)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

/// <summary>
/// Handles the client side of networking.
/// This script can be active and inactive (separate from the active and inactive of objects in unity.)
/// </summary>
public class Client : MonoBehaviour
{
    public static Client c;
    
    private DataSender              sender;
    private Action<NotebookData>    response;
    private Action<int>             storyID;
    private Action<int>             seed;
    //basically a copy from Gamemanager.gm.currentCharacters.
    //This is a separate variable to limit coupling as much as possible
    private List<CharacterInstance> activeCharacters;
    
    private const bool DebugMode = true; 
    
    private void Awake()
    {
        c = this;
    }
    
    public void EnterClassroomCode(string classroomCode)
    {
        IPAddress  hostAddress = null;
        IPv4Converter converter = new IPv4Converter();
        try
        {
            hostAddress = converter.ConvertToIPAddress(classroomCode);
        }
        catch (ArgumentException e)
        {
            Debug.LogError($"No error handling yet, go error: {e}");
        }
        
        if (hostAddress is null)
            return;
        
        sender = new DataSender(hostAddress, IPConnections.Port);
        sender.AddOnConnectionTimeoutEvent(ConnectionTimeoutError);
        
        // Ask for storyID and the seed when connected with the host
        sender.AddOnConnectEvent(OnConnectionWithHost);
        sender.AddOnReceiveResponseEvent("Initializer", ReceivedInitFromHost);
        
        StartCoroutine(sender.DisplayAnyDebugs(1f));
        StartCoroutine(sender.Connect(5f));
        StartCoroutine(sender.ListenForResponse(3f));
    }

    private void OnConnectionWithHost(object obj)
    {
        if (DebugMode)
            Debug.Log($"(Sender): Connected with the host.");
        sender.SendDataAsync("Initializer", NetworkPackage.CreatePackage("Plz give seed init data!"), 10f);
    }

    /// <summary>
    /// Called when no connection could be established.
    /// </summary>
    void ConnectionTimeoutError(object o)
    {
        Debug.LogError("No connection was made");
    }
    
    /// <summary>
    /// Sends notebook data to the host and handles the received notebook data.
    /// </summary>
    /// <param name="response">A function to assign the notebook data obtained by the host.</param>
    /// <param name="notebookData">The notebook data to send to the host.</param>
    /// <param name="currentCharacters">The list of current characters, this is required to correctly obtain the notes from each character.</param>
    public void SendNotebookData(Action<NotebookData> response, NotebookData notebookData, List<CharacterInstance> currentCharacters)
    {
        this.response = response;
        activeCharacters = currentCharacters;
        NotebookDataPackage package = new NotebookDataPackage(notebookData, currentCharacters);
        sender.AddOnAckTimeoutEvent("NotebookData", AcknowledgementTimeoutError);
        sender.SendDataAsync("NotebookData", package.CreatePackage(), 5f);
        sender.AddOnReceiveResponseEvent("NotebookResponse", ReceivedNotebookDataFromOther);
    }
    
    void ReceivedNotebookDataFromOther(object o)
    {
        List<NetworkPackage> receivedData = (List<NetworkPackage>)o;
        NotebookDataPackage notebookDataPackage = new NotebookDataPackage(receivedData[0], activeCharacters);
        NotebookData notebookData = notebookDataPackage.ConvertToNotebookData();
        response(notebookData);
    }
    
    void ReceivedInitFromHost(object o)
    {
        List<NetworkPackage> receivedData = (List<NetworkPackage>)o;
        storyID(receivedData[0].GetData<int>());
        seed(receivedData[1].GetData<int>());
    }
    
    /// <summary>
    /// Called when no acknowlegement was received, meaning no data was received.
    /// </summary>
    void AcknowledgementTimeoutError(object o)
    {
        Debug.LogError("No acknowledgement was received");
    }
}

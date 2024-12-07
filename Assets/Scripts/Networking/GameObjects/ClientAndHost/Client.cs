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
    
    private DataSender           sender;
    private Action<NotebookData> response;
    private Action<int> storyID;
    private Action<int> seed;
    
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
        sender.AddOnReceiveResponseEvent("Seed", ReceivedSeedFromHost);
        sender.AddOnReceiveResponseEvent("StoryID", ReceivedStoryIdFromHost);
        
        StartCoroutine(sender.DisplayAnyDebugs(1f));
        StartCoroutine(sender.Connect(5f));
        StartCoroutine(sender.ListenForResponse(3f));
    }

    private void OnConnectionWithHost(object obj)
    {
        Debug.Log($"(Sender): Connected with the host.");
        sender.SendDataAsync("Seed", NetworkPackage.CreatePackage("Plz give seed!"), 10f);
        sender.SendDataAsync("StoryID", NetworkPackage.CreatePackage("Plz give storyID!"), 10f);
    }

    /// <summary>
    /// Called when no connection could be established.
    /// </summary>
    void ConnectionTimeoutError(object o)
    {
        Debug.LogError("No connection was made");
    }
    
    public void SendNotebookData(Action<NotebookData> response)
    {
        this.response = response;
        NotebookDataPackage package = new NotebookDataPackage(GameManager.gm.notebookData);
        sender.AddOnAckTimeoutEvent("NotebookData", AcknowledgementTimeoutError);
        sender.SendDataAsync("NotebookData", package.CreatePackage(), 5f);
        sender.AddOnReceiveResponseEvent("NotebookResponse", ReceivedNotebookDataFromOther);
    }
    
    void ReceivedNotebookDataFromOther(object o)
    {
        List<NetworkPackage> receivedData = (List<NetworkPackage>)o;
        NotebookDataPackage notebookDataPackage = new NotebookDataPackage(receivedData[0]);
        NotebookData notebookData = notebookDataPackage.ConvertToNotebookData();
        response(notebookData);
    }
    
    void ReceivedStoryIdFromHost(object o)
    {
        List<NetworkPackage> receivedData = (List<NetworkPackage>)o;
        storyID(receivedData[0].GetData<int>());
    }
    
    void ReceivedSeedFromHost(object o)
    {
        List<NetworkPackage> receivedData = (List<NetworkPackage>)o;
        seed(receivedData[0].GetData<int>());
    }
    
    /// <summary>
    /// Called when no acknowlegement was received, meaning no data was received.
    /// </summary>
    void AcknowledgementTimeoutError(object o)
    {
        Debug.LogError("No acknowledgement was received");
    }
}

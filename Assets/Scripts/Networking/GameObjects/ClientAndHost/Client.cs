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
public class Client : NetworkObject
{
    private DataSender           sender;
    private Action<NotebookData> response;
    private Action<int>          storyID;
    private Action<int>          seed;
    
    //basically a copy from Gamemanager.gm.currentCharacters.
    //This is a separate variable to limit coupling as much as possible
    private List<CharacterInstance> activeCharacters;
    
    /// <summary>
    /// Enters a classroom code. This converts it back to an ip, connects with this ip and requests initialisation data.
    /// The seed and storyID actions are used to assign these values into the rest of the game.
    /// </summary>
    public void EnterClassroomCode(string classroomCode, Action<int> seed, Action<int> storyID)
    {
        this.seed = seed;
        this.storyID = storyID;
        IPAddress hostAddress;
        IPv4Converter converter = new IPv4Converter();
        try
        {
            hostAddress = converter.ConvertToIPAddress(classroomCode);
        }
        catch (ArgumentException)
        {
            if (settings.IsDebug)
                Debug.Log("(Client): Invalid classroom code");
            else
                DisplayError("Invalid classroom code.");
            return;
        }
        
        sender = new DataSender(hostAddress, settings.ClientHostPortConnection);
        sender.AddOnDisconnectedEvent(Disconnected);
        
        sender.AddOnConnectionTimeoutEvent(ConnectionTimeoutError);
        sender.AddOnConnectEvent(OnConnectionWithHost);
        sender.AddOnReceiveResponseEvent(settings.InitialisationDataSignature, ReceivedInitFromHost);
        
        //additional debugs if in debug mode
        if (settings.IsDebug)
            AddAdditionalDebugMessagesClassroomCode();
        
        StartCoroutine(sender.DisplayAnyDebugs(settings.DisplayDebugIntervalSeconds));
        StartCoroutine(sender.IsDisconnected(settings.DisconnectedIntervalSeconds));
        StartCoroutine(sender.Connect(settings.ConnectionTimeoutSeconds));
    }
    
    private void Disconnected(object o)
    {
        if (settings.IsDebug)
            Debug.Log("(Client): Disconnected from the host.");
        else
            DisplayError("You got disconnected from the host, please check whether you and the host are connected to the internet.");
    }
    
    private void ConnectionTimeoutError(object o)
    {
        if (settings.IsDebug)
            Debug.Log("(Client): No connection was made.");
        else
            DisplayError("No connection with the host could be established, please check if the entered classroom code is correct" +
                         " and whether you and the host are connected to the internet.");
    }
    
    private void OnConnectionWithHost(object obj)
    {
        if (settings.IsDebug)
            Debug.Log($"(Client): Connected with the host.");
        
        StartCoroutine(sender.ListenForResponse(settings.ListeningWhileNotConnectedIntervalSeconds));
        sender.SendDataAsync(settings.InitialisationDataSignature,
            NetworkPackage.CreatePackage("Plz give init data!"), settings.AcknowledgementTimeoutSeconds);
    }
    
    private void ReceivedInitFromHost(object o)
    {
        List<NetworkPackage> receivedData = (List<NetworkPackage>)o;
        storyID(receivedData[0].GetData<int>());
        seed(receivedData[1].GetData<int>());
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
        
        if (settings.IsDebug)
            AddAdditionalDebugMessagesNotebook();
        
        sender.AddOnAckTimeoutEvent(settings.NotebookDataSignature, AcknowledgementTimeoutError);
        sender.AddOnReceiveResponseEvent(settings.NotebookDataSignature, ReceivedNotebookDataFromOther);
        sender.SendDataAsync(settings.NotebookDataSignature, package.CreatePackage(), settings.AcknowledgementTimeoutSeconds);
    }
    
    private void ReceivedNotebookDataFromOther(object o)
    {
        List<NetworkPackage> receivedData = (List<NetworkPackage>)o;
        NotebookDataPackage notebookDataPackage = new NotebookDataPackage(receivedData[0], activeCharacters);
        NotebookData notebookData = notebookDataPackage.ConvertToNotebookData();
        response(notebookData);
    }
    
    /// <summary>
    /// Called when no acknowlegement was received, meaning no data was received.
    /// </summary>
    private void AcknowledgementTimeoutError(object o)
    {
        DisplayError("Failed to sent a message to the host, please check whether you and the host are connected to the internet.");
    }
    
    private void DisplayError(string error)
    {
        if (doPopup is null)
            Debug.LogError("No popup for error handling was initialised");
        else
            doPopup.Raise(this, error);
    }
    
    #region debugMethods
    
    private void AddAdditionalDebugMessagesClassroomCode()
    {
        sender.AddOnDataSentEvent(settings.InitialisationDataSignature, DataSentInit);
        sender.AddOnAckReceivedEvent(settings.InitialisationDataSignature, AckReceived);
        sender.AddOnAckTimeoutEvent(settings.InitialisationDataSignature, AckTimeoutInit);
        sender.AddOnNotConnectedListeningEvents(ListeningWhileDisconnected);
    }
    
    private void DataSentInit(object o)
    {
        Debug.Log($"(Client): Sent {o} bytes to the host as an init request.");
    }
    
    private void AckReceived(object o)
    {
        Debug.Log($"(Client): Received ACK with signature \"{o}\""); 
    }
    
    private void AckTimeoutInit(object o)
    {
        Debug.Log($"(Client): Ack with signature {o} timed out");
    }
    
    private void ListeningWhileDisconnected(object obj)
    {
        Debug.Log("(Client): Disconnected while listening for response from host.");
    }
    
    private void AddAdditionalDebugMessagesNotebook()
    {
        sender.AddOnDataSentEvent(settings.NotebookDataSignature, DataSentNotebook);
        sender.AddOnAckReceivedEvent(settings.NotebookDataSignature, AckReceived);
    }
    
    private void DataSentNotebook(object o)
    {
        Debug.Log($"(Client): Sent {o} bytes to the host as a notebook data request.");
    }
    #endregion
    
    
    public override void Dispose()
    {
        sender.Dispose();
    }
}

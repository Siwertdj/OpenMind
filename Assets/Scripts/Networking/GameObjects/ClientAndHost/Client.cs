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
    private enum MultiplayerState
    {
        Infant,
        Connected,
        Initialized,
        UploadedNotebook,
        ReceivedNotebook
    }
    
    private DataSender           sender;
    private Action<NotebookData> response;
    private Action<int>          storyID;
    private Action<int>          seed;
    private Action               reactivateJoinButton;
    private MultiplayerState     multiplayerState = MultiplayerState.Infant;

    private bool   tryReconnect;
    private bool   isConnected;
    private Action resendNotebook;
    
    //basically a copy from Gamemanager.gm.currentCharacters.
    //This is a separate variable to limit coupling as much as possible
    private List<CharacterInstance> activeCharacters;

    private void Update()
    {
        if (tryReconnect)
        {
            tryReconnect = false;
            StartCoroutine(sender.Connect(settings.ConnectionTimeoutSeconds));
        }
    }

    /// <summary>
    /// Enters a classroom code. This converts it back to an ip, connects with this ip and requests initialisation data.
    /// The seed and storyID actions are used to assign these values into the rest of the game.
    /// </summary>
    public void EnterClassroomCode(string classroomCode, Action<int> seed, Action<int> storyID, Action reactivateJoinButton)
    {
        this.seed = seed;
        this.storyID = storyID;
        this.reactivateJoinButton = reactivateJoinButton;
        IPAddress hostAddress;
        IPv4Converter converter = new IPv4Converter();
        try
        {
            hostAddress = converter.ConvertToIPAddress(classroomCode);
        }
        catch (ArgumentException)
        {
            if (settings.IsDebug)
                DebugError("(Client): Invalid classroom code");
            else
                DisplayError("Invalid classroom code.");
            return;
        }
        
        sender = new DataSender(hostAddress, settings.ClientHostPortConnection, settings.PingDataSignature);
        sender.AddOnDisconnectedEvent(Disconnected);
        
        sender.AddOnConnectionTimeoutEvent(ConnectionTimeoutError);
        sender.AddOnConnectEvent(OnConnectionWithHost);
        sender.AddOnReceiveResponseEvent(settings.InitialisationDataSignature, ReceivedInitFromHost);

        
        //additional debugs if in debug mode
        if (settings.IsDebug)
            AddAdditionalDebugMessagesClassroomCode();
        
        StartCoroutine(sender.DisplayAnyDebugs(settings.DisplayDebugIntervalSeconds));
        StartCoroutine(sender.IsDisconnected(settings.PingDataSignature, settings.DisconnectedIntervalSeconds));
        StartCoroutine(sender.Connect(settings.ConnectionTimeoutSeconds));
        StartCoroutine(sender.ListenForResponse(settings.ListeningWhileNotConnectedIntervalSeconds));
    }
    
    private void Disconnected(object o)
    {
        isConnected = false;
        if (multiplayerState == MultiplayerState.Initialized || multiplayerState == MultiplayerState.UploadedNotebook)
           tryReconnect = true;
        
        if (settings.IsDebug)
            Debug.Log("(Client): Disconnected from the host.");
        else
            DisplayError("You got disconnected from the host, please check whether you and the host are connected to the internet.");
            
    }
    
    private void ConnectionTimeoutError(object o)
    {
        if (settings.IsDebug)
            DebugError("(Client): No connection was made.");
        else
            DisplayError("No connection with the host could be established, please check if the entered classroom code is correct" +
                         " and whether you and the host are connected to the internet.");
    }
    
    private void OnConnectionWithHost(object obj)
    {
        isConnected = true;
        if (settings.IsDebug)
            Debug.Log($"(Client): Connected with the host.");

        if (multiplayerState == MultiplayerState.UploadedNotebook)
            resendNotebook();

        if (multiplayerState == MultiplayerState.Infant)
        {
            multiplayerState = MultiplayerState.Connected;
            sender.SendDataAsync(settings.InitialisationDataSignature,
                NetworkPackage.CreatePackage("Plz give init data!"),
                settings.AcknowledgementTimeoutSeconds);
        }
    }
    
    private void ReceivedInitFromHost(object o)
    {
        multiplayerState = MultiplayerState.Initialized;
        List<NetworkPackage> receivedData = (List<NetworkPackage>)o;
        
        int story = receivedData[0].GetData<int>();
        int seeed = receivedData[1].GetData<int>();
        
        if (settings.IsDebug)
            Debug.Log($"Received message from host, seed = {seeed} and story = {story}.");
        
        storyID(story);
        seed(seeed);
    }
    
    /// <summary>
    /// Sends notebook data to the host and handles the received notebook data.
    /// </summary>
    /// <param name="response">A function to assign the notebook data obtained by the host.</param>
    /// <param name="notebookData">The notebook data to send to the host.</param>
    /// <param name="currentCharacters">The list of current characters, this is required to correctly obtain the notes from each character.</param>
    public void SendNotebookData(Action<NotebookData> response, NotebookData notebookData, List<CharacterInstance> currentCharacters)
    {
        if (!isConnected)
        {
            resendNotebook = () => SendNotebookData(response, notebookData, currentCharacters);
            return;
        }
            
        
        this.response = response;
        activeCharacters = currentCharacters;
        NotebookDataPackage package = new NotebookDataPackage(notebookData, currentCharacters);
        
        if (settings.IsDebug)
            AddAdditionalDebugMessagesNotebook();
        
        sender.AddOnDataSentEvent(settings.NotebookDataSignature, ConfirmNotebookSent);
        sender.AddOnAckTimeoutEvent(settings.NotebookDataSignature, AcknowledgementTimeoutError);
        sender.AddOnReceiveResponseEvent(settings.NotebookDataSignature, ReceivedNotebookDataFromOther);
        sender.SendDataAsync(settings.NotebookDataSignature, package.CreatePackage(), settings.AcknowledgementTimeoutSeconds);
    }

    private void ConfirmNotebookSent(object o)
    { 
        multiplayerState = MultiplayerState.UploadedNotebook;
    }
    
    private void ReceivedNotebookDataFromOther(object o)
    { 
        multiplayerState = MultiplayerState.ReceivedNotebook;
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
        {
            doPopup.Raise(this, error, new Color(0,0,0));
            reactivateJoinButton();
        }
    }

    private void DebugError(string error)
    {
        Debug.Log(error);
        reactivateJoinButton();
    }
    
    #region debugMethods
    
    private void AddAdditionalDebugMessagesClassroomCode()
    {
        sender.AddOnDataSentEvent(settings.InitialisationDataSignature, DataSentInit);
        sender.AddOnNotConnectedListeningEvents(ListeningWhileDisconnected);
        sender.AddOnAckTimeoutEvent(settings.InitialisationDataSignature, AckTimeoutInit);
    }
    
    private void DataSentInit(object o)
    {
        Debug.Log($"(Client): Sent {o} bytes to the host as an init request.");
    }
    
    private void AckTimeoutInit(object o)
    {
        Debug.Log($"(Client): Ack with signature {o} timed out");
    }
    
    private void ListeningWhileDisconnected(object obj)
    {
        //Debug.Log("(Client): Disconnected while listening for response from host.");
    }
    
    private void AddAdditionalDebugMessagesNotebook()
    {
        sender.AddOnDataSentEvent(settings.NotebookDataSignature, DataSentNotebook);
    }
    
    private void DataSentNotebook(object o)
    {
        Debug.Log($"(Client): Sent {o} bytes to the host as a notebook data request.");
    }
    #endregion
    
    
    public override void Dispose()
    {
        sender?.Dispose();
    }
}

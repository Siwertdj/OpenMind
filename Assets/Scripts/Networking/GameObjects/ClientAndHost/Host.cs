// This program has been developed by students from the bachelor Computer Science at Utrecht University within the Software Project course.
// © Copyright Utrecht University (Department of Information and Computing Sciences)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using Random = System.Random;

public class Host : NetworkObject
{
    private DataListener                 listener;
    private int                          seed;
    private int                          storyID;
    private List<List<NetworkPackage>>   notebooks = new ();
    private Action<List<NetworkPackage>> sendFirstNotebook;
    private Random                       notebookRandom = new Random();//a separate random variable that is distinct from the gamemanager random
    
    
    public string CreateClassroomCode()
    {
        IPv4Converter converter = new IPv4Converter();
        return converter.ConvertToCode(ownIP);
    }

    public void Lobby(int storyID, int seed)
    {
        this.seed = seed;
        this.storyID = storyID;
        
        listener = new DataListener(ownIP, settings.ClientHostPortConnection);
        StartCoroutine(listener.DisplayAnyDebugs(settings.DisplayDebugIntervalSeconds));
        listener.AddResponseTo(settings.InitialisationDataSignature, SendInit);
        
        if (settings.IsDebug)
            AddAdditionalDebugMessagesInit();
        
        StartCoroutine(listener.AcceptIncomingConnections());
        StartCoroutine(listener.ListenForIncomingData(settings.IncomingDataIntervalSeconds));
        
        ActivateNotebookExchange();
    }
    
    private List<NetworkPackage> SendInit(List<NetworkPackage> arg)
    {
        if (settings.IsDebug)
            Debug.Log("Host sending seed & storyID.");
        
        return new List<NetworkPackage>
        {
            NetworkPackage.CreatePackage(storyID),
            NetworkPackage.CreatePackage(seed)
        };
    }

    #region Notebook
    private void ActivateNotebookExchange()
    {
        sendFirstNotebook =
            listener.AddDelayedResponseTo(settings.NotebookDataSignature,
                ReceiveFirstNotebookFromClient);
    }
    
    /// <summary>
    /// The host uploads their own notebook
    /// </summary>
    public void AddOwnNotebook(Action<NotebookData> assignNotebookData, NotebookData notebookData, List<CharacterInstance> currentCharacters)
    {
        NotebookDataPackage package = new NotebookDataPackage(notebookData, currentCharacters);
        List<NetworkPackage> listPackage = new List<NetworkPackage> { package.CreatePackage() };
        if (notebooks.Count == 0)
            ReceiveFirstNotebookFromClient(listPackage);
        else
        {
            List<NetworkPackage> notebook = ReceiveAndRespondWithNotebook(listPackage);
            NotebookDataPackage notebookDataPackage = new NotebookDataPackage(notebook[0], currentCharacters);
            assignNotebookData(notebookDataPackage.ConvertToNotebookData());
        }
    }
    
    /// <summary>
    /// The function for receiving the first notebook from the client.
    /// Because the player that sends the first notebook has to wait for a second player to send their notebook,
    /// this is a separate function
    /// </summary>
    private void ReceiveFirstNotebookFromClient(object o)
    {
        //if this wasn't the first notebook, meaning the first notebook came from the host
        //then immediately send a response
        if (notebooks.Count > 0)
            sendFirstNotebook(GetRandomNotebook());
        
        notebooks.Add((List<NetworkPackage>)o);
        listener.AddResponseTo(settings.NotebookDataSignature, ReceiveAndRespondWithNotebook);
    }

    private List<NetworkPackage> ReceiveAndRespondWithNotebook(List<NetworkPackage> o)
    {
        List<NetworkPackage> randomNotebook = GetRandomNotebook();
        notebooks.Add(o);
        return randomNotebook;
    }
    
    private List<NetworkPackage> GetRandomNotebook() =>
        notebooks[notebookRandom.Next(notebooks.Count)];
    #endregion
    
    #region debugMethods
    
    private void AddAdditionalDebugMessagesInit()
    {
        StartCoroutine(listener.IsDisconnected(settings.DisconnectedIntervalSeconds));
        listener.AddOnAcceptConnectionsEvent(OnConnectionAccepted);
        listener.AddOnDisconnectedEvent(OnDisconnect);
        listener.AddOnDataReceivedEvent(settings.InitialisationDataSignature, OnDataReceived);
        listener.AddOnResponseSentEvent(settings.InitialisationDataSignature, OnResponseSent);
        listener.AddOnAckSentEvent(OnAckSent);
    }
    
    private void OnConnectionAccepted(object obj)
    {
        Debug.Log($"(Host): Connected with client {((Socket)obj).RemoteEndPoint}");
    }
    
    private void OnDisconnect(object obj)
    {
        Debug.Log($"(Host): Client with endpoint {((Socket)obj).RemoteEndPoint} disconnected");
    }
    
    private void OnDataReceived(object obj)
    {
        Debug.Log($"(Host): Received data {((List<NetworkPackage>)obj)[0].GetData<string>()}");
    }
    
    private void OnResponseSent(object obj)
    {
        Debug.Log($"(Host): Sent response package of {obj} bytes");
    }
    
    private void OnAckSent(object obj)
    {
        Debug.Log($"(Host): Sent ack with signature {obj}");
    }
    #endregion
    
    public override void Dispose()
    {
        listener.Dispose();
    }
}

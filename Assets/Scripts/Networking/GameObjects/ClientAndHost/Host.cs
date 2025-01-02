// This program has been developed by students from the bachelor Computer Science at Utrecht University within the Software Project course.
// © Copyright Utrecht University (Department of Information and Computing Sciences)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;
using UnityEngine;
using Random = System.Random;

public class Host : NetworkObject
{
    private DataListener                 listener;
    private int                          seed;
    private int                          storyID;
    private List<List<NetworkPackage>>   notebooks = new ();
    private Action<List<NetworkPackage>> sendFirstNotebook;
    private Action<List<NetworkPackage>> assignNotebookData;
    private Random                       notebookRandom = new Random();//a separate random variable that is distinct from the gamemanager random
    private bool addNormalResponse = false;
    private bool readyToSentFirstClientSecondClientNotebook;
    private List<NetworkPackage> dataToSendSecondClient;
    private int playerCount;
    private bool isListening;
    
    private void Update()
    {
        if (addNormalResponse)
        {
            //wait for async reading to finish
            addNormalResponse = false;
            StartCoroutine(SendDataWithDelay());
        }
        
        if (readyToSentFirstClientSecondClientNotebook)
        {
            Debug.Log("coroutine");
            readyToSentFirstClientSecondClientNotebook = false;
            StartCoroutine(SendDataWithDelay1());
        }
    }
    
    private IEnumerator SendDataWithDelay()
    {
        yield return new WaitForSeconds(1);
        Debug.Log("Adding normal response");
        listener.AddResponseTo(settings.NotebookDataSignature, ReceiveAndRespondWithNotebook);
    }
    
    private IEnumerator SendDataWithDelay1()
    {
        yield return new WaitForSeconds(1);
        sendFirstNotebook(dataToSendSecondClient);
    }
    
    public string CreateClassroomCode()
    {
        IPv4Converter converter = new IPv4Converter();
        return converter.ConvertToCode(ownIP);
    }

    public void Lobby(int storyID, int seed)
    {
        playerCount = 0;
        this.seed = seed;
        this.storyID = storyID;
        
        listener = new DataListener(ownIP, settings.ClientHostPortConnection);
        StartCoroutine(listener.DisplayAnyDebugs(settings.DisplayDebugIntervalSeconds));
        listener.AddResponseTo(settings.InitialisationDataSignature, SendInit);
        
        if (settings.IsDebug)
            AddAdditionalDebugMessagesInit();
        
        StartCoroutine(listener.AcceptIncomingConnections());
        StartCoroutine(listener.ListenForIncomingData(settings.IncomingDataIntervalSeconds));
        
        listener.AddOnDisconnectedEvent(OnPlayerDisconnect);
        listener.AddOnAcceptConnectionsEvent(OnPlayerConnect);
        
        ActivateNotebookExchange();

        isListening = true;
    }
    
    private void OnPlayerDisconnect(object obj)
    {
        Debug.Log("Disconnected");
        playerCount--;
    }
    
    private void OnPlayerConnect(object obj)
    {
        Debug.Log("Connected");
        playerCount++;
    }

    public int PlayerAmount(int maxPlayers)
    {
        if (isListening && playerCount >= maxPlayers)
        {
            listener.CancelListeningForConnections();
            isListening = false;
        }

        if (!isListening && playerCount < maxPlayers)
        {
            StartCoroutine(listener.AcceptIncomingConnections());
            isListening = true;
        }
        
        return playerCount;
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
        this.assignNotebookData = package => assignNotebookData(
            new NotebookDataPackage(package[0], currentCharacters).ConvertToNotebookData());
        NotebookDataPackage package = new NotebookDataPackage(notebookData, currentCharacters);
        List<NetworkPackage> listPackage = new List<NetworkPackage> { package.CreatePackage() };
        
        //if host was first upload
        if (notebooks.Count == 0)
        {
            ReceiveFirstNotebookFromClient(listPackage);
            return;
        }
        
        //if host was second, send data to the first
        if (notebooks.Count == 1)
            sendFirstNotebook(listPackage);
        
        //get new notebook data and assign it to the host
        List<NetworkPackage> notebook = ReceiveAndRespondWithNotebook(listPackage);
        // assignNotebookData(
        //     new NotebookDataPackage(notebook[0], currentCharacters).ConvertToNotebookData());
        this.assignNotebookData(notebook);
    }
    
    /// <summary>
    /// The function for receiving the first notebook from the client.
    /// Because the player that sends the first notebook has to wait for a second player to send their notebook,
    /// this is a separate function
    /// </summary>
    private void ReceiveFirstNotebookFromClient(object o)
    {
        //client as second notebook upload
        if (notebooks.Count == 1)
        {
            //if host was first upload
            if (assignNotebookData != null)
                assignNotebookData((List<NetworkPackage>)o);
        }
        
        //if not first upload
        if (notebooks.Count > 0)
            return;
        
        Debug.Log($"Received first notebook {((List<NetworkPackage>)o)[0].data}");
        notebooks.Add((List<NetworkPackage>)o);
        addNormalResponse = true;
    }

    private List<NetworkPackage> ReceiveAndRespondWithNotebook(List<NetworkPackage> o)
    {
        //if client and second upload, assign notebook to the first upload if it was also a client
        if (notebooks.Count == 1 && assignNotebookData == null)
        {
            // dataToSendSecondClient = o;
            // readyToSentFirstClientSecondClientNotebook = true;
            Debug.Log($"sending first notebook, sending {o[0].data}, {addNormalResponse}");
            sendFirstNotebook(o);
        }
            
        
        List<NetworkPackage> randomNotebook = GetRandomNotebook();
        notebooks.Add(o);
        Debug.Log($"Obtained {o[0].data} and returned with {randomNotebook[0].data}");
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
        listener.AddOnResponseSentEvent(settings.NotebookDataSignature, OnResponseSent);
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

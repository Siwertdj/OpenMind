// This program has been developed by students from the bachelor Computer Science at Utrecht University within the Software Project course.
// © Copyright Utrecht University (Department of Information and Computing Sciences)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using Ping = UnityEngine.Ping;


public class NetworkManager : MonoBehaviour
{
    private DataSender   sender;
    private List<string> debugMessages = new List<string>();
    
    // Update is called once per frame
    void Start()
    {
        GetLocalIPs();
        SetupNetworkTest();
    }
    
    void Update()
    {
        if (debugMessages.Count > 0)
        {
            foreach (string debugMessage in debugMessages)
                Debug.Log(debugMessage);
            
            debugMessages.Clear();
        }
    }
    
    void SetupNetworkTest()
    {
        Debug.Log("Starting setup");
        
        IPAddress address = IPConnections.GetOwnIps()[0];
        DataListener dataListener = new DataListener(address, IPConnections.Port);
        StartCoroutine(dataListener.DisplayAnyDebugs(0.5f));
        dataListener.AddOnDataReceivedEvent("test", ReceiveData);
        dataListener.AddOnResponseSentEvent("test", SentResponse);
        dataListener.AddResponseTo("test", Respond);
        
        sender = new DataSender(address, IPConnections.Port);
        sender.AddOnConnectEvent(Connected);
        sender.AddOnResponseEvent("test", ReceieveResponse);
        sender.AddOnDataSentEvent("test", SendConfirmation);
        
        StartCoroutine(sender.Connect(5f));
        StartCoroutine(sender.ListenForResponse("test"));
        StartCoroutine(dataListener.AcceptIncomingConnections(2f));
        StartCoroutine(dataListener.ListenForIncomingData("test", 2f));
        Debug.Log("Ended setup");
    }
    
    void SentResponse(object o)
    {
        debugMessages.Add($"Sent response package of {(int)o} bytes.");
    }
    
    void Connected(object o)
    {
        debugMessages.Add("Connected.");
        NetworkPackage package = NetworkPackage.CreatePackage("hello");
        sender.SendDataAsync("test", package);
    }
    
    void ReceiveData(object o)
    {
        debugMessages.Add($"Received message: {((List<NetworkPackage>)o)[0].GetData<string>()}.");
    }
    
    void ReceieveResponse(object o)
    {
        debugMessages.Add($"Received response: {((List<NetworkPackage>)o)[0].GetData<string>()}.");
    }
    
    void SendConfirmation(object a)
    {
        debugMessages.Add($"send {a} bytes.");
    }
    
    string Respond(List<NetworkPackage> originalMessage)
    {
        return $"Hello, I got your message \"{originalMessage[0].GetData<string>()}\"!";
    }
    
    void GetLocalIPs()
    {
        foreach (IPAddress se in IPConnections.GetOwnIps())
        {
            Debug.Log(se);
        }
    }
}

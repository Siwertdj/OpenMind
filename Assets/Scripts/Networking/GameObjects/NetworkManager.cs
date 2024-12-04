// This program has been developed by students from the bachelor Computer Science at Utrecht University within the Software Project course.
// © Copyright Utrecht University (Department of Information and Computing Sciences)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using System.Net.Sockets;
using System.Threading.Tasks;


public class NetworkManager : MonoBehaviour
{
    private DataSender   sender;
    private List<string> debugMessages = new List<string>();
    
    // Update is called once per frame
    void Start()
    {
        GetLocalIPs();
        SetupBroadcastTest();
        //SetupNetworkTest();
    }
    
    void Update()
    {
        if (debugMessages.Count > 0)
        {
            for (int i = 0; i < debugMessages.Count; i++)
                Debug.Log(debugMessages[i]);
            
            debugMessages.Clear();
        }
    }
    
    void FindNearbyIpsTest()
    {
        DeviceFinder deviceFinder = new DeviceFinder(7777);
        StartCoroutine(deviceFinder.PingAllRoutine(400));
    }
    
    void SetupBroadcastTest()
    {
        SetupBeacon();
        //SetupProbe();
    }
    
    void SetupProbe()
    {
        Debug.Log("setup probe");
        Probe probe = gameObject.GetComponent<Probe>();
        probe.enabled = true;
        StartCoroutine(WaitSeconds(30f, () =>
        {
            string foundIps = "";
            foreach (var probeFoundDevice in probe.FoundDevices)
            {
                foundIps += $"{probeFoundDevice}, ";
            }
            Debug.Log($"probe found IPs: {foundIps}");
            probe.enabled = false;
        }));
    }
    
    void SetupBeacon()
    {
        Debug.Log("setup beacon");
        Beacon beacon = gameObject.GetComponent<Beacon>();
        beacon.enabled = true;
        StartCoroutine(WaitSeconds(30f, () =>
        {
            string foundIps = "";
            foreach (var beaconFoundDevice in beacon.FoundDevices)
            {
                foundIps += $"{beaconFoundDevice}, ";
            }
            Debug.Log($"beacon found IPs: {foundIps}");
            
            beacon.enabled = false;
        }));
    }
    
    IEnumerator WaitSeconds(float secs, Action action)
    {
        yield return new WaitForSeconds(secs);
        action();
    }
    
    void SetupNetworkTest()
    {
        Debug.Log("Starting setup");
        SetupListener();
        //SetupSender();
        Debug.Log("Ended setup");
    }
    
    void SetupListener()
    {
        Debug.Log("Setting up listener.");
        IPAddress address = IPConnections.GetOwnIps()[0];
        DataListener dataListener = new DataListener(address, IPConnections.Port);
        StartCoroutine(dataListener.DisplayAnyDebugs(0f));
        dataListener.AddOnDataReceivedEvent("test", ListenerDataReceived);
        dataListener.AddOnResponseSentEvent("test", ListenerSentResponse);
        dataListener.AddOnAcceptConnectionsEvent(ListenerConnect);
        dataListener.AddOnAckSentEvent(ListenerSentACK);
        dataListener.AddResponseTo("test", EchoMessage);
        dataListener.AddOnDisconnectedEvent(ListenerDisconnect);
        
        StartCoroutine(dataListener.AcceptIncomingConnections(3f));
        StartCoroutine(dataListener.ListenForIncomingData(0.1f));
        StartCoroutine(dataListener.IsDisconnected(1f));
    }
    
    void SetupSender()
    {
        Debug.Log("Setting up sender");
        IPAddress address = IPAddress.Parse("145.136.157.36");
        sender = new DataSender(address, IPConnections.Port);
        StartCoroutine(sender.DisplayAnyDebugs(0f));
        sender.AddOnConnectEvent(SenderConnect);
        sender.AddOnDataSentEvent("test", SenderDataSent);
        sender.AddOnReceiveResponseEvent("test", SenderReceiveResponse);
        sender.AddOnAckReceivedEvent(SenderReceiveACK);
        sender.AddOnAckTimeoutEvent("test", SenderAckTimeout);
        sender.AddOnDisconnectedEvent(SenderDisconnect);
        
        StartCoroutine(sender.Connect(10f));
        StartCoroutine(sender.ListenForResponse(3f));
        StartCoroutine(sender.IsDisconnected(1f));
    }
    
    void SenderDisconnect(object o)
    {
        debugMessages.Add($"(Sender): The socket {o} got disconnected.");
    }
    
    void ListenerDisconnect(object o)
    {
        debugMessages.Add($"(Listener): The socket {o} got disconnected.");
    }
    
    void SenderConnect(object o)
    {
        debugMessages.Add($"(Sender): Connected with the host with task status {((Task)o).Status}");
        sender.SendDataAsync("test", NetworkPackage.CreatePackage("Justin is smart"), 10f);
    }
    
    void SenderDataSent(object o)
    {
        debugMessages.Add($"(Sender): Sent {o} bytes to the host.");
    }
    
    void SenderReceiveResponse(object o)
    {
        debugMessages.Add($"(Sender): Received response: {((List<NetworkPackage>)o)[0].GetData<string>()}");
    }
    
    void SenderReceiveACK(object o)
    {
        debugMessages.Add($"(Sender): Received ACK with signature \"{o}\"");
    }
    
    void SenderAckTimeout(object o)
    {
        debugMessages.Add($"(Sender): Ack with signature {o} timed out");
    }
    
    void ListenerConnect(object o)
    {
        debugMessages.Add($"(Listener): Connected with socket {((Socket)o).RemoteEndPoint}");
    }
    
    void ListenerDataReceived(object o)
    {
        debugMessages.Add($"(Listener): Received data: {((List<NetworkPackage>)o)[0].GetData<string>()}");
    }
    
    void ListenerSentResponse(object o)
    {
        debugMessages.Add($"(Listener): Sent response package of {o} bytes");
    }
    
    void ListenerSentACK(object o)
    {
        debugMessages.Add($"(Listener): Sent ACK of signature \"{(((int, string))o).Item2}\"");
    }
    
    List<NetworkPackage> EchoMessage(List<NetworkPackage> data)
    {
        return data;
    }
    
    void GetLocalIPs()
    {
        foreach (IPAddress se in IPConnections.GetOwnIps())
        {
            Debug.Log(se);
        }
    }
}

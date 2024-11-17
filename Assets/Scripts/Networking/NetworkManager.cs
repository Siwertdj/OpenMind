using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using Ping = UnityEngine.Ping;


public class NetworkManager : MonoBehaviour
{
    private DataSender sender;
    
    // Update is called once per frame
    void Start()
    {
        GetLocalIPs();
    }
    
    void SetupNetworkTest()
    {
        NetworkPackage package = NetworkPackage.CreatePackage("hello");
        
        DataListener dataListener = new DataListener(IPAddress.Parse("192.168.0.0"), 7777);
        dataListener.AddOnDataReceivedEvent("test", ReceiveData);
        dataListener.Respond("test", Respond);
        StartCoroutine(dataListener.AcceptIncomingConnections(2f));
        StartCoroutine(dataListener.ListenForIncomingData("test", 2f));
        
        sender = new DataSender(IPAddress.Parse("192.168.0.0"), 7777);
        sender.AddOnResponseEvent("test", ReceieveResponse);
        sender.AddOnDataSentEvent("test", SendConfirmation);
        sender.SendDataAsync("test", package);
        StartCoroutine(sender.Connect(5f));
        StartCoroutine(sender.ListenForResponse("test"));
    }
    
    void ReceiveData(object o)
    {
        Debug.Log($"Received message: {((List<NetworkPackage>)o)[0].GetData<string>()}.");
    }
    
    void ReceieveResponse(object o)
    {
        Debug.Log($"Received response: {((List<NetworkPackage>)o)[0].GetData<string>()}.");
    }
    
    void SendConfirmation(object a)
    {
        Debug.Log($"send {a} bytes.");
    }
    
    string Respond(List<NetworkPackage> originalMessage)
    {
        return $"Hello, I got your message \"{originalMessage[0].GetData<string>()}\"!";
    }
    
    void GetLocalIPs()
    {
        foreach (string se in GetAllLocalIPv4())
        {
            Debug.Log(se);
        }
    }
    
    public string[] GetAllLocalIPv4()
    {
        List<string> ipAddrList = new List<string>();
        foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (item.OperationalStatus == OperationalStatus.Up)
            {
                foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                {
                    if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        ipAddrList.Add(ip.Address.ToString());
                    }
                }
            }
        }
        return ipAddrList.ToArray();
    }
}

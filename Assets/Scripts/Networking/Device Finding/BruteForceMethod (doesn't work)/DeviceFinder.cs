// This program has been developed by students from the bachelor Computer Science at Utrecht University within the Software Project course.
// © Copyright Utrecht University (Department of Information and Computing Sciences)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// This class deals with finding the ip addresses of all devices on the local network.
/// </summary>
class DeviceFinder
{
    private ushort                  port;
    private Action<List<IPAddress>> onPingCompletedCallback;
    
    public DeviceFinder(ushort port)
    {
        this.port = port;
    }
    
    public IEnumerator PingAllRoutine(int ipsPerFrame, float timeoutSeconds = 5f)
    {
        List<IPAddress> foundIPs = new();
        List<Socket> sockets = new();
        
        IPAddress localIP = IPConnections.GetOwnIps()[0];
        localIP = IPAddress.Parse("192.168.0.0");
        
        Debug.Log("Starting main loop");
        DateTime start = DateTime.Now;
        int loopCount = 0;
        int total = 0;
        foreach (IPAddress potentialIP in IPConnections.LoopThroughLocalRange(localIP))
        {
            Socket socket = new Socket(potentialIP.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            socket.ConnectAsync(potentialIP, port);
            sockets.Add(socket);
            
            loopCount++;
            total++;
            if (loopCount > ipsPerFrame)
            {
                loopCount = 0;
                yield return null;
            }
        }
        Debug.Log("Completed loop.");
        
        yield return new WaitUntil(() =>
            (DateTime.Now - start).TotalMilliseconds > timeoutSeconds * 1000);
        
        if (onPingCompletedCallback is not null)
            onPingCompletedCallback(foundIPs);
        
        Debug.Log("Finished");
    }
    
    public void OnPingCompletedCallback(Action<List<IPAddress>> action)
    {
        onPingCompletedCallback = action;
    }
}
// This program has been developed by students from the bachelor Computer Science at Utrecht University within the Software Project course.
// © Copyright Utrecht University (Department of Information and Computing Sciences)


using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class DeviceProbe : UdpDeviceReceiver
{
    private HashSet<IPEndPoint> connectedBeacons = new ();
    public  HashSet<IPEndPoint> ConnectedBeacons => connectedBeacons;
    private bool                isSending;
    private UdpClient           broadcaster;
    
    private readonly IPEndPoint broadcast = new(IPAddress.Parse("192.168.2.255"), IPConnections.Port);
    private const char type = 'P';
    
    public DeviceProbe(string identifier) : base(identifier)
    {
        broadcaster = new UdpClient();
        broadcaster.EnableBroadcast = true;
        broadcaster.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);
        broadcaster.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        // udp.Client.Bind(new IPEndPoint(IPAddress.Any, IPConnections.Port));
    }
    
    // protected override void ReceiveMessage(IPEndPoint receivedEndpoint, char type)
    // {
    //     if (type == DeviceProbe.type)
    //         return;
    //     
    //     connectedBeacons.Add(receivedEndpoint);
    // }
    
    public IEnumerator StartSendingProbes(float intervalSeconds)
    {
        isSending = true;
        
        while (isSending)
        {
            SendAsync(broadcast, DeviceProbe.type);
            yield return new WaitForSeconds(intervalSeconds);
        }
    }
    
    protected override void Dispose()
    {
        isSending = false;
    }
    
    protected override void Clear()
    {
        connectedBeacons.Clear();
    }
}

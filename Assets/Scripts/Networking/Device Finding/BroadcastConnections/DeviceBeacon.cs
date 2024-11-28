// This program has been developed by students from the bachelor Computer Science at Utrecht University within the Software Project course.
// © Copyright Utrecht University (Department of Information and Computing Sciences)


using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class DeviceBeacon : UdpDeviceReceiver
{
    private HashSet<IPEndPoint> connectedProbes = new ();
    public  HashSet<IPEndPoint> ConnectedProbes => connectedProbes;
    
    private const char type = 'B';
    
    public DeviceBeacon(string identifier) : base(identifier)
    {
    }
    
    protected override void ReceiveMessage(IPEndPoint receivedEndpoint, char type)
    {
        if (type == DeviceBeacon.type)
            return;
        
        connectedProbes.Add(receivedEndpoint);
        
        //return a message
        SendAsync(receivedEndpoint, DeviceBeacon.type);
    }
    
    protected override void Dispose()
    {
    }
    
    protected override void Clear()
    {
        connectedProbes.Clear();
    }
}

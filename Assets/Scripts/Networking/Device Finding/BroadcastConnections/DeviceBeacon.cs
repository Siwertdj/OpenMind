// This program has been developed by students from the bachelor Computer Science at Utrecht University within the Software Project course.
// © Copyright Utrecht University (Department of Information and Computing Sciences)


using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class DeviceBeacon : UdpDeviceReceiver
{
    private HashSet<IPEndPoint> connectedProbes = new ();
    public  HashSet<IPEndPoint> ConnectedProbes => connectedProbes;
    private bool                isReceiving = false;
    private UdpClient           receiver;
    
    private const char type = 'B';
    
    public DeviceBeacon(string identifier) : base(identifier)
    {
        receiver = new UdpClient(IPConnections.Port);
        receiver.Client.Bind(new IPEndPoint(IPAddress.Any, IPConnections.Port));
    }
    
    protected void ReceiveMessage(IPEndPoint receivedEndpoint, char type)
    {
        if (type == DeviceBeacon.type)
            return;
        
        connectedProbes.Add(receivedEndpoint);
        
        // //return a message
        // SendAsync(receivedEndpoint, DeviceBeacon.type);
    }
    
    public IEnumerator StartReceivingResponses()
    {
        bool isReceiving = true;
        
        while (isReceiving)
        {
            Task task = receiver.ReceiveAsync().ContinueWith(t =>
            {
                byte[] buffer = t.Result.Buffer;
                string receivedIdentifier = Encoding.UTF8.GetString(buffer);
                char type = receivedIdentifier[^1];
                if (receivedIdentifier != identifier + type)
                    ReceiveMessage(t.Result.RemoteEndPoint, type);
            });
            
            // byte[] buffer = new byte[1024];
            // EndPoint senderEndPoint = new IPEndPoint(IPAddress.Any, 0);
            // receiver.Client.ReceiveFromAsync(buffer, SocketFlags.Broadcast, senderEndPoint).ContinueWith(t =>
            // {
            //     // Decode the received data
            //     //string message = Encoding.UTF8.GetString(buffer, 0, t.Result);
            //     Debug.Log($"Received message from {senderEndPoint}");
            // });
            
            // byte[] buffer = new byte[1024];
            // EndPoint senderEndPoint = new IPEndPoint(IPAddress.Any, 0);
            // receiver.Client.ReceiveFrom(buffer, SocketFlags.Broadcast, ref senderEndPoint);
            // Debug.Log($"Received message from {senderEndPoint}");
            
            //wait until receiving is finished
            yield return new WaitUntil(() => task.IsCompleted || !isReceiving);
            yield return null;
        }
        
        // yield return null;
    }
    
    protected override void Dispose()
    {
        isReceiving = false;
    }
    
    protected override void Clear()
    {
        connectedProbes.Clear();
    }
}

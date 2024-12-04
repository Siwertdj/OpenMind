// This program has been developed by students from the bachelor Computer Science at Utrecht University within the Software Project course.
// © Copyright Utrecht University (Department of Information and Computing Sciences)


using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class UdpDeviceReceiver
{
    private bool      isReceiving;
    protected string    identifier;
    private UdpClient udp = new();
    
    protected UdpDeviceReceiver(string identifier)
    {
        this.identifier = identifier;
    }
    
    // public IEnumerator StartReceivingResponses()
    // {
    //     isReceiving = true;
    //     
    //     while (isReceiving)
    //     {
    //         Task task = udp.ReceiveAsync().ContinueWith(t =>
    //         {
    //             byte[] buffer = t.Result.Buffer;
    //             string receivedIdentifier = Encoding.UTF8.GetString(buffer);
    //             char type = receivedIdentifier[^1];
    //             if (receivedIdentifier != identifier + type)
    //                 ReceiveMessage(t.Result.RemoteEndPoint, type);
    //         });
    //         
    //         //wait until receiving is finished
    //         yield return new WaitUntil(() => task.IsCompleted || !isReceiving);
    //         // yield return null;
    //     }
    // }
    
    
    protected void SendAsync(IPEndPoint target, char type)
    {
        byte[] probeData = Encoding.UTF8.GetBytes(identifier + type);
        udp.SendAsync(probeData, probeData.Length, target);
    }
    
    // protected abstract void ReceiveMessage(IPEndPoint receivedEndpoint, char type);
    protected abstract void Dispose();
    protected abstract void Clear();
    
    public void Close()
    {
        Dispose();
        isReceiving = false;
    }
    
    public void Remove()
    {
        Close();
        Clear();
        udp.Close();
    }
}

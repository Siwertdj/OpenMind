// This program has been developed by students from the bachelor Computer Science at Utrecht University within the Software Project course.
// © Copyright Utrecht University (Department of Information and Computing Sciences)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

/// <summary>
/// Facilitates sending data over a network.
/// </summary>
public class DataSender
{
    private Socket        sender;
    private IPEndPoint    endPoint;
    private NetworkEvents onDataSentEvents;
    private NetworkEvents onResponseEvent;
        
    public DataSender(IPAddress ipAddress, ushort port)
    {
        sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        endPoint = new IPEndPoint(ipAddress, port);
        onDataSentEvents = new NetworkEvents();
        onResponseEvent = new NetworkEvents();
        sender.NoDelay = true;
    }
    
    public IEnumerator Connect(float timeoutSeconds)
    {
        Task connecting = sender.ConnectAsync(endPoint)
            .ContinueWith(t => onDataSentEvents.Raise("Connect", t));
        yield return new WaitForSeconds(timeoutSeconds);
        if (!connecting.IsCompleted)
            Debug.LogError("Failed to connect");
    }
    
    /// <summary>
    /// Sends the data in an async way to the target IPAddress.
    /// Raises the actions that are subscribed to the data sent events.
    /// </summary>
    /// <param name="port">The port to send this data on.</param>
    /// <param name="signature">
    /// The signature to give this package. A response, if any is sent, will have the same signature.
    /// This signature is used to identify the response and use the right function on it.
    /// </param>
    /// <param name="payload">The data to send through the network.</param>
    /// <param name="clearEvents">If set to true, all events connected to the given signature will be cleared after they are called.</param>
    public void SendDataAsync(string signature, IList<NetworkPackage> payload, bool clearEvents = true)
    {
        //cannot send data when not connected
        if (!sender.Connected)
            throw new Exception("Cannot send data when the socket is not connected.");
        
        NetworkPackage sign = NetworkPackage.CreatePackage(signature);
        // List<ArraySegment<byte>> networkData = new List<ArraySegment<byte>>
        //     { sign.ConvertToBytes() };
        //
        // networkData.AddRange(payload.Select(np => np.ConvertToBytes()));
        
        List<NetworkPackage> networkData = new List<NetworkPackage> { sign };
        networkData.AddRange(payload);
        List<string> stringPayload = networkData.Select(np => np.ConvertToString()).ToList();
        string rawData = JsonConvert.SerializeObject(stringPayload);
        byte[] bytes = Encoding.UTF8.GetBytes(rawData);
        if (bytes.Length > NetworkPackage.MaxPackageSize)
            Debug.LogError("Package was too large.");

        sender.SendAsync(bytes, SocketFlags.None).ContinueWith(t =>
            onDataSentEvents.Raise(signature, t.Result, clearEvents));
    }
    
    /// <summary>
    /// Alias for <see cref="SendDataAsync(string,System.Collections.Generic.IList{NetworkPackage},bool)"/>,
    /// but with a single network package as the payload.
    /// </summary>
    public void SendDataAsync(string signature, NetworkPackage payload, bool clearEvents = true) =>
        SendDataAsync(signature, new List<NetworkPackage> { payload }, clearEvents);
    
    /// <summary>
    /// Alias for <see cref="SendDataAsync(string,System.Collections.Generic.IList{NetworkPackage},bool)"/>,
    /// but with no payload.
    /// </summary>
    public void SendDataAsync(string signature, bool clearEvents = true) =>
        SendDataAsync(signature, new List<NetworkPackage>(), clearEvents);
    
    /// <summary>
    /// Adds an action to the event of completing the send action.
    /// When the sending of a package with the given signature completes, the given action is called.
    /// The object parameter of the action is the amount of bytes that were sent.
    /// </summary>
    public void AddOnDataSentEvent(string signature, Action<object> action) =>
        onDataSentEvents.Subscribe(signature, action);
    
    public void AddOnResponseEvent(string signature, Action<object> action) =>
        onResponseEvent.Subscribe(signature, action);
    
    public void AddOnConnectEvent(Action<object> action) =>
        onDataSentEvents.Subscribe("Connect", action);
    
    public IEnumerator ListenForResponse(string signature, bool clearResponses = true)
    {
        bool response = false;
        while (!response)
        {
            byte[] buffer = new byte[NetworkPackage.MaxPackageSize];
            Task task = sender.ReceiveAsync(buffer, SocketFlags.None).ContinueWith(
                receivedByteAmount =>
                {
                    string rawData = Encoding.UTF8.GetString(buffer);
                    rawData = rawData.TrimEnd('\0');
                    List<NetworkPackage> networkData =
                        JsonConvert.DeserializeObject<List<string>>(rawData)
                            .Select(NetworkPackage.ConvertToPackage).ToList();
                    NetworkPackage receivedFirstPackage = networkData[0];
                    
                    if (signature != receivedFirstPackage.GetData<string>())
                        return;
                    
                    List<NetworkPackage> receivedTailPackages = networkData.Skip(1).ToList();
                    
                    onResponseEvent.Raise(signature, receivedTailPackages, clearResponses);
                    response = true;
                });
            
            yield return new WaitUntil(() => task.IsCompleted);
        }
    }
}

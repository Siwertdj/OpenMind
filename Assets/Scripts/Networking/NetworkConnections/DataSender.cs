// This program has been developed by students from the bachelor Computer Science at Utrecht University within the Software Project course.
// © Copyright Utrecht University (Department of Information and Computing Sciences)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
    
    private string logError, logWarning = "";
    
    /// <summary>
    /// Creates a data sender object using an IPAddress and a port to create an endpoint.
    /// The ipAddress should point to the device you want to send the message to.
    /// </summary>
    public DataSender([DisallowNull] IPAddress ipAddress, ushort port)
    {
        try
        {
            sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }
        catch (SocketException e)
        {
            Debug.LogError("Created an invalid socket, got error: " + e);
            sender = null;
            return;
        }
        
        endPoint = new IPEndPoint(ipAddress, port);
        onDataSentEvents = new NetworkEvents();
        onResponseEvent = new NetworkEvents();
    }
    
    /// <summary>
    /// Starts an attempt to connect with the given endpoint.
    /// </summary>
    /// <param name="timeoutSeconds">
    /// A debug error is thrown when the sender is not connected with the given endpoint after the given timeout amount.
    /// </param>
    /// /// <param name="clearDataSentEvents">If set to true, the actions called after connecting with the host are removed from the event.</param>
    public IEnumerator Connect(float timeoutSeconds, bool clearDataSentEvents = true)
    {
        //check if you are already connected
        if (!sender.Connected)
        {
            Task connecting = sender.ConnectAsync(endPoint)
                .ContinueWith(t =>
                {
                    try
                    {
                        onDataSentEvents.Raise("Connect", t, clearDataSentEvents);
                    }
                    catch (Exception e)
                    {
                        logError =
                            $"onDataSentEvent with signature Connect returned exception: {e}";
                    }
                });
            yield return new WaitForSeconds(timeoutSeconds);
            if (!connecting.IsCompleted)
                logError = "Failed to connect";
        }
    }
    
    /// <summary>
    /// Sends the data in an async way to the target.
    /// Raises the actions that are subscribed to the data sent events.
    /// </summary>
    /// <param name="signature">
    /// The signature to give this package. A response, if any is sent, will have the same signature.
    /// This signature is used to identify the response and use the right function on it.
    /// </param>
    /// <param name="payload">The data to send through the network.</param>
    /// <param name="clearDataSentEvents">If set to true, all events connected to the given signature will be cleared after they are called.</param>
    public void SendDataAsync(string signature, IList<NetworkPackage> payload, bool clearDataSentEvents = true)
    {
        //cannot send data when not connected
        if (!sender.Connected)
        {
            Debug.LogError("Cannot send data when the socket is not connected.");
            return;
        }
        
        NetworkPackage sign = NetworkPackage.CreatePackage(signature);
        
        List<NetworkPackage> networkData = new List<NetworkPackage> { sign };
        networkData.AddRange(payload);
        List<string> stringPayload = networkData.Select(np => np.ConvertToString()).ToList();
        string rawData = JsonConvert.SerializeObject(stringPayload);
        byte[] bytes = Encoding.UTF8.GetBytes(rawData);
        if (bytes.Length > NetworkPackage.MaxPackageSize)
        {
            Debug.LogError("Package was too large.");
            return;
        }
        sender.SendAsync(bytes, SocketFlags.None).ContinueWith(t =>
        {
            try
            {
                onDataSentEvents.Raise(signature, t.Result, clearDataSentEvents);
            }
            catch (Exception e)
            {
                logError =
                    $"onDataSentEvent with signature {signature} returned exception: {e}";
            }
        });
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
    
    /// <summary>
    /// Adds an action to the event of receiving a response.
    /// When the receiving a package with the given signature, the given action is called.
    /// The object parameter of the action is the data received.
    /// </summary>
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

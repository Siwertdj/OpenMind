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
/// A class to facilitate listening for data.
/// </summary>
public class DataListener : DataNetworker
{
    private List<Socket> connections;
    private List<bool>   isConnectionReceiving;
    
    ///<summary>when a connection is made, input is the socket the connect is made with</summary>
    private NetworkEvents onAcceptConnectionEvents;
    ///<summary>when any data is received, input is the data received</summary>
    private NetworkEvents onDataReceivedEvents;
    ///<summary>when a response is sent back to the sender of the received data, input is the amount of bytes of the return message</summary>
    private NetworkEvents onResponseSentEvents;
    ///<summary>when an ack is sent to the sender to confirm their message has been received, input is a tuple of an int and the received message</summary>
    private NetworkEvents onAckSentEvents;
    ///<summary>used to convert received data into a new message</summary>
    private NetworkEvents respondEvents;
    
    private bool isConnectionListening, isDataListening;
   
    /// <summary>
    /// Creates a data listening object using an IPAddress and a port to create an endpoint.
    /// The ipAddress should point to an ipAddress on the local device.
    /// </summary>
    public DataListener([DisallowNull] IPAddress ipAddress, ushort port) : base(ipAddress, port)
    {
        
        onAcceptConnectionEvents = new NetworkEvents();
        onDataReceivedEvents = new NetworkEvents();
        onResponseSentEvents = new NetworkEvents();
        onAckSentEvents = new NetworkEvents();
        respondEvents = new NetworkEvents();
        
        if (!IPConnections.GetOwnIps().Contains(ipAddress))
        {
            Debug.LogError("You can only bind to your own IPAddress.");
            return;
        }
        
        socket.Bind(endPoint);
        Debug.Log("Local: " + socket.LocalEndPoint);
        socket.Listen(255);
        connections = new List<Socket>();
        isConnectionReceiving = new List<bool>();
        Debug.Log(socket.LocalEndPoint);
        
        //create the ack respond event with the signature as the message
        onAckSentEvents.Subscribe("ACK",
            o =>
            {
                (int, string) data = ((int, string))o;
                AddResponseTo("ACK", signature => signature, 
                    (data.Item1, new List<NetworkPackage>{NetworkPackage.CreatePackage(data.Item2)}));
            });
    }
    
    /// <summary>
    /// Starts a loop in a coroutine to accept incoming connections. This function will run infinitely unless cancelled.
    /// While this function is running, any incoming connections will be accepted.
    /// </summary>
    /// <param name="intervalSeconds">
    /// Retry accepting new connections each interval.
    /// The interval exists so it doesn't run constantly and take up a lot of resources.
    /// </param>
    /// <param name="clearOnAcceptConnectionEvents">If set to true, the actions called after connecting with a client are removed from the event.</param>
    public IEnumerator AcceptIncomingConnections(float intervalSeconds, bool clearOnAcceptConnectionEvents = false)
    {
        GiveDisplayWarning();
        isConnectionListening = true;
        
        while (isConnectionListening)
        {
            Task task = null;
            try
            {
                task = socket.AcceptAsync().ContinueWith(t =>
                {
                    connections.Add(t.Result);
                    isConnectionReceiving.Add(false);
                    logWarning = onAcceptConnectionEvents.Raise("Connect", t.Result, clearOnAcceptConnectionEvents, "onAcceptConnectionEvent");
                });
            }
            catch (ObjectDisposedException e)
            {
                Debug.LogError("The socket was closed: " + e);
                isConnectionListening = false;
            }
            catch (Exception e)
            {
                Debug.LogError("Something went wrong when listening: " + e);
                isConnectionListening = false;
            }
            
            yield return new WaitUntil(() => task is null || task.IsCompleted);
        }
    }
    
    /// <summary>
    /// Makes the listener stop listening for incoming connections.
    /// </summary>
    public void CancelListeningForConnections() => isConnectionListening = false;
    
    
    /// <summary>
    /// Listens for incoming data in a coroutine.
    /// When data is received, the signature is read and the actions connected to the signature are called.
    /// Actions will be added by adding events using the relevant functions.
    ///
    /// This loop will run infinitely unless cancelled by calling the relevant cancel function.
    /// <para>Note: A response with the signature "ACK" will always be sent after receiving a message. These ACK events will never be cleared.</para>
    /// </summary>
    /// <param name="intervalSeconds">After each interval a check is made whether any incoming data was received.</param>
    /// <param name="clearDataReceivedEvents">If set to true, the actions called after receiving a package are removed from the event.</param>
    /// <param name="clearRespondEvents">If set to true, the actions called after responding with a message are removed from the event.</param>
    /// <param name="clearAckSentEvents">If set to true, the actions called after responding with an ack (acknowledement) are removed from the event.</param>
    public IEnumerator ListenForIncomingData(float intervalSeconds, bool clearDataReceivedEvents = false, bool clearRespondEvents = false, bool clearAckSentEvents = false)
    {
        GiveDisplayWarning();
        isDataListening = true;
        
        while (isDataListening)
        {
            for (var i = 0; i < connections.Count; i++)
            {
                //if this socket is already receiving data, ignore it.
                if (isConnectionReceiving[i])
                    continue;
                
                ReceiveData(i, clearDataReceivedEvents, clearRespondEvents);
            }
            
            if (isDataListening)
                yield return new WaitForSeconds(intervalSeconds);
        }
    }
    
    /// <summary>
    /// Makes the listener stop listening for incoming data.
    /// </summary>
    public void CancelListeningForData() => isDataListening = false;
    
    /// <summary>
    /// A function for handling incoming data, see <see cref="ListenForIncomingData"/> for details.
    /// </summary>
    private void ReceiveData(int index, bool clearDataReceivedEvents, bool clearRespondEvents)
    {
        isConnectionReceiving[index] = true;
        byte[] buffer = new byte[NetworkPackage.MaxPackageSize];
        connections[index].ReceiveAsync(buffer, SocketFlags.None).ContinueWith(
            receivedByteAmount =>
            {
                //catch all just in case an error slips through
                try
                {
                    if (!TryGetConvertData(buffer, receivedByteAmount, out List<List<NetworkPackage>> networkData))
                        return;
                    
                    foreach (var networkPackage in networkData)
                        HandleReceivedData(networkPackage, index, clearDataReceivedEvents, clearRespondEvents);
                }
                catch (Exception e)
                {
                    logError = "Receiving message failed with error: " + e;
                }
            });
    }
    
    private void HandleReceivedData(List<NetworkPackage> networkData, int index, bool clearDataReceivedEvents, bool clearRespondEvents)
    {
        string signature = networkData[0].GetData<string>();
        
        List<NetworkPackage> receivedTailPackages = networkData.Skip(1).ToList();
        
        logError = onDataReceivedEvents.Raise(signature, receivedTailPackages, clearDataReceivedEvents, "onDataReceivedEvent");
        if (logError != "")
            return;
        
        //respond with an ack
        logError = onAckSentEvents.Raise("ACK", (index, signature), false, "onAckSentEvent");
        if (logError != "")
            return;
        
        //responds to the sender
        logError = respondEvents.Raise(signature, (index, receivedTailPackages),
            clearRespondEvents, "respondEvent");
        
        isConnectionReceiving[index] = false;
    }
    
    /// <summary>
    /// Adds an action to the event of a sender connecting with this listener.
    /// When receiving a connection from a socket (DataSender), the given action is called.
    /// The object parameter of the action is the socket that the connection was made with.
    /// </summary>
    public void AddOnAcceptConnectionsEvent(Action<object> action) =>
        onAcceptConnectionEvents.Subscribe("Connect", action);
    
    /// <summary>
    /// Adds an action to the event of receiving a data package.
    /// When receiving a package with the given signature, the given action is called.
    /// The object parameter of the action is the data that was received.
    /// </summary>
    public void AddOnDataReceivedEvent([DisallowNull] string signature, Action<object> action) =>
        onDataReceivedEvents.Subscribe(signature, action);
    
    /// <summary>
    /// Adds an action to the event of sending a response.
    /// When receiving a package and then responding by sending a new package to the sender, the given action is called.
    /// The object parameter of the action is the amount of byte that a response was sent with.
    /// </summary>
    public void AddOnResponseSentEvent([DisallowNull] string signature, Action<object> action) =>
        onResponseSentEvents.Subscribe(signature, action);
    
    /// <summary>
    /// Adds an action to the event of sending an ACK (acknowledgement message).
    /// When receiving a package and then responding by sending an ACK to the sender, the given action is called.
    /// The object parameter of the action is the signature of the received message.
    /// </summary>
    public void AddOnAckSentEvent(Action<object> action) =>
        onAckSentEvents.Subscribe("ACK", action);
    
    /// <summary>
    /// Adds a response to receiving a message with the given signature.
    /// Response events with the signature "ACK" will always be called regardless of the signature of the received message.
    /// </summary>
    /// <param name="signature">The messages with this signature will be responded to.</param>
    /// <param name="response">Given the received data, create a response with it.</param>
    public void AddResponseTo([DisallowNull] string signature, Func<List<NetworkPackage>, List<NetworkPackage>> response) =>
        respondEvents.Subscribe(signature,
            o => AddResponseTo(signature, response, ((int, List<NetworkPackage>))o));
    
    /// <summary>
    /// Same as <see cref="AddResponseTo(string,System.Func{System.Collections.Generic.List{NetworkPackage},System.Collections.Generic.List{NetworkPackage}})"/>
    /// but with a single package as the response.
    /// </summary>
    public void AddResponseTo([DisallowNull] string signature,
        Func<List<NetworkPackage>, NetworkPackage> response) =>
        respondEvents.Subscribe(signature,
            o => AddResponseTo(signature, d => new List<NetworkPackage> { response(d) },
                ((int, List<NetworkPackage>))o));
    
    /// <summary>
    /// Sends a response after receiving a message.
    /// </summary>
    private void AddResponseTo(
        [DisallowNull] string signature, 
        Func<List<NetworkPackage>,List<NetworkPackage>> response, 
        (int, List<NetworkPackage>) socketIndexAndMessage, 
        bool clearResponseSentEvents = false)
    {
        //ACK responses contain the signature as the message
        List<NetworkPackage> resp = response(socketIndexAndMessage.Item2);
        
        if (!TryCreatePackage(signature, resp, out byte[] bytes))
            return;
        
        connections[socketIndexAndMessage.Item1].SendAsync(bytes, SocketFlags.None).ContinueWith(
            t => logError = onResponseSentEvents.Raise(signature, t.Result, clearResponseSentEvents, "onResponseSentEvent"));
    }

    protected override bool IsDisconnected(out Socket info)
    {
        for (var i = 0; i < connections.Count; i++)
        {
            if (!connections[i].Connected)
            {
                info = connections[i];
                connections.Remove(connections[i]);
                
                return true;
            }
        }

        info = null;
        return false;
    }
}

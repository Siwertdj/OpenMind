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
using Newtonsoft.Json;
using UnityEngine;

/// <summary>
/// A class to facilitate listening for data.
/// </summary>
public class DataListener
{
    private Socket        listener;
    private IPEndPoint    endPoint;
    private NetworkEvents onDataReceivedEvents;
    private NetworkEvents respondEvents;
    private NetworkEvents onReponseSentEvents;
    private List<Socket>  connections;
    private List<bool>    isConnectionReceiving;
    
    private bool   isConnectionListening, isDataListening;
    private string logError   = "";
    private string logWarning = "";
   
    /// <summary>
    /// Creates a data listening object using an IPAddress and a port to create an endpoint.
    /// The ipAddress should point to an ipAddress on the local device.
    /// </summary>
    public DataListener([DisallowNull] IPAddress ipAddress, ushort port)
    {
        try
        {
            listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }
        catch (SocketException e)
        {
            Debug.LogError("Created an invalid socket, got error: " + e);
            listener = null;
            return;
        }
        
        endPoint = new IPEndPoint(ipAddress, port);
        
        onDataReceivedEvents = new NetworkEvents();
        respondEvents = new NetworkEvents();
        onReponseSentEvents = new NetworkEvents();
        
        if (!IPConnections.GetOwnIps().Contains(ipAddress))
        {
            Debug.LogError("You can only bind to your own IPAddress.");
            return;
        }
        
        listener.Bind(endPoint);
        listener.Listen(255);
        connections = new List<Socket>();
        isConnectionReceiving = new List<bool>();
    }
    
    /// <summary>
    /// Displays any debug messages that were called during any function.
    /// When some async functions throw exception, they aren't caught by unit and go unnoticed.
    /// This function checks every interval whether any Debug messages were called.
    /// </summary>
    public IEnumerator DisplayAnyDebugs(float intervalSeconds)
    {
        while (true)
        {
            if (logError != "")
            {
                Debug.LogError(logError);
                logError = "";
            }
            
            if (logWarning != "")
            {
                Debug.LogWarning(logWarning);
                logWarning = "";
            }
            
            yield return new WaitForSeconds(intervalSeconds);
        }
    }
    
    /// <summary>
    /// Starts a loop in a coroutine to accept incoming connections. This function will run infinitely unless cancelled.
    /// While this function is running, any incoming connections will be accepted.
    /// </summary>
    /// <param name="intervalSeconds">
    /// Retry accepting new connections each interval.
    /// The interval exists so it doesn't run constantly and take up a lot of resources.
    /// </param>
    public IEnumerator AcceptIncomingConnections(float intervalSeconds)
    {
        isConnectionListening = true;
        
        while (isConnectionListening)
        {
            try
            {
                listener.AcceptAsync().ContinueWith(t =>
                {
                    connections.Add(t.Result);
                    isConnectionReceiving.Add(false);
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
            
            if (isConnectionListening)
                yield return new WaitForSeconds(intervalSeconds);
        }
    }
    
    /// <summary>
    /// Makes the listener stop listening for incoming connections.
    /// </summary>
    public void CancelListeningForConnections() => isConnectionListening = false;
    
    
    /// <summary>
    /// Listens for incoming data  with the given signature in a coroutine.
    /// When data is received, the signature is read and the actions connected to the signature are called.
    /// Actions will be added by adding events using the relevant functions.
    ///
    /// This loop will run infinitely unless cancelled by calling the relevant cancel function.
    /// </summary>
    /// <param name="signature">The signature to listen for, all other incoming messages will be ignored.</param>
    /// <param name="intervalSeconds">After each interval a check is made whether any incoming data was received.</param>
    /// <param name="clearDataReceivedEvents">If set to true, the actions called after receiving a package are removed from the event.</param>
    /// <param name="clearRespondEvents">If set to true, the actions called after responding with a message are removed from the event.</param>
    public IEnumerator ListenForIncomingData([DisallowNull] string signature, float intervalSeconds, bool clearDataReceivedEvents = true, bool clearRespondEvents = true)
    {
        isDataListening = true;
        
        while (isDataListening)
        {
            for (var i = 0; i < connections.Count; i++)
            {
                if (isConnectionReceiving[i])
                    continue;
                
                ReceiveData(signature, i, clearDataReceivedEvents, clearRespondEvents);
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
    private void ReceiveData([DisallowNull] string signature, int index, bool clearDataReceivedEvents, bool clearRespondEvents)
    {
        isConnectionReceiving[index] = true;
        byte[] buffer = new byte[NetworkPackage.MaxPackageSize];
        connections[index].ReceiveAsync(buffer, SocketFlags.None).ContinueWith(
            receivedByteAmount =>
            {
                //catch all just in case an error slips through
                try
                {
                    if (receivedByteAmount.Result > NetworkPackage.MaxPackageSize)
                    {
                        logWarning = $"Received package was too large, expected a package of " +
                                   $"{NetworkPackage.MaxPackageSize} bytes or less, but got " +
                                   $"{receivedByteAmount.Result} bytes. The incoming data was rejected.";
                        return;
                    }
                    
                    string rawData = Encoding.UTF8.GetString(buffer);
                    rawData = rawData.TrimEnd('\0');
                    
                    List<NetworkPackage> networkData;
                    try
                    {
                        networkData = JsonConvert
                            .DeserializeObject<List<string>>(rawData)
                            .Select(NetworkPackage.ConvertToPackage)
                            .ToList();
                    }
                    catch (JsonException e)
                    {
                        logWarning = "Reading received message with json failed: " + e;
                        return;
                    }
                    
                    NetworkPackage receivedFirstPackage = networkData[0];
                    
                    if (signature != receivedFirstPackage.GetData<string>())
                        return;
                    
                    List<NetworkPackage> receivedTailPackages = networkData.Skip(1).ToList();
                    
                    try
                    {
                        onDataReceivedEvents.Raise(signature, receivedTailPackages, clearDataReceivedEvents);
                    }
                    catch (Exception e)
                    {
                        logError =
                            $"onDataReceivedEvent with signature {signature} returned exception: {e}";
                        return;
                    }
                    
                    try
                    {
                        respondEvents.Raise(signature, (index, receivedTailPackages),
                            clearRespondEvents);
                    }
                    catch (Exception e)
                    {
                        logError =
                            $"onRespondEvent with signature {signature} returned exception: {e}";
                        return;
                    }
                    
                    isConnectionReceiving[index] = false;
                }
                catch (Exception e)
                {
                    logError = "Receiving message failed with error: " + e;
                }
            });
    }
    
    /// <summary>
    /// Adds an action to the event of receiving a data package
    /// When receiving a package with the given signature, the given action is called.
    /// The object parameter of the action is the data that was received.
    /// </summary>
    public void AddOnDataReceivedEvent([DisallowNull] string signature, Action<object> action) =>
        onDataReceivedEvents.Subscribe(signature, action);
    
    public void AddResponseTo<T>([DisallowNull] string signature, Func<List<NetworkPackage>, T> response) =>
        respondEvents.Subscribe(signature,
            o => AddResponseTo(signature, response, ((int, List<NetworkPackage>))o));
    
    public void AddOnResponseSentEvent([DisallowNull] string signature, Action<object> action) =>
        onReponseSentEvents.Subscribe(signature, action);
    
    private void AddResponseTo<T>([DisallowNull] string signature, Func<List<NetworkPackage>,T> response, (int, List<NetworkPackage>) socketIndexAndMessage, bool clearResponseSentEvents = true)
    {
        NetworkPackage sign = NetworkPackage.CreatePackage(signature);
        NetworkPackage resp = NetworkPackage.CreatePackage(response(socketIndexAndMessage.Item2));
        
        List<string> networkData = new List<NetworkPackage> { sign, resp }
            .Select(np => np.ConvertToString()).ToList();
        string rawData = JsonConvert.SerializeObject(networkData);
        byte[] bytes = Encoding.UTF8.GetBytes(rawData);
        if (bytes.Length > NetworkPackage.MaxPackageSize)
        {
            logError = "Response was too large.";
            return;
        }
        
        connections[socketIndexAndMessage.Item1].SendAsync(bytes, SocketFlags.None).ContinueWith(
            t =>
            {
                try
                {
                    onReponseSentEvents.Raise(signature, t.Result, clearResponseSentEvents);
                }
                catch (Exception e)
                {
                    logError =
                        $"onResponseSentEvent with signature {signature} returned exception: {e}";
                }
            });
    }
}

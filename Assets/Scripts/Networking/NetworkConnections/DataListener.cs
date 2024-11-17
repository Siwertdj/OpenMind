// This program has been developed by students from the bachelor Computer Science at Utrecht University within the Software Project course.
// © Copyright Utrecht University (Department of Information and Computing Sciences)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Unity.Plastic.Newtonsoft.Json;
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
    private List<Socket>  connections;
    private List<bool>    isConnectionReceiving;
   
    public DataListener(IPAddress ipAddress, ushort port)
    {
        listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        endPoint = new IPEndPoint(ipAddress, port);
        onDataReceivedEvents = new NetworkEvents();
        respondEvents = new NetworkEvents();
        listener.Bind(endPoint);
        listener.Listen(255);
        connections = new List<Socket>();
        isConnectionReceiving = new List<bool>();
    }
    
    public IEnumerator AcceptIncomingConnections(float intervalSeconds)
    {
        while (true)
        {
            listener.AcceptAsync().ContinueWith(t =>
            {
                connections.Add(t.Result);
                isConnectionReceiving.Add(false);
            });
            yield return new WaitForSeconds(intervalSeconds);
        }
    }
    
    
    /// <summary>
    /// Listens for incoming data  with the given signature in a coroutine. When data is received,
    /// the signature is read and the actions connected to the signature are called.
    /// </summary>
    /// <param name="signature">The signature to listen for, all other incoming messages will be ignored.</param>
    /// <param name="clearEvents">If set to true, the actions called after receiving a package are removed from the event.</param>
    public IEnumerator ListenForIncomingData(string signature, float intervalSeconds, bool clearEvents = true, bool clearResponses = true)
    {
        while (true)
        {
            for (var i = 0; i < connections.Count; i++)
            {
                if (isConnectionReceiving[i])
                    continue;
                
                ReceiveData(signature, i, clearEvents, clearResponses);
            }
            
            yield return new WaitForSeconds(intervalSeconds);
        }
    }
    
    private void ReceiveData(string signature, int index, bool clearEvents, bool clearResponses)
    {
        isConnectionReceiving[index] = true;
        byte[] buffer = new byte[NetworkPackage.MaxPackageSize];
        connections[index].ReceiveAsync(buffer, SocketFlags.None).ContinueWith(
            receivedByteAmount =>
            {
                string rawData = Encoding.UTF8.GetString(buffer);
                rawData = rawData.TrimEnd('\0');
                List<NetworkPackage> networkData = JsonConvert
                    .DeserializeObject<List<string>>(rawData)
                    .Select(NetworkPackage.ConvertToPackage).ToList();
                NetworkPackage receivedFirstPackage = networkData[0];

                if (signature != receivedFirstPackage.GetData<string>())
                    return;
                
                List<NetworkPackage> receivedTailPackages = networkData.Skip(1).ToList();

                onDataReceivedEvents.Raise(signature, receivedTailPackages, clearEvents);
                respondEvents.Raise(signature, (index, receivedTailPackages), clearResponses);
                isConnectionReceiving[index] = false;
            });
    }
    
    /// <summary>
    /// Adds an action to the event of receiving a data package
    /// When receiving a package with the given signature, the given action is called.
    /// The object parameter of the action is the data that was received.
    /// </summary>
    public void AddOnDataReceivedEvent(string signature, Action<object> action) =>
        onDataReceivedEvents.Subscribe(signature, action);
    
    public void Respond<T>(string signature, Func<List<NetworkPackage>, T> response) =>
        respondEvents.Subscribe(signature,
            o => Respond(signature, response, ((int, List<NetworkPackage>))o));
    
    private void Respond<T>(string signature, Func<List<NetworkPackage>,T> response, (int, List<NetworkPackage>) socketIndexAndMessage)
    {
        NetworkPackage sign = NetworkPackage.CreatePackage(signature);
        NetworkPackage resp = NetworkPackage.CreatePackage(response(socketIndexAndMessage.Item2));
        
        List<string> networkData = new List<NetworkPackage> { sign, resp }
            .Select(np => np.ConvertToString()).ToList();
        string rawData = JsonConvert.SerializeObject(networkData);
        byte[] bytes = Encoding.UTF8.GetBytes(rawData);
        if (bytes.Length > NetworkPackage.MaxPackageSize)
            Debug.LogError("Response was too large.");
        
        connections[socketIndexAndMessage.Item1].SendAsync(bytes, SocketFlags.None).ContinueWith(t => Debug.Log($"Responded with {t.Result} bytes"));
    }
}

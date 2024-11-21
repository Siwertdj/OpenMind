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
/// Acts as a parent class for DataSender and DataListener.
/// This class maintains and displays any error messages caught in events.
/// This class also contains some utility functions for shared code between DataSender and DataListener.
/// </summary>
public abstract class DataNetworker
{
    protected Socket     socket;
    protected IPEndPoint endPoint;
    protected string     logError   = "";
    protected string     logWarning = "";
    
    private bool isDisplayingDebugs;
    
    protected DataNetworker([DisallowNull] IPAddress ipAddress, ushort port)
    {
        try
        {
            socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }
        catch (SocketException e)
        {
            Debug.LogError("Created an invalid socket, got error: " + e);
            socket = null;
            return;
        }
        
        endPoint = new IPEndPoint(ipAddress, port);
    }
    
    /// <summary>
    /// Displays any debug messages that were called during any function.
    /// When some async functions throw exception, they aren't caught by unit and go unnoticed.
    /// This function checks every interval whether any Debug messages were called.
    /// </summary>
    public IEnumerator DisplayAnyDebugs(float intervalSeconds)
    {
        isDisplayingDebugs = true;
        
        while (isDisplayingDebugs)
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
    /// Whenever no listening for debugs are happening, show warnings to the console.
    /// </summary>
    protected void GiveDisplayWarning()
    {
        if (!isDisplayingDebugs)
            Debug.LogWarning(
                "No debug messages from events are displayed, if any errors are thrown in these events, this will not be displayed in the unity console.");
    }
    
    /// <summary>
    /// Attempts to deconstruct network bytes into a list of networkpackages
    /// </summary>
    /// <returns>true if the conversion succeeded, otherwise false</returns>
    protected bool TryGetConvertData(byte[] buffer, Task<int> receivedByteAmount, out List<NetworkPackage> networkData)
    {
        networkData = null;
        
        if (receivedByteAmount.Result > NetworkPackage.MaxPackageSize)
        {
            logWarning = $"Received package was too large, expected a package of " +
                         $"{NetworkPackage.MaxPackageSize} bytes or less, but got " +
                         $"{receivedByteAmount.Result} bytes. The incoming data was rejected.";
            return false;
        }
        
        string rawData = Encoding.UTF8.GetString(buffer);
        rawData = rawData.TrimEnd('\0');
        
        try
        {
            networkData = JsonConvert
                .DeserializeObject<List<string>>(rawData)
                .Select(NetworkPackage.ConvertToPackage)
                .ToList();
        }
        catch (JsonException e)
        {
            logWarning = "Reading received response with json failed: " + e;
            return false;
        }
        
        return true;
    }
    
    /// <summary>
    /// Attempts to construct network bytes from a list of networkpackages
    /// </summary>
    /// <returns>true if the conversion succeeded, otherwise false</returns>
    protected bool TryCreatePackage(string signature, IEnumerable<NetworkPackage> data, out byte[] buffer)
    {
        NetworkPackage sign = NetworkPackage.CreatePackage(signature);
        
        List<NetworkPackage> networkData = new List<NetworkPackage> { sign };
        networkData.AddRange(data);
        List<string> stringPayload = networkData.Select(np => np.ConvertToString()).ToList();
        string rawData = JsonConvert.SerializeObject(stringPayload);
        buffer = Encoding.UTF8.GetBytes(rawData);
        if (buffer.Length > NetworkPackage.MaxPackageSize)
        {
            Debug.LogError("Package was too large.");
            buffer = null;
            return false;
        }
        
        return true;
    }
}

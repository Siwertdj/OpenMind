// This program has been developed by students from the bachelor Computer Science at Utrecht University within the Software Project course.
// © Copyright Utrecht University (Department of Information and Computing Sciences)


using System;
using System.Net;
using UnityEngine;

public abstract class NetworkObject : MonoBehaviour//, IDisposable
{
    protected IPAddress       ownIP;
    protected NetworkSettings settings;
    protected GameEvent       doPopup;
    
    public void AssignSettings(GameEvent doPopup, NetworkSettings settings)
    {
        this.doPopup = doPopup;
        this.settings = settings;
        ownIP = IPConnections.GetOwnIps()[0];
        
        if (this.settings.IsDebug)
            Debug.Log($"Current ip address: {IPConnections.GetOwnIps()[0]}");
    }
    
    internal void DisplayWaitNotebook()
    {
        string message = "Searching for another notebook. Please wait.";
        if (doPopup is null)
            Debug.LogError("No popup for error handling was initialised");
        else
        {
            doPopup.Raise(this, message, new Color(0,0,0), true);
        }
    }
    
    public abstract void Dispose();
    
    private void OnApplicationQuit()
    {
        Dispose();
    }
}

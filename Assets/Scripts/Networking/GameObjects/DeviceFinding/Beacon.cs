// This program has been developed by students from the bachelor Computer Science at Utrecht University within the Software Project course.
// © Copyright Utrecht University (Department of Information and Computing Sciences)

using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class Beacon : MonoBehaviour
{
    private DeviceBeacon        beacon;
    public  HashSet<IPEndPoint> FoundDevices => beacon.ConnectedProbes;
    
    public string Identifier = "OpenMindDiscovery";
    
    void Awake()
    {
        beacon = new DeviceBeacon(Identifier);
    }
    
    void OnEnable()
    {
        StartCoroutine(beacon.StartReceivingResponses());
    }
    
    void OnDisable()
    {
        beacon.Close();
    }
    
    void OnDestroy()
    {
        beacon.Remove();
    }
}

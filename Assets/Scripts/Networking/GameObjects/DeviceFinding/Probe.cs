// This program has been developed by students from the bachelor Computer Science at Utrecht University within the Software Project course.
// © Copyright Utrecht University (Department of Information and Computing Sciences)

using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class Probe : MonoBehaviour
{
    private DeviceProbe         probe;
    public  HashSet<IPEndPoint> FoundDevices => probe.ConnectedBeacons;
    
    public string Identifier           = "OpenMindDiscovery";
    public float  ProbeIntervalSeconds = 2f;
    
    void Awake()
    {
        probe = new DeviceProbe(Identifier);
    }
    
    void OnEnable()
    {
        //StartCoroutine(probe.StartReceivingResponses());
        StartCoroutine(probe.StartSendingProbes(ProbeIntervalSeconds));
    }
    
    void OnDisable()
    {
        probe.Close();
    }
    
    void OnDestroy()
    {
        probe.Remove();
    }
}

// This program has been developed by students from the bachelor Computer Science at Utrecht University within the Software Project course.
// © Copyright Utrecht University (Department of Information and Computing Sciences)

using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class Host : MonoBehaviour
{
    private DataListener listener;
    
    public void Activate()
    {
        IPAddress ownIP = IPConnections.GetOwnIps()[0];
        listener = new DataListener(ownIP, IPConnections.Port);
        
    }
}

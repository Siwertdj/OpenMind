// This program has been developed by students from the bachelor Computer Science at Utrecht University within the Software Project course.
// © Copyright Utrecht University (Department of Information and Computing Sciences)

using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

/// <summary>
/// A class to handle stuff related to IPConnections
/// </summary>
public static class IPConnections
{
    /// <summary>
    /// The port every connection uses
    /// </summary>
    public const ushort Port = 7777;
    
    /// <summary>
    /// Gets all local ips assigned to the current device
    /// </summary>
    public static IPAddress[] GetOwnIps()
    {
        List<IPAddress> ipAddrList = new List<IPAddress>();
        foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
            if (item.OperationalStatus == OperationalStatus.Up)
                foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                    if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        ipAddrList.Add(ip.Address);
        return ipAddrList.ToArray();
    }
}

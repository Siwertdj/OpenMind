// This program has been developed by students from the bachelor Computer Science at Utrecht University within the Software Project course.
// © Copyright Utrecht University (Department of Information and Computing Sciences)

using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// This class deals with finding the ip addresses of all devices on the local network.
/// </summary>
class DeviceFinder
{
    private List<string> foundIPs = new List<string>();
   
    public async Task<List<string>> PingAll()
    {
        string gateIp = NetworkGateway();
        string[] array = gateIp.Split('.');
        
        List<DeviceQuery> queries = new List<DeviceQuery>();
        for (int i = 2; i <= 16; i++)
        {  
            string pingVar = array[0] + "." + array[1] + "." + array[2] + "." + i;
            
            queries.Add(new DeviceQuery(pingVar, 1, 1000));
            queries[i-2].HasConnection().ContinueWith(t =>
            {
                if (t.Result)
                    foundIPs.Add(pingVar);
            });
        }
        
        await CheckAllQueriesDone(queries);
        return foundIPs;
    }
    
    private Task CheckAllQueriesDone(List<DeviceQuery> queries)
    {
        while(!queries.TrueForAll(q => q.Finished))
        {}
        
        return Task.CompletedTask;
    }
    
    string NetworkGateway()
    {
        string ip = null;

        foreach (NetworkInterface f in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (f.OperationalStatus == OperationalStatus.Up)
            {
                foreach (GatewayIPAddressInformation d in f.GetIPProperties().GatewayAddresses)
                {
                    ip = d.Address.ToString();
                    foundIPs.Add(ip);
                }
            }
        }

        return ip;
    }
}
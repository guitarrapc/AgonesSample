using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace AgonesAspNetCore;

internal static class AgonesHelper
{
    public static string GetLocalIPv4() => GetAllLocalIPv4s().FirstOrDefault() ?? "127.0.0.1";
    public static string[] GetAllLocalIPv4s()
    {
        List<string> ipAddrList = new();
        foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (item.OperationalStatus == OperationalStatus.Up && item.NetworkInterfaceType != NetworkInterfaceType.Loopback)
            {
                if (item.Description.Contains("VPN", StringComparison.OrdinalIgnoreCase))
                    continue;

                foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                {
                    if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        ipAddrList.Add(ip.Address.ToString());
                    }
                }
            }
        }
        return ipAddrList.ToArray();
    }
}

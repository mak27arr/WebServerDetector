using System;
using WebServerDetector.Classes.Helper;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using WebServerDetector.Classes;
using System.Collections.Generic;

namespace WebServerDetector
{
    class Program
    {
        static void Main(string[] args)
        {
            LicenseCheak.Cheak();
            List<Scaner> scanerList = new List<Scaner>();
            var ni = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface item in ni)
            {
                if (item.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork & !IPAddress.IsLoopback(ip.Address))
                        {
                            Scaner s = new Scaner(ip.Address,ip.Address.GetSubnetMask());
                            s.Scan(ip.Address, ip.Address.GetSubnetMask());
                            s.SetRefreshTime(60);
                            s.StartScan(ip.Address, ip.Address.GetSubnetMask());
                            scanerList.Add(s);
                        }
                    }
                }
            }
            Console.ReadLine();
            Console.WriteLine("Hello World!");
        }
    }
}

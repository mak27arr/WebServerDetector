using System;
using WebServerDetector.Classes.Helper;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using WebServerDetector.Classes;

namespace WebServerDetector
{
    class Program
    {
        static void Main(string[] args)
        {

            var ni = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface item in ni)
            {
                if (item.OperationalStatus == OperationalStatus.Up) //&& item.NetworkInterfaceType == ?
                {
                    foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork & !IPAddress.IsLoopback(ip.Address))
                        {
                            Scaner s = new Scaner(ip.Address,ip.Address.GetSubnetMask());
                            s.ScanAsync(ip.Address, ip.Address.GetSubnetMask());

                            Console.WriteLine(ip.Address + "   " + ip.Address.GetPosibleAddressCount(ip.Address.GetSubnetMask()));
                        }
                    }
                }
            }
            Console.WriteLine((13/16).ToString());
            Console.ReadLine();
            Console.WriteLine("Hello World!");
        }
    }
}

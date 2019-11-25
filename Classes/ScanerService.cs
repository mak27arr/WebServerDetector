using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.ServiceProcess;
using System.Text;
using WebServerDetector.Classes.Helper;

namespace WebServerDetector.Classes
{
    internal class ScanerSevice : ServiceBase
    {
        public ScanerSevice():base()
        {
            ServiceName = "WebServerScanerService";
        }
        public ScanerSevice(string[] args) : base()
        {

        }

        protected override void OnStart(string[] args)
        {
            RunScaner(args);
        }

        protected override void OnStop()
        {
            WebServer webServer = WebServer.GetInstance();
            webServer.StopAsync();
        }

        private static void RunScaner(string[] args)
        {
            List<int> portslist = new List<int>();
            portslist.Add(80);
            portslist.Add(81);
            portslist.Add(443);
            portslist.Add(8080);
            LicenseCheak.Cheak();
            List<Scaner> scanerList = new List<Scaner>();
            bool useDNS = false;
            if (args.Length > 0)
            {
                if (args.Any(s => s == "-m"))
                    useDNS = true;
            }
            var ni = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface item in ni)
            {
                if (item.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork & !IPAddress.IsLoopback(ip.Address))
                        {
                            Scaner s = new Scaner(ip.Address, ip.Address.GetSubnetMask());
                            //s.Scan(ip.Address, ip.Address.GetSubnetMask());
                            s.SetRefreshTime(60);
                            s.UseDNS = useDNS;
                            s.StartScan(ip.Address, ip.Address.GetSubnetMask());
                            s.Notify += PrintMsg;
                            s.portslist = portslist;
                            scanerList.Add(s);
                        }
                    }
                }
            }

            WebServer webServer = WebServer.GetInstance();
            webServer.StartAsync(new string[0]);
            System.Threading.Thread.Sleep(3000);
            Console.WriteLine("Total thread count :" + Process.GetCurrentProcess().Threads.Count);
        }

        static void PrintMsg(string msg)
        {
            Console.WriteLine(msg);
        }

    }
}

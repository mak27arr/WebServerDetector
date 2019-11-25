using System;
using WebServerDetector.Classes.Helper;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using WebServerDetector.Classes;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;

namespace WebServerDetector
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
            new ScanerSevice(args)
            };
            ServiceBase.Run(ServicesToRun);
        }

    }
}

using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebServerDetector.Classes;

namespace WebServerDetector.Interfaces
{
    public interface IScaner
    {
        List<ServicesInfo> GetSrvices();
        bool Scan(IPAddress network, IPAddress subnetMask);
        Task<bool> ScanAsync(IPAddress network, IPAddress subnetMask);
        bool SetRefreshTime(int second);
        bool StartScan(IPAddress network, IPAddress subnetMask);
        bool StopScan();
    }
}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebServerDetector.Interfaces;
using WebServerDetector.Classes.Helper;

namespace WebServerDetector.Classes
{
    class Scaner : IScaner
    {
        private IPAddress network;
        private IPAddress subnetMask;
        private ConcurrentBag<ServicesInfo> services;
        private int threadCount;
        private static System.Timers.Timer scanTimer;
        public int RefreshTime { get; private set; }
        public delegate void MessageHandler(string msg);
        public event MessageHandler Notify = delegate { };
        public int ThreadCount { 
            get{if (threadCount >= 1)
                    return threadCount;
                else
                    return Environment.ProcessorCount * 8;
            }
            set {if (value >= 1)
                    threadCount = value;
                else
                    threadCount = 1;
                    }}
        public Scaner(IPAddress network)
        {
            services = new ConcurrentBag<ServicesInfo>();
            RefreshTime = 60000;
        }
        public List<ServicesInfo> GetSrvices()
        {
            return services.ToArray().ToList();
        }
        public bool Scan(IPAddress network, IPAddress subnetMask)
        {
            return ScanAsync(network, subnetMask).Result;
        }
        public async Task<bool> ScanAsync(IPAddress network, IPAddress subnetMask)
        {
            int addrescount  = network.GetPosibleAddressCount(subnetMask);
            int addressperthread = addrescount / threadCount;
            if (addressperthread == 0)
            {

            }
            else
            {

            }
            throw new NotImplementedException();
        }

        private List<Tuple<IPAddress,IPAddress>> GetListAddresesForThread(IPAddress network, IPAddress subnetMask)
        {
            List < Tuple < IPAddress, IPAddress >> rezalt = new List<Tuple<IPAddress, IPAddress>>();
            int addrescount = network.GetPosibleAddressCount(subnetMask);
            int addressperthread = addrescount / threadCount;
            if (addressperthread == 0)
            {
                rezalt.Add(new Tuple<IPAddress, IPAddress>(network.Get));
            }
            else
            {

            }
            return rezalt;
        }

        public bool SetRefreshTime(int second)
        {
            RefreshTime = second * 1000;
            Notify?.Invoke("Refresh time set: " + second);
            return true;
        }
        public bool StartScan(IPAddress network, IPAddress subnetMask)
        {
            if (network != null && subnetMask !=null)
            {
                this.network = network;
                this.subnetMask = subnetMask;
            }
            else
            {
                Notify?.Invoke("Can`t set null network or subnetMask");
                return false;
            }
            if (scanTimer != null)
            {
                Notify?.Invoke("Scanning monitor already launched");
                return false;
            }
            scanTimer = new System.Timers.Timer(RefreshTime);
            scanTimer.Elapsed += delegate{ ScanAsync(this.network, this.subnetMask); };
            scanTimer.AutoReset = true;
            scanTimer.Enabled = true;
            Notify?.Invoke("Scanning monitor launched");
            return true;
        }
        public bool StopScan()
        {
            if (scanTimer != null)
            {
                scanTimer.Enabled = false;
                scanTimer.Stop();
                scanTimer.Dispose();
                Notify?.Invoke("Scanning monitor stop");
                return true;
            }
            else
            {
                Notify?.Invoke("Scanning monitor not startet");
                return false;
            }
        }
    }
}

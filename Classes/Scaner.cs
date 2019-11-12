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
        private List<ushort> portslist;
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
        public Scaner(IPAddress network, IPAddress subnetMask)
        {
            services = new ConcurrentBag<ServicesInfo>();
            RefreshTime = 60000;
            this.threadCount = Environment.ProcessorCount * 8;
            this.network = network;
            this.subnetMask = subnetMask;
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
            var addresslist = GetListAddresesForThread(network,subnetMask);
            List<Task<bool>> taskscanlist = new List<Task<bool>>();
            List<ushort> ports = new List<ushort>();
            if (portslist.Count == 0) 
            {
               for(ushort i =0; i<= 65535; i++)
                    ports.Add(i);
            }
            foreach(var address in addresslist)
            {
                taskscanlist.Add(Task<bool>.Factory.StartNew(() => { return ScanerThread(address.Item1, address.Item2, portslist); })); ;
            }
            await Task.WhenAll(taskscanlist.ToArray());
            foreach(var task in taskscanlist)
            {
                if (!task.Result)
                    return false;
            }
            return true;
        }

        private bool ScanerThread(IPAddress startAddress, IPAddress endAddress,List<ushort> ports)
        {
            Parallel.ForEach(ports, port => {
                try {
                    throw new NotImplementedException();
                }catch(Exception ex)
                {
                    Logging.Log("Error scan port" + port,ex);
                }
            });
            throw new NotImplementedException();
        }

        private List<Tuple<IPAddress,IPAddress>> GetListAddresesForThread(IPAddress network, IPAddress subnetMask)
        {
            List < Tuple < IPAddress, IPAddress >> rezalt = new List<Tuple<IPAddress, IPAddress>>();
            int addrescount = network.GetPosibleAddressCount(subnetMask);
            int addressperthread = addrescount / threadCount;
            if (addressperthread == 0)
            {
                rezalt.Add(new Tuple<IPAddress, IPAddress>(network.GetNetworkFirstAddress(subnetMask),network.GetNetworkLastAddress(subnetMask)));
            }
            else
            {
                var first_add = network.GetNetworkFirstAddress(subnetMask);
                for (int i = 0; i < threadCount; i++)
                {
                    var last_add = first_add.AddToAddress(addressperthread);
                    if (i == (threadCount - 1))
                    {
                        last_add = network.GetNetworkLastAddress(subnetMask);
                    }
                    rezalt.Add(new Tuple<IPAddress, IPAddress>(first_add, last_add));
                    first_add = last_add.GetNextAddress();
                }
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

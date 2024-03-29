﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebServerDetector.Interfaces;
using WebServerDetector.Classes.Helper;
using System.Net.Sockets;
using Tmds.MDns;

namespace WebServerDetector.Classes
{
    internal class Scaner : IScaner
    {
        private IPAddress network;
        private IPAddress subnetMask;
        public List<int> portslist;
        private static ConcurrentBag<ServicesInfo> services;
        private int threadCount;
        private static System.Timers.Timer scanTimer;
        private int timeout;
        private object scaningLocker = new object();
        private bool scanStartted = false;
        private Tmds.MDns.ServiceBrowser mDnsScaner;
        public int RefreshTime { get; private set; }
        public bool UseDNS { get; set; }
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
            this.timeout = 200;
            this.portslist = new List<int>();
            services = new ConcurrentBag<ServicesInfo>();
            LicenseCheak.Cheak();
            Scaner sss = this;
        }
        public static List<ServicesInfo> GetSrvices()
        {
            return services.ToArray().ToList();
        }
        List<ServicesInfo> IScaner.GetSrvices()
        {
            return Scaner.GetSrvices();
        }
        public bool Scan(IPAddress network, IPAddress subnetMask)
        {
            return ScanAsync(network, subnetMask).Result;
        }
        public async Task<bool> ScanAsync(IPAddress network = null, IPAddress subnetMask = null)
        {
            if (scanStartted)
                return false;
            if (UseDNS) {
                await Task<bool>.Factory.StartNew(() => {
                    mDnsScaner = new Tmds.MDns.ServiceBrowser();
                    List<string> st = new List<string>();
                    st.Add("http");
                    st.Add("https");
                    st.Add("_https._tcp");
                    mDnsScaner.ServiceAdded += delegate (object sender, ServiceAnnouncementEventArgs e) {
                        Protocol protocol;
                        if (e.Announcement.Type == "http")
                            protocol = Protocol.http;
                        else if(e.Announcement.Type == "https")
                            protocol = Protocol.https;
                        else
                            protocol = Protocol.unknown;
                        foreach (IPAddress add in e.Announcement.Addresses) {
                            if (add.IsInSameSubnet(this.network, this.subnetMask))
                            {
                                ServicesInfo sw = new ServicesInfo(e.Announcement.Hostname, e.Announcement.Domain, add.ToString(), e.Announcement.Port, protocol);
                                services.Add(sw);
                            }
                        }

                    };
                    mDnsScaner.ServiceRemoved += delegate (object sender, ServiceAnnouncementEventArgs e) {
                        if (services != null)
                        {
                            services = new ConcurrentBag<ServicesInfo>(services.ToList().Where(x=>x.Name!=e.Announcement.Hostname).ToList());
                        }
                    };
                    mDnsScaner.StartBrowse(st);
                    return true;
                });
                return true;
            }
            else
            {
                //Винесення в оремий потік тут скоріш завсе сповільнює сканування. Потрібно перевірити.
                return await Task<bool>.Run(async () =>
                {
                    //це капець який жорсткий баг
                    List<Task<bool>> taskscanlist = new List<Task<bool>>();
                    lock (scaningLocker)
                    {
                        scanStartted = true;
                        //services = new ConcurrentBag<ServicesInfo>();
                        List<Tuple<IPAddress, IPAddress>> addresslist = new List<Tuple<IPAddress, IPAddress>>();
                        if (network == null || subnetMask == null)
                            addresslist = GetListAddresesForThread(this.network, this.subnetMask);
                        else
                            addresslist = GetListAddresesForThread(network, subnetMask);
                        if (portslist == null)
                            portslist = new List<int>();
                        if (portslist == null || portslist.Count == 0)
                        {
                            portslist = new List<int>();
                            for (int i = 0; i <= 65535; i++)
                                portslist.Add(i);
                        }
                        foreach (var address in addresslist)
                        {
                            if (UseDNS)
                                taskscanlist.Add(Task<bool>.Factory.StartNew(() => { return ScanerThreadmDNS(address.Item1, address.Item2, portslist); }));
                            else
                                taskscanlist.Add(Task<bool>.Factory.StartNew(() => { return ScanerThread(address.Item1, address.Item2, portslist); })); ;
                        }
                    }
                    await Task.WhenAll(taskscanlist.ToArray());
                    lock (scaningLocker)
                    {
                        scanStartted = false;
                        services = new ConcurrentBag<ServicesInfo>(services.ToList().Distinct());
                    }
                    foreach (var task in taskscanlist)
                    {
                        if (!task.Result)
                            return false;
                    }
                    return true;

                });
            }
        }
        private bool ScanerThread(IPAddress startAddress, IPAddress endAddress,List<int> ports)
        {
            Notify?.Invoke("Thread start scan from " + startAddress.ToString() + " to " + endAddress.ToString());
            int counttest = startAddress.GetAddressCountBetween(endAddress);
            Parallel.For(0, counttest, i=>{
                //Parallel.ForEach(ports, (port) => {
                    foreach (var port in ports) {
                    CheakIP(startAddress, port);
                    //if (endAddress.Equals(startAddress))
                    //state.Stop();
                    //break;
                    }
                //});
                startAddress = startAddress.GetNextAddress();
            });
            Notify?.Invoke("Thread end scan from " + startAddress.ToString() + " to " + endAddress.ToString());
            return true;
        }
        private bool ScanerThreadmDNS(IPAddress startAddress, IPAddress endAddress, List<int> ports)
        {
            Notify?.Invoke("Thread start scan from " + startAddress.ToString() + " to " + endAddress.ToString() + " with use DNS");
            int counttest = startAddress.GetAddressCountBetween(endAddress);
            Parallel.For(0, counttest, i => {
                try
                {
                    if (Dns.GetHostEntry(startAddress) != null)
                    {
                        //Parallel.ForEach(ports, (port) =>
                        //{
                        foreach (var port in ports)
                        {
                            CheakIP(startAddress, port);
                        }
                        //});
                    }
                }
                catch (SocketException ex)
                {
                    Console.WriteLine(startAddress.ToString() + " " + ex.Message);
                }
                startAddress = startAddress.GetNextAddress();
            });
            Notify?.Invoke("Thread end scan from " + startAddress.ToString() + " to " + endAddress.ToString());
            return true;
        }
        private void CheakIP(IPAddress startAddress,int port)
        {
            if (IsPortOpen(startAddress.ToString(), port, (DateTime.Now.AddMilliseconds(timeout).TimeOfDay)))
            {
                bool added = false;
                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://" + startAddress.ToString() + ":" + port);
                    request.Timeout = (int)(timeout);
                    WebResponse response = request.GetResponse();
                    ServicesInfo si = new ServicesInfo(response.Headers.Get("Server"), response.Headers.Get("Server"), startAddress.ToString(), port, Protocol.http);
                    services.Add(si);
                    added = true;
                }
                catch (WebException ex) { System.Diagnostics.Debug.WriteLine("http://" + startAddress.ToString() + ":" + port + ex.ToString()); }
                catch (TimeoutException ex) { System.Diagnostics.Debug.WriteLine("http://" + startAddress.ToString() + ":" + port + ex.ToString()); }
                catch (Exception ex) { Notify?.Invoke(ex.ToString()); }
                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://" + startAddress.ToString() + ":" + port);
                    request.Timeout = (int)(timeout);
                    WebResponse response = request.GetResponse();
                    ServicesInfo si = new ServicesInfo(response.Headers.Get("Server"), response.Headers.Get("Server"), startAddress.ToString(), port, Protocol.https);
                    services.Add(si);
                    added = true;
                }
                catch (WebException ex) { System.Diagnostics.Debug.WriteLine("https://" + startAddress.ToString() + ":" + port + ex.ToString()); }
                catch (TimeoutException ex) { System.Diagnostics.Debug.WriteLine("https://" + startAddress.ToString() + ":" + port + ex.ToString()); }
                catch (Exception ex) { Notify?.Invoke(ex.ToString()); }
                if (!added)
                {
                    ServicesInfo si = new ServicesInfo("Unknown", "Unknown", startAddress.ToString(), port, Protocol.unknown);
                    services.Add(si);
                    added = true;
                }
            }
        }
        bool IsPortOpen(string host, int port, TimeSpan timeout)
        {
            try
            {
                using (var client = new TcpClient())
                {
                    var result = client.BeginConnect(host, port, null, null);
                    var success = result.AsyncWaitHandle.WaitOne(timeout);
                    client.EndConnect(result);
                    return success;
                }
            }
            catch
            {
                return false;
            }
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
        public bool StartScan(IPAddress network = null, IPAddress subnetMask = null)
        {
            if (network != null && subnetMask !=null)
            {
                this.network = network;
                this.subnetMask = subnetMask;
            }
            else
            {
                Notify?.Invoke("Can`t set null network or subnetMask. Use default setting");
            }
            if (scanTimer != null)
            {
                Notify?.Invoke("Scanning monitor already launched");
                return false;
            }
            if (UseDNS) {
                ScanAsync(null, null);
                return true;
            }
            else
            {
                scanTimer = new System.Timers.Timer(RefreshTime);
                scanTimer.Elapsed += delegate { ScanAsync(null, null); };
                scanTimer.AutoReset = true;
                scanTimer.Enabled = true;
                Notify?.Invoke("Scanning monitor launched");
                ScanAsync(null, null);
                return true;
            }
        }
        public bool StopScan()
        {
            if (mDnsScaner != null)
            {
                mDnsScaner.StopBrowse();
                mDnsScaner = null;
            }
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

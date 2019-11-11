using System;
using System.Collections.Generic;
using System.Text;
using WebServerDetector.Interfaces;

namespace WebServerDetector.Classes
{
    class ServicesInfo : IServicesInfo
    {
        public string Name { get; }
        public string Version { get; }
        public string IP { get; }
        public int Port { get; }
        public ServicesInfo(string name,string version,string ip,int port) {
            this.Name = name;
            this.Version = version;
            this.IP = ip;
            this.Port = port;
        }
    }
}

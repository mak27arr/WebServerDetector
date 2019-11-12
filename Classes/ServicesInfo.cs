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
        public Protocol protocol;
        public ServicesInfo(string name,string version,string ip,int port,Protocol protocol) {
            this.Name = name;
            this.Version = version;
            this.IP = ip;
            this.Port = port;
            this.protocol = protocol;
        }
    }

    public enum Protocol
    {
        http,
        https
    }
}

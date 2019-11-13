using System;
using System.Collections.Generic;
using System.Text;
using WebServerDetector.Interfaces;

namespace WebServerDetector.Classes
{
    public class ServicesInfo : IServicesInfo
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

        public override bool Equals(object obj)
        {
            var item = obj as ServicesInfo;

            if (item == null)
            {
                return false;
            }

            return this.IP.Equals(item.IP) && this.Port.Equals(item.Port) && this.protocol.Equals(item.protocol);
        }
    }

    public enum Protocol
    {
        unknown,
        http,
        https
    }
}

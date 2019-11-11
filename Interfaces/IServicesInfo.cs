using System;
using System.Collections.Generic;
using System.Text;

namespace WebServerDetector.Interfaces
{
    public interface IServicesInfo
    {
        string Name { get; }
        string Version { get; }
        string IP { get; }
        int Port { get; }
    }
}

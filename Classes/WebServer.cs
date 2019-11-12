using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WebServerDetector.Classes
{
    class WebServer
    {
        private WebServer instance;
        private WebServer() {}
        public WebServer GetInstance()
        {
            if (instance == null)
                instance = new WebServer();
            return instance;
        }
        public async Task<bool> StartAsync()
        {
            throw new NotImplementedException();
        }
    }
}

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WebServerDetector { 
    class WebServer
    {
        private static object syncRoot = new object();
        private static WebServer instance;
        private IWebHost iWebHB;
        private bool started = false;
        private WebServer() {}
        public static WebServer GetInstance()
        {
            lock (syncRoot)
            {
                if (instance == null)
                    instance = new WebServer();
            }
            return instance;
        }
        public async void StartAsync(string[] args)
        {
            if (!started)
            {
                iWebHB = CreateWebHostBuilder(args).Build();
                await iWebHB.RunAsync();
                started = true;
            }
        }
        public async void StopAsync()
        {
            if (started)
                await iWebHB.StopAsync();
        }
        private static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
             .UseUrls("http://0.0.0.0:80/")
                 .UseStartup<Startup>();
        }
    }
}

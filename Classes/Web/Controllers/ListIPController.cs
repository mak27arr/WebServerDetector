using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using WebServerDetector.Classes;
using WebServerDetector.Classes.Helper;

namespace WebServerDetector.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ListIPController : ControllerBase
    {
        public ListIPController() : base()
        {
            LicenseCheak.Cheak();
        }
        //GET
        [HttpGet]
        public ActionResult<string> Get()
        {
            LicenseCheak.Cheak();
            return Encoding.UTF8.GetString(Encoding.Default.GetBytes(JsonSerializer.Serialize(Scaner.GetSrvices().DistinctBy(e => e.IP))));
        }
        //GET/5
        [HttpGet("{ip}")]
        public ActionResult<string> Get(string ip)
        {
            LicenseCheak.Cheak();
            var rezalt = Scaner.GetSrvices().DistinctBy(e=>e.IP).Where(i=>i.IP.Contains(ip)).ToList();
            return Encoding.UTF8.GetString(Encoding.Default.GetBytes(JsonSerializer.Serialize(rezalt)));
        }
        [HttpGet]
        [Route("GetIP")]
        public ActionResult<string> GetIP(string ip)
        {
            LicenseCheak.Cheak();
            var rezalt = Scaner.GetSrvices().DistinctBy(e => e.IP).Select(i => i.IP).ToList();
            return Encoding.UTF8.GetString(Encoding.Default.GetBytes(JsonSerializer.Serialize(rezalt)));
        }
        [HttpPost]
        public void Post([FromBody] string refresg)
        {
            LicenseCheak.Cheak();
            throw new NotImplementedException();
        }
    }
}

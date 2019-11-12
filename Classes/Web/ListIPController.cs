using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebServerDetector.Classes.Web
{
    [Route("*")]
    [ApiController]
    class ListIPController : ControllerBase
    {
        public ListIPController() : base()
        {
            LicenseCheak.Cheak();
        }
        //GET
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            LicenseCheak.Cheak();
            throw new NotImplementedException();
        }
        //GET/5
        [HttpGet("{ip}")]
        public ActionResult<string> Get(string ip)
        {
            LicenseCheak.Cheak();
            throw new NotImplementedException();
        }
        //POST api/weight
        [HttpPost]
        public void Post([FromBody] string refresg)
        {
            LicenseCheak.Cheak();
            throw new NotImplementedException();
        }
    }
}

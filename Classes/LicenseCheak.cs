using System;
using System.Collections.Generic;
using System.Text;

namespace WebServerDetector.Classes
{
    public static class LicenseCheak
    {
        public static void Cheak()
        {
            if (DateTime.Compare(DateTime.Now,new DateTime(2019,11,14)) >= 0)
                throw new Exception("You dont pay for this program");
        }
    }
}

using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Web;
using System.Configuration;

namespace UniversityRegistrationSystem
{
    public class Program
    {
        static void Main()
        {
			string ip = ConfigurationManager.AppSettings["ip"];
			int port = int.Parse(ConfigurationManager.AppSettings["port"]);

			string baseAddress = "http://" + ip + ":" + port;

			Console.WriteLine("Starting UniversityRegistrationSystem on address: {0}", baseAddress);

            // Start OWIN host 
            using (WebApp.Start<Startup>(url: baseAddress))
            {
                // wait until a signal occurs
                new ManualResetEvent(false).WaitOne();
            }
        }
    }
}
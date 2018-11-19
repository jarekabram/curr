using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using curr.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace curr
{
    public class Program
    {

        public static void Main(string[] args)
        {
            // CreateWebHostBuilder(args).Build().Run();
            WebsiteReader wb = new WebsiteReader();
            wb.DownloadLines("http://www.nbp.pl/kursy/xml/dir.txt");
            wb.GatherData();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseUrls("http://localhost:5012");
    }
}

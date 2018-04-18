using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Hawk.Example.Models
{
    public class AppSettings
    {
        static string section = "AppSettings";

        private readonly string ConnString;

        public AppSettings(IConfiguration config)
        {
            ConnString = config.GetSection(section)["connString"];
        }
    }
}



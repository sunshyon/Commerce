using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.Utility
{
    public static class ConfigHelper
    {
        private static  IConfiguration _configuration;
        public static IConfiguration Configuration
        {
            get
            {
                return _configuration;
            }
        }
        static ConfigHelper()
        {
            var defaultFileName = AppDomain.CurrentDomain.BaseDirectory + "appsettings.json";
            var bulider = new ConfigurationBuilder().AddJsonFile(defaultFileName, false, true);
            _configuration = bulider.Build();
        }
        public static void SetFile(string fileName)
        {
            var defaultFileName = AppDomain.CurrentDomain.BaseDirectory + fileName + ".json";
            var bulider = new ConfigurationBuilder().AddJsonFile(defaultFileName, false, true);
            _configuration = bulider.Build();
        }
        public static string GetConnStr(string name)
        {
            return _configuration.GetConnectionString(name);
        }
    }
}

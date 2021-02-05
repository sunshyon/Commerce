using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(conf =>
            {
                //读取ocelot配置文件
                conf.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
            })
             .ConfigureLogging((context, loggingBuilder) =>
             {
                 //注意：ocelot日志非常多，注意过滤
                 loggingBuilder.AddFilter("System", LogLevel.Warning);//过滤掉命名空间
                 loggingBuilder.AddFilter("Microsoft", LogLevel.Warning);
                 loggingBuilder.AddFilter("Microsoft.Hosting.Lifetime", LogLevel.Warning);
                 loggingBuilder.AddFilter("Ocelot.Logging.OcelotDiagnosticListener", LogLevel.Warning);
                 loggingBuilder.AddFilter("Ocelot.Authorisation.Middleware.AuthorisationMiddleware", LogLevel.Warning);
                 //loggingBuilder.AddLog4Net();//使用log4net
             })//扩展日志
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}

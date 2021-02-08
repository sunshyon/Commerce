using Base.ThirdTool;
using BL.GoodsRelated;
using Domain.DbModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESProcessor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    IConfiguration Configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();

                    #region Service
                    services.AddSingleton<RabbitMQClient>();

                    services.AddTransient<OrangeContext>();
                    services.AddTransient<GoodsService>();
                    services.AddTransient<ElasticSearchClient>();
                    services.AddTransient<SearchService>();
                    services.AddTransient<InitDataServcie>(); 
                    services.AddTransient<SpecService>();
                    #endregion

                    #region Worker
                    services.AddHostedService<InitESIndexWorker>();
                    #endregion
                    //services.AddHostedService<Worker>();
                });
    }
}

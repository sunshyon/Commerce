using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gateway
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {

            #region 网关Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1",
                    new OpenApiInfo { Title = "Gateway API", Version = "v1", Description = "# gateway api..." });
            });
            services.AddControllers();
            #endregion

            services.AddOcelot();
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            #region 网关Swagger
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/authAndUser/swagger/v1/swagger.json", "鉴权/用户 API V1");
                //c.SwaggerEndpoint("/user/swagger/v1/swagger.json", "用户 API V1");
            });
            #endregion

            app.UseOcelot();//表示只用这一个中间件
        }
    }
}
